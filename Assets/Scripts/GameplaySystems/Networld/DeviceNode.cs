using System;
using System.Collections;
using System.Collections.Generic;
using OS.Network;
using UnityEngine.InputSystem.LowLevel;
using Utility;

namespace GameplaySystems.Networld
{
	public class DeviceNode : 
		IDeviceWithNetworkInterface,
		INetworkNode
	{
		private NetworkInterface deviceInterface = new NetworkInterface();
		private NetworkInterface loopbackInterface;
		private readonly NetworkConnection connection;
		private Subnet localSubnet;
		private uint defaultGateway;
		private uint localAddress;
		private NetworkInterface loopbackOutput;
		private readonly Queue<Packet> sendQueue = new Queue<Packet>();

		public NetworkInterface LoopbackInterface => loopbackInterface;
		public uint DefaultGateway => defaultGateway;

		public event Action<PacketEvent> UnhandledPacketReceived; 

		public NetworkConnection NetworkConnection => connection;

		public DeviceNode(Subnet localSubnet, uint defaultGateway, uint localAddress)
		{
			this.localSubnet = localSubnet;
			this.defaultGateway = defaultGateway;
			this.localAddress = localAddress;
			
			// Make the interface addressable
			this.deviceInterface.MakeAddressable(localSubnet, localAddress);
			
			// Create a loopback interface that...loops back to us.
			this.loopbackInterface = new NetworkInterface("lo");
			this.loopbackInterface.MakeAddressable(NetUtility.LoopbackSubnet, NetUtility.LoopbackAddress);

			// The loopback output interface is how we actually read what's sent to 127.0.0.1.
			// It's confusing as fuck but essentially:
			// - Game exposes the 127.0.0.1 iface we created above
			// - Game sends packet through that interface
			// - We read that packet through loopback output
			// - We send packet to the relevant Listener based solely on the port.
			this.loopbackOutput = new NetworkInterface("lo");
			this.loopbackOutput.Connect(this.loopbackInterface);
			
			// Create the NetworkConnection that controls us.
			var newConnection = new NetworkConnection(this);
			this.connection = newConnection;
		}

		public void EnqueuePacketForDelivery(Packet packet)
		{
			this.sendQueue.Enqueue(packet);
		}
		
		/// <inheritdoc />
		public void NetworkUpdate()
		{
			// Dispatch packets in the send queue
			while (sendQueue.TryDequeue(out Packet packetToSend))
			{
				// If destination matches loopback address, set source address to match it...
				// and send it to the loopback interface.
				if (packetToSend.DestinationAddress == loopbackInterface.NetworkAddress)
				{
					packetToSend.SourceAddress = packetToSend.DestinationAddress;
					loopbackInterface.Send(packetToSend);
				}
				
				// Otherwise we set the source to our LAN address and send the packet to our gateway.
				else
				{
					packetToSend.SourceAddress = this.deviceInterface.NetworkAddress;
					this.deviceInterface.Send(packetToSend);
				}
			}
			
			// Anything that comes out of loopback-out, goes right back in.
			Packet? loopbackOutputPacket = null;
			while ((loopbackOutputPacket = loopbackOutput.Receive()) != null)
			{
				loopbackOutput.Send(loopbackOutputPacket.Value);
			}
			
			// Receive from the loopback interface.
			Packet? loopbackPacket = null;
			while ((loopbackPacket = loopbackInterface.Receive()) != null)
			{
				OnReceivePacket(loopbackPacket.Value, loopbackInterface);
			}
			
			// Receive from the gateway interface
			Packet? gatewayPacket= null;
			while ((gatewayPacket = deviceInterface.Receive()) != null)
			{
				OnReceivePacket(gatewayPacket.Value, deviceInterface);
			}
		}

		private void OnReceivePacket(Packet packet, NetworkInterface sourceInterface)
		{
			switch (packet.PacketType)
			{
				case PacketType.Ping:
				{
					Packet response = packet.Clone();

					response.SwapSourceAndDestination();
					response.PacketType = PacketType.Pong;

					sourceInterface.Send(response);
					break;
				}
				default:
				{
					var packetEvent = new PacketEvent(packet, sendQueue);

					UnhandledPacketReceived?.Invoke(packetEvent);

					if (packetEvent.Refused)
					{
						Packet response = packet.Clone();
						response.SwapSourceAndDestination();
						response.PacketType = PacketType.Refusal;
						sourceInterface.Send(response);
					}

					break;
				}
			}
		}

		/// <inheritdoc />
		public NetworkInterface NetworkInterface => deviceInterface;
	}
}