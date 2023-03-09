﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using OS.Network;
using Steamworks;

namespace GameplaySystems.Networld
{
	public class LocalAreaNode : INetworkSwitch
	{
		private List<NetworkInterface> insideInterfaces = new List<NetworkInterface>();
		private NetworkInterface outboundInterface = new NetworkInterface();
		private List<DeviceNode> devices = new List<DeviceNode>();
		private Subnet insideNetwork;
		private Dictionary<uint, DeviceNode> reservations = new Dictionary<uint, DeviceNode>();
		private uint simulatedDefaultGatewayAddress;
		private Queue<Packet> receivedPackets = new Queue<Packet>();
		private Queue<Packet> sendQueue = new Queue<Packet>();
		private List<PortForwardingRule> forwardingTable = new List<PortForwardingRule>();
		private Dictionary<(uint, ushort), ushort> portTranslations = new Dictionary<(uint, ushort), ushort>();

		public LocalAreaNode(Subnet insideLocalSubnet)
		{
			this.insideNetwork = insideLocalSubnet;
			this.simulatedDefaultGatewayAddress = this.insideNetwork.FirstHost;
			
		}

		/// <inheritdoc />
		public void NetworkUpdate()
		{
			// Read packets from all interfaces
			ReadPackets(this.outboundInterface);
			foreach (NetworkInterface iface in insideInterfaces)
				ReadPackets(iface);
			
			// Process all received packets
			while (receivedPackets.TryDequeue(out Packet packet))
				ProcessPacket(packet);

			// Dispatch packets in the send queue
			while (sendQueue.TryDequeue(out Packet packetToSend))
				Dispatch(packetToSend);
			
				// Update all devices
			for (var i = 0; i < devices.Count; i++)
				devices[i].NetworkUpdate();
		}

		private void Dispatch(Packet packet)
		{
			// Is the destination in our network?
			if ((packet.DestinationAddress & insideNetwork.Mask) == insideNetwork.NetworkAddress)
			{
				// Find a matching interface
				foreach (NetworkInterface iface in this.insideInterfaces)
				{
					if (iface.NetworkAddress == packet.DestinationAddress)
					{
						iface.Send(packet);
						return;
					}
				}

				return;
			}
			
			// The packet is destined outside of the LAN.
			// If our main interface isn't addressable, it means that we're not connected
			// to an ISP in the network simulation and must drop the packet.
			if (!NetworkInterface.Addressable)
				return;
			
			// Find a port-forwarding rule that matches all of the packet's attributes.
			PortForwardingRule? rule = this.forwardingTable.FirstOrDefault(x => x.InsideAddress == packet.SourceAddress
			                                                                    && x.InsidePort == packet.SourcePort
			                                                                    && x.OutsideAddress == packet.DestinationAddress
			                                                                    && x.OutsidePort == packet.DestinationPort);
			
			// If we don't find any, MAKE one.
			if (rule == null)
			{
				rule = new PortForwardingRule
				{
					InsideAddress = packet.SourceAddress,
					InsidePort = packet.SourcePort,
					OutsideAddress = packet.DestinationAddress,
					OutsidePort = packet.DestinationPort
				};
				
				// The global address will be that of our outgoing interface.
				rule.GlobalAddress = NetworkInterface.NetworkAddress;
				
				// Global port is...complicated. These ports aren't treated as Listeners though, they're purely
				// internal. This has the effect of firewalling these outside ports to anyone but the recipient
				// of the packet.
				ushort? globalPort = GetPortTranslation(rule.InsideAddress, rule.InsidePort);
				
				// Drop the packet if we've ran out of ports.
				if (!globalPort.HasValue)
					return;

				rule.GlobalPort = globalPort.Value;
				forwardingTable.Add(rule);
			}

			// Change the source attributes to match the rule.
			packet.SourceAddress = rule.GlobalAddress;
			packet.SourcePort = rule.GlobalPort;
			
			// Take a spin, now you're in with the techno set. We goin' surfin' on the fuckin' Internet.
			this.outboundInterface.Send(packet);
		}

