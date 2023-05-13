using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Serialization.Binary;
using OS.Network;
using UnityEngine;

namespace GameplaySystems.Networld
{
	public class Listener
	{
		private ListenerHandle handle;

		private Dictionary<uint, ConnectionHandle> connections = new Dictionary<uint, ConnectionHandle>();
		private Queue<Connection> pendingConnections = new Queue<Connection>();

		public ServerInfo ServerInfo => handle.ServerInfo;
		
		public Listener(ListenerHandle handle)
		{
			this.handle = handle;
			this.handle.PacketReceived += OnPacketReceived;
		}

		public Connection? AcceptConnection()
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

						var connectionHandle = new ConnectionHandle(this, id.Value, packetEvent.Packet.SourceAddress, packetEvent.Packet.SourcePort);
						this.connections.Add(id.Value, connectionHandle);

						this.pendingConnections.Enqueue(new Connection(connectionHandle));
						
						// Notify the sender that we've accepted the connection
						Packet response = packetEvent.Packet.Clone();
						response.SwapSourceAndDestination();
						response.PacketType = PacketType.ConnectAccept;
						
						// Encode the connection ID
						var transmissionPacket = new TransmissionProtocolMessage();
						transmissionPacket.ConnectionId = id.Value;

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

						// Create a new handle
						var connectionHandle = new ConnectionHandle(this, transmissionPacket.ConnectionId, packetEvent.Packet.SourceAddress, packetEvent.Packet.SourcePort);
						connections.Add(transmissionPacket.ConnectionId, connectionHandle);

						this.pendingConnections.Enqueue(new Connection(connectionHandle));
						
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

		internal class ConnectionHandle
		{
			private Listener listener;
			private uint connectionId;
			private uint remoteADdress;
			private ushort remotePort;
			private Queue<byte[]> receivedData = new Queue<byte[]>();

			public ServerInfo ServerInfo => listener.ServerInfo;

			public bool IsValid => listener != null
			                       && listener.connections.ContainsKey(connectionId);
			
			public ConnectionHandle(Listener listener, uint connectionId, uint remoteAddress, ushort remotePort)
			{
				this.listener = listener;
				this.connectionId = connectionId;
				this.remoteADdress = remoteAddress;
				this.remotePort = remotePort;
			}

			public void EnqueueReceivedData(byte[] data)
			{
				if (data.Length == 0)
					return;
				this.receivedData.Enqueue(data);
			}

			public bool TryDequeueReceivedData(out byte[] data)
			{
				return receivedData.TryDequeue(out data);
			}

			public void Send(byte[] data)
			{
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
		}
	}
}