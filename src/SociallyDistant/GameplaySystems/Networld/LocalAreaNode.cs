using System.Diagnostics;
using Serilog;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.Network;
using SociallyDistant.OS.Network;

namespace SociallyDistant.GameplaySystems.Networld
{
	public class LocalAreaNode : INetworkSwitch<NetworkInterface>
	{
		private readonly IHostNameResolver hostResolver;
		private readonly Dictionary<Guid, DeviceNode> connections = new Dictionary<Guid, DeviceNode>();
		private List<NetworkInterface> insideInterfaces = new List<NetworkInterface>();
		private NetworkInterface outboundInterface = new NetworkInterface();
		private List<DeviceNode> devices = new List<DeviceNode>();
		private Subnet insideNetwork;
		private Dictionary<uint, DeviceNode> reservations = new Dictionary<uint, DeviceNode>();
		private uint simulatedDefaultGatewayAddress;
		private List<PortForwardingRule> forwardingTable = new List<PortForwardingRule>();
		private Dictionary<(uint, ushort), ushort> portTranslations = new Dictionary<(uint, ushort), ushort>();

		public LocalAreaNode(Subnet insideLocalSubnet, IHostNameResolver worldHostResolver)
		{
			this.insideNetwork = insideLocalSubnet;
			this.simulatedDefaultGatewayAddress = this.insideNetwork.FirstHost;
			this.hostResolver = worldHostResolver;
		}

		/// <inheritdoc />
		public void NetworkUpdate()
		{
			foreach (DeviceNode device in devices)
			{
				device.NetworkUpdate();
			}
			
			var continueREading = false;
			do
			{
				continueREading = false;

				// Read packets from all interfaces
				continueREading |= ReadPackets(this.outboundInterface);
				foreach (NetworkInterface iface in insideInterfaces)
					continueREading |= ReadPackets(iface);
			} while (continueREading);
		}

		private void Dispatch(Packet packet)
		{
			// Is the destination in our network?
			if ((packet.DestinationAddress & insideNetwork.mask) == insideNetwork.networkAddress)
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

				// If this was an ICMP ping, and no device handled the packet, send back an ICMP reject.
				if (packet.PacketType == PacketType.IcmpPing)
				{
					packet.SwapSourceAndDestination();
					packet.PacketType = PacketType.IcmpReject;
					Dispatch(packet);
				}
				
				return;
			}
			
			// The packet is destined outside of the LAN.
			// If our main interface isn't addressable, it means that we're not connected
			// to an ISP in the network simulation and must drop the packet.
			if (!NetworkInterface.Addressable)
			{
				Log.Warning("Packet traversing through a LocalAreaNode is outbound, and the LAN has no connection to the Net. Packet will be dropped.");
				return;
			}

			// If the source address is that of our outbound interface, send it to the Internet straight away.
			if (packet.SourceAddress == this.outboundInterface.NetworkAddress)
			{
				this.outboundInterface.Send(packet);
				return;
			}
			
			// Find a port-forwarding rule that matches all of the packet's attributes.
			PortForwardingRule? rule = this.forwardingTable.FirstOrDefault(x => x.IsForOutgoing 
																				&& x.InsideAddress == packet.SourceAddress
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
					OutsidePort = packet.DestinationPort,
					IsForOutgoing = true
				};
				
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
			packet.SourceAddress = NetworkInterface.NetworkAddress;
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
			if ((packet.SourceAddress & this.insideNetwork.mask) == insideNetwork.networkAddress
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
			if ((packet.SourceAddress & this.insideNetwork.mask) != insideNetwork.networkAddress
			    && outboundInterface.Addressable
			    && packet.DestinationAddress == outboundInterface.NetworkAddress)
			{
				if (HandleOwnPacket(packet, false))
					return;
			}
			
			// For packets originating within our network, leave them as-is.
			if ((packet.SourceAddress & this.insideNetwork.mask) == insideNetwork.networkAddress)
			{
				Dispatch(packet);
				return;
			}

			// Find a forwarding rule matching the destination address and port
			PortForwardingRule? rule = forwardingTable.FirstOrDefault(x => (x.OutsideAddress==packet.SourceAddress || x.OutsideAddress==0)
			                                                               && (x.OutsidePort == packet.SourcePort || x.OutsidePort==0)
			                                                               && x.GlobalPort == packet.DestinationPort);
			
			// If no rule was found, refuse the packet
			if (rule == null)
			{
				packet.SwapSourceAndDestination();
				// for ICMP pings we send an ICMP reject in this case
				if (packet.PacketType == PacketType.IcmpPing)
					packet.PacketType = PacketType.IcmpReject;
				else 
					packet.PacketType = PacketType.Refusal;
				Dispatch(packet);
				return;
			}
			
			// Apply the translation as per the rule
			packet.DestinationAddress = rule.InsideAddress;
			packet.DestinationPort = rule.InsidePort;
			Dispatch(packet);
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
				case PacketType.IcmpPing:
				{
					bool isPortOpen = this.forwardingTable.Any(x => x.GlobalPort == packet.DestinationPort);
					
					packet.SwapSourceAndDestination();

					if (isPortOpen)
						packet.PacketType = PacketType.IcmpAck;
					else
						packet.PacketType = PacketType.IcmpReject;
					
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

			Dispatch(packet);
			return true;
		}
		
		private bool ReadPackets(NetworkInterface iface)
		{
			Packet? packet = null;
			if ((packet = iface.Receive()) != null)
			{
				ProcessPacket(packet.Value);
				return true;
			}

			return false;
		}
		
		/// <inheritdoc />
		public IEnumerable<NetworkInterface> Neighbours => insideInterfaces;

		/// <inheritdoc />
		public IHostNameResolver HostResolver => hostResolver;

		/// <inheritdoc />
		public NetworkInterface NetworkInterface => outboundInterface;

		public NetworkConnection SetUpNewDevice(IComputer computer)
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
			var deviceNode = new DeviceNode(insideNetwork, simulatedDefaultGatewayAddress, networkAddress, computer, hostResolver);
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
			this.connections.Add(connection.Identifier, deviceNode);
			return connection;
		}

		public void DeleteDevice(NetworkConnection connection)
		{
			// Find the device
			if (!connections.TryGetValue(connection.Identifier, out DeviceNode node))
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
		
		public ForwardingTableEntry GetForwardingRule(INetworkConnection connection, ushort insidePort, ushort outsidePort)
		{
			if (!connections.TryGetValue(connection.Identifier, out DeviceNode node))
				throw new InvalidOperationException("Device is not a part of this LAN");

			uint insideAddress = node.NetworkInterface.NetworkAddress;
		
			// Find an existing rule matching the criteria
			PortForwardingRule? rule = forwardingTable.FirstOrDefault(x => x.InsideAddress == insideAddress
			                                                               && x.InsidePort == insidePort
			                                                               && !x.IsForOutgoing
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

		public bool ContainsDevice(INetworkConnection connection)
		{
			return this.connections.ContainsKey(connection.Identifier);
		}
	}
	
	
}