		private ushort? GetPortTranslation(uint insideAddress, ushort insidePort)
		{
			if (this.portTranslations.TryGetValue((insideAddress, insidePort), out ushort port))
				return port;
			
			// Ow. My heap.
			ushort[] reservedPortsForAddress = portTranslations.Where(x => x.Key.Item1 == insideAddress)
				.Select(x => x.Value)
				.ToArray();
			
			for (ushort i = 0; i < ushort.MaxValue; i++)
			{
				if (!reservedPortsForAddress.Contains(i))
				{
					portTranslations.Add((insideAddress, insidePort), i);
					return i;
				}
			}

			return null;
		}

		private void ProcessPacket(Packet packet)
		{
			// If the packet is destined to us and is from within the LAN, handle
			// the packet as our own.
			if ((packet.SourceAddress & this.insideNetwork.Mask) == insideNetwork.NetworkAddress
			    && packet.DestinationAddress == insideNetwork.FirstHost)
			{
				HandleOwnPacket(packet);
				return;
			}
			
			// For packets originating within our network, leave them as-is.
			if ((packet.SourceAddress & this.insideNetwork.Mask) == insideNetwork.NetworkAddress)
			{
				sendQueue.Enqueue(packet);
				return;
			}
			
			// Find a forwarding rule matching the destination address and port
			PortForwardingRule? rule = forwardingTable.FirstOrDefault(x => x.OutsideAddress == packet.SourcePort
			                                                               && x.OutsidePort == packet.SourcePort
			                                                               && x.GlobalAddress == packet.DestinationAddress
			                                                               && x.GlobalPort == packet.DestinationPort);
			
			// If no rule was found, refuse the packet
			if (rule == null)
			{
				packet.SwapSourceAndDestination();
				packet.PacketType = PacketType.Refusal;
				sendQueue.Append(packet);
				return;
			}
			
			// Apply the translation as per the rule
			packet.DestinationAddress = rule.InsideAddress;
			packet.DestinationPort = rule.InsidePort;
			sendQueue.Enqueue(packet);
		}

		private void HandleOwnPacket(Packet packet)
		{
			switch (packet.PacketType)
			{
				case PacketType.Ping:
				{
					packet.SwapSourceAndDestination();
					packet.PacketType = PacketType.Pong;
					break;
				}
				default:
				{
					packet.SwapSourceAndDestination();
					packet.PacketType = PacketType.Refusal;
					break;
				}
			}

			sendQueue.Enqueue(packet);
		}
		
		private void ReadPackets(NetworkInterface iface)
		{
			Packet? packet = iface.Receive();
			if (packet != null)
				this.receivedPackets.Enqueue(packet.Value);
		}
		
		/// <inheritdoc />
		public IEnumerable<NetworkInterface> Neighbours => insideInterfaces;

		/// <inheritdoc />
		public NetworkInterface NetworkInterface => outboundInterface;

		public NetworkConnection SetUpNewDevice()
		{
			// Reserve an IP address for the device
			uint networkAddress = simulatedDefaultGatewayAddress + 1;
			while (reservations.ContainsKey(networkAddress))
				networkAddress++;

			if (networkAddress > insideNetwork.LastHost)
				throw new InvalidOperationException("Maximum number of device hosts reached for this LAN.");
			
			// Create the new device node... pretend this line of code just magically bought a computer.
			// We don't know what computer it is but it has an Ethernet port and it's our job to
			// wire it up.
			var deviceNode = new DeviceNode(insideNetwork, simulatedDefaultGatewayAddress, networkAddress);
			devices.Add(deviceNode);
			reservations.Add(networkAddress, deviceNode);

			// Now pretend we're a network switch that can have infinitely many Ethernet ports on it, but we must
			// ask the universe to create more ports on our chassis before we can use them. This does that.
			var newInterface = new NetworkInterface();
			this.insideInterfaces.Add(newInterface);
			
			// Configure the new interface to have the same IP address and subnet as the device connected to it.
			newInterface.MakeAddressable(this.insideNetwork, networkAddress);
			
			// Now, grab an Ethernet patch cable and connect the device to the network!
			deviceNode.NetworkInterface.Connect(newInterface);
			
			// Return the NetworkConnection that the rest of the game is allowed to use.
			return deviceNode.NetworkConnection;
		}
	}

	public class PortForwardingRule
	{
		public uint GlobalAddress { get; set; }
		public uint InsideAddress { get; set; }
		public uint OutsideAddress { get; set; }
		public ushort InsidePort { get; set; }
		public ushort OutsidePort { get; set; }
		public ushort GlobalPort { get; set; }
	}
}