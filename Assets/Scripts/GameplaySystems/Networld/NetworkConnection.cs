using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OS.Network;
using UnityEngine;
using Utility;

namespace GameplaySystems.Networld
{
	public class NetworkConnection : 
		INetworkConnection
	{
		private DeviceNode deviceNode;
		private const ushort MaximumOutboundConnections = 1024;
		private readonly Dictionary<ushort, Listener> listeners = new Dictionary<ushort, Listener>();

		public string LocalInterfaceName => deviceNode.NetworkInterface.Name;
		public string LoopbackInterfaceName => deviceNode.LoopbackInterface.Name;
		public string LocalAddress => NetUtility.GetNetworkAddressString(deviceNode.NetworkInterface.NetworkAddress);
		public string SubnetMask => NetUtility.GetNetworkAddressString(deviceNode.NetworkInterface.SubnetMask);
		public string LoopbackAddress => NetUtility.GetNetworkAddressString(deviceNode.LoopbackInterface.NetworkAddress);
		
		
		public NetworkConnection(DeviceNode node)
		{
			if (node.NetworkConnection != null)
				throw new InvalidOperationException("Cannot create a NetworkConnection object outside of the constructor of a DeviceNode.");
			
			this.deviceNode = node;
		}

		/// <inheritdoc />
		public IEnumerable<NetworkInterfaceInformation> GetInterfaceInformation()
		{
			yield return deviceNode.LoopbackInterface.GetInterfaceInformation();
			
			NetworkInterfaceInformation mainInfo = deviceNode.NetworkInterface.GetInterfaceInformation();
			mainInfo.DefaultGateway = NetUtility.GetNetworkAddressString(deviceNode.DefaultGateway);
			yield return mainInfo;
		}

		public Task<PingResult> Ping(uint address, float timeuotInSeconds)
		{
			// Completion source for the ping result
			var completionSource = new TaskCompletionSource<PingResult>();
			
			// Create the ping packet
			var pingPacket = new Packet
			{
				PacketType = PacketType.Ping,
				DestinationAddress = address
			};

			// Send it to the net simulation node
			this.deviceNode.EnqueuePacketForDelivery(pingPacket);
			
			// Event for handling Pong packets
			PingResult pingResult = default;
			var handled = false;
			void HandlePongPacket(PacketEvent packetEvent)
			{
				if (handled)
					return;
				
				// Packet was already handled
				if (packetEvent.Handled)
					return;
				
				// Not a pong packet
				if (packetEvent.Packet.PacketType != PacketType.Pong)
					return;
				
				// Done!
				packetEvent.Handle();
				pingResult = PingResult.Pong;
				handled = true;
				
				completionSource.SetResult(pingResult);
				deviceNode.UnhandledPacketReceived -= HandlePongPacket;
			}

			// Subscribe to unhandled packet events
			deviceNode.UnhandledPacketReceived += HandlePongPacket;

			Task.Run(async () =>
			{
				await Task.Delay((int) Mathf.Round(timeuotInSeconds * 1000));
				if (!handled)
				{
					pingResult = PingResult.TimedOut;
					handled = true;
					completionSource.SetResult(pingResult);
					deviceNode.UnhandledPacketReceived -= HandlePongPacket;
				}
			});

			return completionSource.Task;
		}

		private Listener ListenInternal(ushort port, ServerType serverType, SecurityLevel secLevel)
		{
			if (listeners.ContainsKey(port))
				throw new InvalidOperationException("Port " + port + " is already in use.");

			var handle = new ListenerHandle(port, listeners, deviceNode, serverType, secLevel);
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
	}
}