using SociallyDistant.Core.OS.Network;
using SociallyDistant.OS.Network;

namespace SociallyDistant.GameplaySystems.Networld
{
	public class InternetServiceNode : INetworkSwitch<NetworkInterface>
	{
		private readonly IHostNameResolver hostResolver;
		private readonly List<NetworkInterface> neighbourInterfaces = new List<NetworkInterface>();
		private readonly List<LocalAreaNode> localAreaNodes = new List<LocalAreaNode>();
		private readonly Dictionary<LocalAreaNode, int> lanInterfaces = new Dictionary<LocalAreaNode, int>();
		private readonly NetworkInterface coreRouter = new NetworkInterface();
		private readonly Subnet addressRange;

		/// <inheritdoc />
		public NetworkInterface NetworkInterface => coreRouter;

		public InternetServiceNode(Subnet addressRange, IHostNameResolver hostResolver)
		{
			this.addressRange = addressRange;
			this.NetworkInterface.MakeAddressable(addressRange, addressRange.FirstHost);
			this.hostResolver = hostResolver;
		}

		/// <inheritdoc />
		public void NetworkUpdate()
		{
			foreach (LocalAreaNode node in localAreaNodes)
			{
				node.NetworkUpdate();
			}
			
			var continueReading = false;

			do
			{
				continueReading = false;

				continueReading |= ReadCorePackets();
				continueReading |= ReadNeighbours();
			} while (continueReading);
		}

		private bool ReadCorePackets()
		{
			Packet? corePacket = null;
			if ((corePacket = NetworkInterface.Receive()) != null)
			{
				ProcessPacket(corePacket.Value);
				return true;
			}

			return false;
		}

		private bool ReadNeighbours()
		{
			var result = false;
			foreach (NetworkInterface iface in this.Neighbours)
			{
				Packet? nextPacket = null;
				if ((nextPacket = iface.Receive()) != null)
				{
					ProcessPacket(nextPacket.Value);
					result |= true;
				}
			}

			return result;
		}
		
		private void ProcessPacket(Packet packet)
		{
			uint destination = packet.DestinationAddress;

			NetworkInterface? mostSpecificNetwork = null;
			foreach (NetworkInterface iface in Neighbours)
			{
				if (iface.NetworkAddress == destination)
				{
					mostSpecificNetwork = iface;
					break;
				}
				
				if ((destination & iface.SubnetMask) != (iface.NetworkAddress & iface.SubnetMask))
					continue;

				if (mostSpecificNetwork != null)
				{
					int mostSpecificBitCount = NetUtility.CountBits(mostSpecificNetwork.SubnetMask);
					int ourBitCount = NetUtility.CountBits(iface.SubnetMask);
					if (ourBitCount > mostSpecificBitCount)
						mostSpecificNetwork = iface;
					continue;
				}

				//mostSpecificNetwork = iface;
			}

			if (mostSpecificNetwork == null)
			{
				// TODO: Allow core router to handle sending packets to other ISPs in the game world.
				// mostSpecificNetwork = NetworkInterface;
				
				// Instead of doing what's described above, send a void packet back to whoever sent this packet.
				Packet voidPacket = packet.Clone();
				voidPacket.PacketType = PacketType.Void;
				voidPacket.SwapSourceAndDestination();

				// immediately send this void packet
				ProcessPacket(voidPacket);
				return;
			}

			mostSpecificNetwork.Send(packet);
		}
		
		/// <inheritdoc />
		public IEnumerable<NetworkInterface> Neighbours => neighbourInterfaces;

		/// <inheritdoc />
		public IHostNameResolver HostResolver => hostResolver;

		private uint? GetNextPublicAddress()
		{
			uint firstPublic = addressRange.FirstHost + 1;

			foreach (NetworkInterface neighbour in Neighbours)
			{
				if ((neighbour.NetworkAddress & neighbour.SubnetMask) != addressRange.networkAddress)
					continue;

				if (neighbour.NetworkAddress == firstPublic)
					firstPublic++;
			}

			if (firstPublic > addressRange.LastHost)
				return null;

			return firstPublic;
		}
		
		public void ConnectLan(LocalAreaNode node, uint publicAddress)
		{
			var networkInterface = new NetworkInterface();
			
			SimulationThread.ScheduleWork(() =>
			{
				// Create an interface for the LAN to connect to
				lanInterfaces.Add(node, neighbourInterfaces.Count);
				this.neighbourInterfaces.Add(networkInterface);

				// Connect the LAN to our network
				node.NetworkInterface.Connect(networkInterface);

				// Make the two interfaces addressable
				networkInterface.MakeAddressable(addressRange, publicAddress);
				node.NetworkInterface.MakeAddressable(addressRange, publicAddress);

				// Keep track of the LAN so we can update it
				this.localAreaNodes.Add(node);
			});
		}

		public void DropClient(LocalAreaNode node)
		{
			SimulationThread.ScheduleWork(() =>
			{
				// Which interface was this node connected to? If none, we stop.
				if (!lanInterfaces.TryGetValue(node, out int index))
					return;

				// Get that interface.
				NetworkInterface ourInterface = this.neighbourInterfaces[index];

				// Remove it.
				this.neighbourInterfaces.RemoveAt(index);
				this.lanInterfaces.Remove(node);
				this.localAreaNodes.Remove(node);

				// Shift all interface indices above the one we just axed
				foreach (LocalAreaNode key in lanInterfaces.Keys.ToArray())
				{
					if (lanInterfaces[key] > index)
						lanInterfaces[key]--;
				}

				// Drop the addresses on both interfaces
				ourInterface.MakeUnaddressable();
				node.NetworkInterface.MakeUnaddressable();

				// Disconnect them.
				node.NetworkInterface.Disconnect();
			});
		}
	}
}