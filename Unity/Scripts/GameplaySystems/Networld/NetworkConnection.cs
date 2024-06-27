using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GamePlatform;
using OS.Network;
using UnityEngine;
using Utility;

namespace GameplaySystems.Networld
{
	public class NetworkConnection : 
		INetworkConnection
	{
		private readonly Guid id = Guid.NewGuid();
		private readonly object sync = new object();
		private readonly List<PacketQueue> queues = new List<PacketQueue>();
		private readonly IHostNameResolver hostResolver;
		private DeviceNode deviceNode;
		private const ushort MaximumOutboundConnections = 1024;
		private readonly Dictionary<ushort, Listener> listeners = new Dictionary<ushort, Listener>();

		public string LocalInterfaceName => deviceNode.NetworkInterface.Name;
		public string LoopbackInterfaceName => deviceNode.LoopbackInterface.Name;
		public string LocalAddress => NetUtility.GetNetworkAddressString(deviceNode.NetworkInterface.NetworkAddress);
		public string SubnetMask => NetUtility.GetNetworkAddressString(deviceNode.NetworkInterface.SubnetMask);
		public string LoopbackAddress => NetUtility.GetNetworkAddressString(deviceNode.LoopbackInterface.NetworkAddress);
		
		
		public NetworkConnection(DeviceNode node, IHostNameResolver hostResolver)
		{
			this.hostResolver = hostResolver;

			if (node.NetworkConnection != null)
				throw new InvalidOperationException("Cannot create a NetworkConnection object outside of the constructor of a DeviceNode.");
			
			this.deviceNode = node;
			this.deviceNode.UnhandledPacketReceived += HandlePacketReceived;
		}

		private void HandlePacketReceived(PacketEvent packetEvent)
		{
			lock (sync)
			{
				Packet packet = packetEvent.Packet;
				for (int i = 0; i < queues.Count; i++)
				{
					PacketQueue queue = queues[i];
					queue.Enqueue(packet);
				}

				packetEvent.Handle();
			}
		}

		/// <inheritdoc />
		public IEnumerable<NetworkInterfaceInformation> GetInterfaceInformation()
		{
			yield return deviceNode.LoopbackInterface.GetInterfaceInformation();
			
			NetworkInterfaceInformation mainInfo = deviceNode.NetworkInterface.GetInterfaceInformation();
			mainInfo.DefaultGateway = NetUtility.GetNetworkAddressString(deviceNode.DefaultGateway);
			yield return mainInfo;
		}

		/// <inheritdoc />
		public Guid Identifier => id;

		/// <inheritdoc />
		public bool Connected => this.deviceNode.NetworkInterface.Connected;

		public async Task<PingResult> Ping(uint address, float timeuotInSeconds, bool acceptVoidPackets)
		{
			// Find out if the address is actually a real one in the world. No need to use the network simulation if that's the case, we can
			// fake a timeout depending on whether we accept void packets.
			//
			// For known non-existent addresses, waiting for simulation is a waste of time and CPU.
			if (!this.hostResolver.IsValidSubnet(address))
			{
				// if we aren't accepting void packets from the simulation, we fake a full timeout because that's
				// what the simulation would do.
				if (!acceptVoidPackets)
					await Task.Delay((int) Mathf.Round(timeuotInSeconds * 1000));

				return PingResult.TimedOut;
			}
			
			using var queue = new PacketQueue(this, acceptVoidPackets);
			var tokenSource = new CancellationTokenSource();

			// Create the ping packet
			var pingPacket = new Packet
			{
				PacketType = PacketType.Ping,
				DestinationAddress = address
			};

			// Send it to the net simulation node
			this.deviceNode.EnqueuePacketForDelivery(pingPacket);

			// Event for handling Pong packets
			var pingResult = PingResult.Pong;

			Task<bool> pingTask = WaitForPong(address, queue, tokenSource.Token);
			
			bool receivedPong = await Task.WhenAny(pingTask, Task.Delay((int) Mathf.Round(timeuotInSeconds * 1000))) == pingTask;

			if (!receivedPong || !pingTask.Result)
			{
				tokenSource.Cancel();
				pingResult = PingResult.TimedOut;
			}
			
			return pingResult;
		}

		private async Task<bool> WaitForPong(uint address, PacketQueue queue, CancellationToken cancelToken)
		{
			while (true)
			{
				cancelToken.ThrowIfCancellationRequested();
				Packet packet = await queue.Dequeue(cancelToken);
				
				// not for us
				if (packet.SourceAddress != address)
					continue;

				if (packet.PacketType == PacketType.Void)
					return false;
				
				// Not a pong packet
				if (packet.PacketType != PacketType.Pong)
					continue;

				break;
			}

			return true;
		}
		
		private Listener ListenInternal(ushort port, ServerType serverType, SecurityLevel secLevel)
		{
			if (listeners.ContainsKey(port))
				throw new InvalidOperationException("Port " + port + " is already in use.");

			var queue = new PacketQueue(this);
			var handle = new ListenerHandle(port, listeners, deviceNode, queue, serverType, secLevel);
			var listener = new Listener(handle);
			listeners.Add(port, listener);

			return listener;
		}
		
		public IListener Listen(ushort port, ServerType serverType = ServerType.Netcat, SecurityLevel secLevel = SecurityLevel.Open)
		{
			if (port >= (ushort.MaxValue - MaximumOutboundConnections))
				Debug.LogWarning("Creating a listener on port " + port + " for in-bound connections is generally not advised. You are taking up one of the " + MaximumOutboundConnections + " reserved ports for out-bound traffic.");

			return ListenInternal(port, serverType, secLevel);
		}

		/// <inheritdoc />
		public bool Resolve(string host, out uint address)
		{
			uint? result = hostResolver.HostLookup(host);

			address = result.GetValueOrDefault();

			return result.HasValue;
		}

		/// <inheritdoc />
		public async Task<PortScanResult> ScanPort(uint address, ushort port)
		{
			// Ping the host first. No sense scanning a port if the host is down.
			PingResult pingResult = await Ping(address, 4, true);
			if (pingResult != PingResult.Pong)
				return new PortScanResult(port, PortStatus.Closed, ServerType.Unknown);

			using var queue = new PacketQueue(this, true);
			
			// Create the ping packet
			var pingPacket = new Packet
			{
				PacketType = PacketType.IcmpPing,
				DestinationAddress = address,
				DestinationPort = port
			};

			// Send it to the net simulation node
			this.deviceNode.EnqueuePacketForDelivery(pingPacket);
			
			// Event for handling Pong packets
			var tokenSource = new CancellationTokenSource();
			Task<PortScanResult> waitTask = WaitForIcmpPacket(address, port, queue, tokenSource.Token);

			bool success = await Task.WhenAny(waitTask, Task.Delay(4000)) == waitTask;

			if (!success)
			{
				tokenSource.Cancel();
				return new PortScanResult(port, PortStatus.Closed, ServerType.Unknown);
			}

			return waitTask.Result;
		}

		private async Task<PortScanResult> WaitForIcmpPacket(uint address, ushort port, PacketQueue queue, CancellationToken token)
		{
			while (true)
			{
				token.ThrowIfCancellationRequested();
				Packet packet = await queue.Dequeue(token);

				if (packet.SourceAddress != address && packet.SourcePort != port)
					continue;

				// Not a pong packet
				if (packet.PacketType != PacketType.IcmpAck && packet.PacketType != PacketType.IcmpReject)
					continue;
				
				// Done!
				return new PortScanResult(
					packet.SourcePort,
					packet.PacketType == PacketType.IcmpAck ? PortStatus.Open : PortStatus.Closed,
					NetUtility.DetectServerType(packet.SourcePort)
				);
			}
		}

		/// <inheritdoc />
		public bool IsListening(ushort port)
		{
			return listeners.ContainsKey(port);
		}

		/// <inheritdoc />
		public string GetHostName(uint address)
		{
			return hostResolver.ReverseHostLookup(address) ?? NetUtility.GetNetworkAddressString(address);
		}

		public async Task<ConnectionResult> Connect(uint remoteAddress, ushort port)
		{
			ConnectionResult result = default;
			
			ushort outboundPort = ushort.MaxValue - MaximumOutboundConnections;

			Listener? localListener = null;
			while (outboundPort < ushort.MaxValue)
			{
				if (!listeners.ContainsKey(outboundPort))
				{
					localListener = ListenInternal(outboundPort, ServerType.Netcat, SecurityLevel.Open);
					break;
				}

				outboundPort++;
			}

			if (localListener == null)
			{
				result.Result = ConnectionResultType.MaximumConnectionsReached;
				return result;
			}
			
			// Send a connect packet
			var packet = new Packet
			{
				PacketType = PacketType.Connect,
				SourcePort = outboundPort,
				DestinationAddress = remoteAddress,
				DestinationPort = port
			};
			deviceNode.EnqueuePacketForDelivery(packet);
			
			// Wait.
			float timeout = 30;
			while (timeout > 0)
			{
				result.Connection = localListener.AcceptConnection();
				if (result.Connection != null)
				{
					result.Result = ConnectionResultType.Connected;
					return result;
				}

				timeout -= Time.deltaTime;
				await UniTask.DelayFrame(1);
			}

			if (result.Connection == null)
			{
				result.Result = ConnectionResultType.TimedOut;
				localListener.Close();
			}

			return result;
		}

		private sealed class PacketQueue : IPacketQueue
		{
			private volatile bool deleted;
			
			private readonly bool acceptVoidPackets;
			private readonly NetworkConnection connection;
			private readonly ConcurrentQueue<Packet> queue = new ConcurrentQueue<Packet>();

			public bool Deleted => deleted;
			
			public PacketQueue(NetworkConnection connection, bool acceptVoidPackets = false)
			{
				this.connection = connection;
				lock (connection.sync)
					this.connection.queues.Add(this);
				this.acceptVoidPackets = acceptVoidPackets;
			}

			/// <inheritdoc />
			public event Action<Packet> Received;

			/// <inheritdoc />
			public bool TryDequeue(out Packet packet)
			{
				if (deleted)
				{
					packet = default;
					return false;
				}
				
				return queue.TryDequeue(out packet);
			}

			public void Enqueue(Packet packet)
			{
				if (deleted)
					return;
				
				if (packet.PacketType == PacketType.Void && !acceptVoidPackets)
					return;
				
				queue.Enqueue(packet);
				Received?.Invoke(packet);
			}

			public async Task<Packet> Dequeue(CancellationToken token)
			{
				if (deleted)
					return default;
				
				Packet packet = default;

				while (!queue.TryDequeue(out packet))
				{
					if (deleted)
						break;
					
					token.ThrowIfCancellationRequested();
					await Task.Yield();
				}

				return packet;
			}

			/// <inheritdoc />
			public async void Dispose()
			{
				deleted = true;

				lock (connection.sync)
					connection.queues.Remove(this);
			}
		}
	}
}