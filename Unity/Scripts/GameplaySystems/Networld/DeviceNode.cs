using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePlatform;
using OS.Devices;
using OS.Network;
using Steamworks;
using UnityEngine.InputSystem.LowLevel;
using Utility;

namespace GameplaySystems.Networld
{
	public class DeviceNode : 
		IDeviceWithNetworkInterface<NetworkInterface>,
		INetworkNode
	{
		private NetworkInterface deviceInterface = new NetworkInterface();
		private LoopbackInterface loopbackInterface = new LoopbackInterface();
		private readonly NetworkConnection connection;
		private Subnet localSubnet;
		private uint defaultGateway;
		private uint localAddress;

		public LoopbackInterface LoopbackInterface => loopbackInterface;
		public uint DefaultGateway => defaultGateway;

		public event Action<PacketEvent> UnhandledPacketReceived; 

		public NetworkConnection NetworkConnection => connection;
		public IComputer Computer { get; }

		public DeviceNode(Subnet localSubnet, uint defaultGateway, uint localAddress, IComputer computer, IHostNameResolver hostResolver)
		{
			Computer = computer;
			
			this.localSubnet = localSubnet;
			this.defaultGateway = defaultGateway;
			this.localAddress = localAddress;
			
			// Make the interface addressable
			this.deviceInterface.MakeAddressable(localSubnet, localAddress);
			
			// Create the NetworkConnection that controls us.
			var newConnection = new NetworkConnection(this, new DeviceHostResolver(hostResolver, this));
			this.connection = newConnection;
		}

		public void EnqueuePacketForDelivery(Packet packetToSend)
		{
			// If destination matches loopback address, set source address to match it...
			// and send it to the loopback interface.
			if ((packetToSend.DestinationAddress & 0xff000000) == (loopbackInterface.NetworkAddress & 0xff000000))
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

		/// <inheritdoc />
		public async Task NetworkUpdate()
		{
			// Receive from the loopback interface.
			Packet? loopbackPacket = null;
			if ((loopbackPacket = loopbackInterface.Receive()) != null)
			{
				OnReceivePacket(loopbackPacket.Value, loopbackInterface);
			}
			// Receive from the gateway interface
			Packet? gatewayPacket = null;
			if ((gatewayPacket = deviceInterface.Receive()) != null)
			{
				OnReceivePacket(gatewayPacket.Value, deviceInterface);
			}
		}

		private void OnReceivePacket(Packet packet, ISimulationNetworkInterface sourceInterface)
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
				case PacketType.IcmpPing:
				{
					Packet response = packet.Clone();
					response.SwapSourceAndDestination();

					if (this.connection.IsListening(packet.DestinationPort))
					{
						response.PacketType = PacketType.IcmpAck;
					}
					// TODO: Hackables
					// TODO: Firewalls
					else
					{
						response.PacketType = PacketType.IcmpReject;
					}
					
					sourceInterface.Send(response);
					break;
				}
				default:
				{
					GameManager.ScheduleAction(() =>
					{
						var packetEvent = new PacketEvent(packet, this);

						UnhandledPacketReceived?.Invoke(packetEvent);

						if (packetEvent.Refused)
						{
							Packet response = packet.Clone();
							response.SwapSourceAndDestination();
							response.PacketType = PacketType.Refusal;
							sourceInterface.Send(response);
						}
						else if (!packetEvent.Handled)
						{
							// the packet went unhandled so we'll try again in another simulation tick
							EnqueuePacketForDelivery(packet);
						}
					});
					break;
				}
			}
		}

		/// <inheritdoc />
		public NetworkInterface NetworkInterface => deviceInterface;
	}

	public sealed class DeviceHostResolver : IHostNameResolver
	{
		private readonly DeviceNode deviceNode;
		private readonly IHostNameResolver upstreamResolver;

		public DeviceHostResolver(IHostNameResolver upstreamResolver, DeviceNode deviceNode)
		{
			this.upstreamResolver = upstreamResolver;
			this.deviceNode = deviceNode;
		}

		/// <inheritdoc />
		public bool IsValidSubnet(uint address)
		{
			// 127.0.0.0/8
			if ((address & 0xff000000) == (NetUtility.LoopbackAddress & 0xff000000))
				return true;

			NetworkInterface deviceInterface = deviceNode.NetworkInterface;

			// anything in the device's current LAN is valid.
			if ((address & deviceInterface.SubnetMask) == (deviceInterface.NetworkAddress & deviceInterface.SubnetMask))
				return true;

			return upstreamResolver.IsValidSubnet(address);
		}

		/// <inheritdoc />
		public string ReverseHostLookup(uint networkAddress)
		{
			if (networkAddress == NetUtility.LoopbackAddress)
				return "localhost";

			return upstreamResolver.ReverseHostLookup(networkAddress);
		}

		/// <inheritdoc />
		public uint? HostLookup(string hostname)
		{
			if (hostname == "localhost")
				return NetUtility.LoopbackAddress;

			return upstreamResolver.HostLookup(hostname);
		}
	}
}