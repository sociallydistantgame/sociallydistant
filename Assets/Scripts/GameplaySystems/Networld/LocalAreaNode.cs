using System;
using System.Collections.Generic;
using System.Linq;
using OS.Network;

namespace GameplaySystems.Networld
{
	public class LocalAreaNode : INetworkSwitch
	{
		private readonly Dictionary<NetworkConnection, DeviceNode> connections = new Dictionary<NetworkConnection, DeviceNode>();
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
			
			// If the source address is that of our outbound interface, send it to the Internet straight away.
			if (packet.SourceAddress == this.outboundInterface.NetworkAddress)
			{
				this.outboundInterface.Send(packet);
				return;
			}
			
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
				if(HandleOwnPacket(packet, true))
					return;
			}
			
			// For packets destined to our outside interface that are also coming from outside of the LAN, handle them
			// internally as well. This allows anyone on the net to ping us.
			//
			// Note: We don't accept these packets if they originate from inside the LAN. IT'S NOT A BUG!
			// The intended behaviour is for the packet to be scooped up by the Internet Service Node and
			// sent straight back to us.
			if ((packet.SourceAddress & this.insideNetwork.Mask) != insideNetwork.NetworkAddress
			    && outboundInterface.Addressable
			    && packet.DestinationAddress == outboundInterface.NetworkAddress)
			{
				if (HandleOwnPacket(packet, false))
					return;
			}
			
			// For packets originating within our network, leave them as-is.
			if ((packet.SourceAddress & this.insideNetwork.Mask) == insideNetwork.NetworkAddress)
			{
				sendQueue.Enqueue(packet);
				return;
			}

			// Find a forwarding rule matching the destination address and port
			PortForwardingRule? rule = forwardingTable.FirstOrDefault(x => (x.OutsideAddress==packet.SourceAddress || x.OutsideAddress==0)
			                                                               && (x.OutsidePort == packet.SourcePort || x.OutsidePort==0)
			                                                               && (x.GlobalAddress == packet.DestinationAddress || x.GlobalAddress == 0)
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

		private bool HandleOwnPacket(Packet packet, bool refuseUnrecognized)
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
					if (!refuseUnrecognized)
						return false;
					
					packet.SwapSourceAndDestination();
					packet.PacketType = PacketType.Refusal;

					break;
				}
			}

			sendQueue.Enqueue(packet);
			return true;
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
			NetworkConnection connection = deviceNode.NetworkConnection;
			this.connections.Add(connection, deviceNode);
			return connection;
		}

		public void DeleteDevice(NetworkConnection connection)
		{
			// Find the device
			if (!connections.TryGetValue(connection, out DeviceNode node))
				return;
			
			// Disconnect it from our LAN.
			node.NetworkInterface.Disconnect();
			
			// Find the corresponding LAN interface, it'll have just been disconnected.
			NetworkInterface? lanInterface = this.insideInterfaces.FirstOrDefault(x => !x.Connected && x != this.NetworkInterface);
			if (lanInterface != null)
			{
				// Remove any forwarding rules associated with it
				for (int i = forwardingTable.Count - 1; i >= 0; i--)
				{
					if (forwardingTable[i].InsideAddress != lanInterface.NetworkAddress)
						continue;
					
					// Drop the port reservation, if any
					PortForwardingRule rule = forwardingTable[i];
					if (portTranslations.ContainsKey((rule.InsideAddress, rule.InsidePort)))
						portTranslations.Remove((rule.InsideAddress, rule.InsidePort));
					
					// Drop the rule itself
					forwardingTable.RemoveAt(i);
				}
				
				// Drop the interface itself.
				insideInterfaces.Remove(lanInterface);
			}
			
			// Find the IP reservation for the node and remove it.
			uint ipReservation = reservations.Keys.FirstOrDefault(x => reservations[x] == node);
			if (reservations.ContainsKey(ipReservation))
				reservations.Remove(ipReservation);
			
			// Drop the device from our list
			devices.Remove(node);
		}
		
		public ForwardingTableEntry GetForwardingRule(NetworkConnection connection, ushort insidePort, ushort outsidePort)
		{
			if (!connections.TryGetValue(connection, out DeviceNode node))
				throw new InvalidOperationException("Device is not a part of this LAN");

			uint insideAddress = node.NetworkInterface.NetworkAddress;
		
			// Find an existing rule matching the criteria
			PortForwardingRule? rule = forwardingTable.FirstOrDefault(x => x.InsideAddress == insideAddress
			                                                               && x.InsidePort == insidePort
			                                                               && x.GlobalAddress == 0
			                                                               && x.GlobalPort == outsidePort);
			
			// Create it if there's none.
			if (rule == null)
			{
				rule = new PortForwardingRule
				{
					InsideAddress = insideAddress,
					InsidePort = insidePort,
					GlobalPort = outsidePort
				};

				forwardingTable.Add(rule);
				
				// Reserve the port in the translation table so we don't try to reserve it somewhere else
				this.portTranslations.Add((insideAddress, insidePort), outsidePort);
			}

			return new ForwardingTableEntry(this.forwardingTable, rule, this.portTranslations);
		}

		public bool ContainsDevice(NetworkConnection connection)
		{
			return this.connections.ContainsKey(connection);
		}
	}
	
	
}