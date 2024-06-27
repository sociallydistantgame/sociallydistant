using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Serialization.Binary;
using OS.Network;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameplaySystems.Networld
{
	public class Listener : IListener
	{
		private static readonly Dictionary<Guid, SharedConnectionState> sharedStates = new Dictionary<Guid, SharedConnectionState>();
		private ListenerHandle handle;

		private Dictionary<uint, ConnectionHandle> connections = new Dictionary<uint, ConnectionHandle>();
		private Queue<Connection> pendingConnections = new Queue<Connection>();

		public ServerInfo ServerInfo => handle.ServerInfo;
		
		public Listener(ListenerHandle handle)
		{
			this.handle = handle;
			this.handle.PacketReceived += OnPacketReceived;
		}

		public IConnection? AcceptConnection()
		{
			if (this.pendingConnections.TryDequeue(out Connection connection))
				return connection;
			
			return null;
		}
		
		private uint? GetNextConnectionId()
		{
			uint id = 0;
			while (id < uint.MaxValue)
			{
				if (!connections.ContainsKey(id))
					return id;
				
				id++;
			}

			return null;
		}
		
		private void OnPacketReceived(PacketEvent packetEvent)
		{
			if (packetEvent.Handled)
				return;

			try
			{
				switch (packetEvent.Packet.PacketType)
				{
					case PacketType.Connect:
					{
						uint? id = GetNextConnectionId();

						if (!id.HasValue)
						{
							packetEvent.Refuse();
							return;
						}

						// Generate shared state for the new connection. This state is for the hacking system to be able to
						// interact with remote devices when a connection is made.
						var stateId = Guid.NewGuid();
						var connectionState = new SharedConnectionState
						{
							ServerInfo = this.ServerInfo
						};

						sharedStates.Add(stateId, connectionState);
						
						var connectionHandle = new ConnectionHandle(this, id.Value, packetEvent.Packet.SourceAddress, packetEvent.Packet.SourcePort, connectionState);
						this.connections.Add(id.Value, connectionHandle);

						this.pendingConnections.Enqueue(new Connection(connectionHandle));
						
						// Notify the sender that we've accepted the connection
						Packet response = packetEvent.Packet.Clone();
						response.SwapSourceAndDestination();
						response.PacketType = PacketType.ConnectAccept;

						// Encode the connection ID
						var transmissionPacket = new TransmissionProtocolMessage();
						transmissionPacket.ConnectionId = id.Value;
						transmissionPacket.Data = stateId.ToByteArray();

						using (var ms = new MemoryStream())
						{
							using var writer = new BinaryWriter(ms, Encoding.UTF8);
							using var dataWriter = new BinaryDataWriter(writer);
							
							transmissionPacket.Write(dataWriter);
							response.Data = ms.ToArray();
						}
						
						// Send it!
						packetEvent.Handle(response);
						break;
					}
					case PacketType.ConnectAccept:
					{
						// Read the ID that was sent to us by the remote host.
						using var ms = new MemoryStream(packetEvent.Packet.Data);
						using var binaryReader = new BinaryReader(ms, Encoding.UTF8);
						using var dataReader = new BinaryDataReader(binaryReader);

						var transmissionPacket = new TransmissionProtocolMessage();
						transmissionPacket.Read(dataReader);

						// We should have received the shared state ID in the packet. Decode it and find the state.
						var sharedSTateId = new Guid(transmissionPacket.Data);
						var sharedState = sharedStates[sharedSTateId];

						// Store our client info.
						sharedState.ClientInfo = this.ServerInfo;
						
						// Remove the state from the dictionary since handshake is done
						sharedStates.Remove(sharedSTateId);
						
						// Create a new handle
						var connectionHandle = new ConnectionHandle(this, transmissionPacket.ConnectionId, packetEvent.Packet.SourceAddress, packetEvent.Packet.SourcePort, sharedState);
						connections.Add(transmissionPacket.ConnectionId, connectionHandle);

						this.pendingConnections.Enqueue(new Connection(connectionHandle));
						
						break;
					}
					case PacketType.Disconnect:
					{
						using var ms = new MemoryStream(packetEvent.Packet.Data);
						using var binaryReader = new BinaryReader(ms, Encoding.UTF8);
						using var dataReader = new BinaryDataReader(binaryReader);

						var transmissionPacket = new TransmissionProtocolMessage();
						transmissionPacket.Read(dataReader);

						if (!connections.TryGetValue(transmissionPacket.ConnectionId, out ConnectionHandle connection))
						{
							packetEvent.Handle();
							return;
						}

						connection.Close();
						break;
					}
					case PacketType.Transmission:
					{
						using var ms = new MemoryStream(packetEvent.Packet.Data);
						using var binaryReader = new BinaryReader(ms, Encoding.UTF8);
						using var dataReader = new BinaryDataReader(binaryReader);

						var transmissionPacket = new TransmissionProtocolMessage();
						transmissionPacket.Read(dataReader);

						if (!connections.ContainsKey(transmissionPacket.ConnectionId))
						{
							packetEvent.Refuse();
							return;
						}

						connections[transmissionPacket.ConnectionId].EnqueueReceivedData(transmissionPacket.Data);
						
						break;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				packetEvent.Refuse();
			}
		}

		public void Close()
		{
			this.handle.Invalidate();
		}

		private void CloseConnectionInternal(uint id)
		{
			if (!this.connections.TryGetValue(id, out ConnectionHandle connection))
				return;

			// tell the other side we're hanging up

			using (var ms = new MemoryStream())
			{
				using (var writer = new BinaryWriter(ms, Encoding.UTF8, true))
				{
					var dataWriter = new BinaryDataWriter(writer);
					var transmission = new TransmissionProtocolMessage();
					transmission.ConnectionId = id;

					transmission.Write(dataWriter);
				}

				ms.Seek(0, SeekOrigin.Begin);
				var packet = new Packet
				{
					PacketType = PacketType.Disconnect,
					Data = ms.ToArray(),
					DestinationAddress = connection.RemoteAddress,
					DestinationPort = connection.RemotePort
				};

				handle.Send(packet);
			}

			// Drop the connection on our end.
			this.connections.Remove(id);
		}
		
		internal class ConnectionHandle
		{
			private Listener listener;
			private uint connectionId;
			private uint remoteADdress;
			private ushort remotePort;
			private Queue<byte[]> receivedData = new Queue<byte[]>();
			private SharedConnectionStateHandle sharedStateHandle;

			public uint RemoteAddress => remoteADdress;
			public ushort RemotePort => remotePort;
			
			public ServerInfo ServerInfo => sharedStateHandle.ServerInfo;

			public bool IsValid => listener != null
			                       && listener.connections.ContainsKey(connectionId);
			
			public ConnectionHandle(Listener listener, uint connectionId, uint remoteAddress, ushort remotePort, SharedConnectionState sharedState)
			{
				this.listener = listener;
				this.connectionId = connectionId;
				this.remoteADdress = remoteAddress;
				this.remotePort = remotePort;
				this.sharedStateHandle = new SharedConnectionStateHandle(sharedState);
			}

			public void EnqueueReceivedData(byte[] data)
			{
				if (data.Length == 0)
					return;
				this.receivedData.Enqueue(data);
			}

			public bool TryDequeueReceivedData(out byte[] data)
			{
				if (!IsValid)
					throw new IOException($"The connection to {NetUtility.GetNetworkAddressString(RemoteAddress)}:{RemotePort} has been closed.");

				return receivedData.TryDequeue(out data);
			}

			public void Send(byte[] data)
			{
				if (!IsValid)
					throw new IOException($"The connection to {NetUtility.GetNetworkAddressString(RemoteAddress)}:{RemotePort} has been closed.");
				
				if (data.Length == 0)
					return;

				var transportPacket = new TransmissionProtocolMessage
				{
					ConnectionId = connectionId,
					Data = data
				};

				var actualPacket = new Packet
				{
					PacketType = PacketType.Transmission,
					DestinationAddress = remoteADdress,
					DestinationPort = remotePort
				};
				
				using (var ms = new MemoryStream())
				{
					using var binaryWriter = new BinaryWriter(ms, Encoding.UTF8);
					using var dataWriter = new BinaryDataWriter(binaryWriter);

					transportPacket.Write(dataWriter);

					actualPacket.Data = ms.ToArray();
				}

				listener.handle.Send(actualPacket);
			}

			public void Close()
			{
				listener?.CloseConnectionInternal(connectionId);
				listener = null;
			}
		}
	}

	public class SharedConnectionState
	{
		public ServerInfo ClientInfo { get; set; }
		public ServerInfo ServerInfo { get; set; }
	}

	public class SharedConnectionStateHandle
	{
		private readonly SharedConnectionState sharedState;

		public SharedConnectionStateHandle(SharedConnectionState state)
		{
			this.sharedState = state;
		}

		public ServerInfo ServerInfo => sharedState.ServerInfo;
		public ServerInfo ClientInfo => sharedState.ClientInfo;
	}
}