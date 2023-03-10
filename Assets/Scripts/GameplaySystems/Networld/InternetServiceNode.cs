using System;
using System.Collections.Generic;
using OS.Network;
using Utility;

namespace GameplaySystems.Networld
{
	public class InternetServiceNode : INetworkSwitch
	{
		private readonly List<NetworkInterface> neighbourInterfaces = new List<NetworkInterface>();
		private readonly List<LocalAreaNode> localAreaNodes = new List<LocalAreaNode>();
		private readonly Dictionary<LocalAreaNode, int> lanInterfaces = new Dictionary<LocalAreaNode, int>();
		private readonly NetworkInterface coreRouter = new NetworkInterface();
		private readonly Queue<Packet> packetQueue = new Queue<Packet>();
		private readonly Subnet addressRange;

		/// <inheritdoc />
		public NetworkInterface NetworkInterface => coreRouter;

		public InternetServiceNode(Subnet addressRange)
		{
			this.addressRange = addressRange;
			this.NetworkInterface.MakeAddressable(addressRange, addressRange.FirstHost);
		}
		
		/// <inheritdoc />
		public void NetworkUpdate()
		{
			Packet? corePacket = NetworkInterface.Receive();
			if (corePacket != null)
				packetQueue.Enqueue(corePacket.Value);
			
			foreach (NetworkInterface iface in this.Neighbours)
			{
				Packet? packet = iface.Receive();
				if (packet == null)
					continue;

				packetQueue.Enqueue(packet.Value);
			}

			while (packetQueue.TryDequeue(out Packet packet))
				ProcessPacket(packet);
			
			// Update all connected LANs
			for (var i = 0; i < localAreaNodes.Count; i++)
				localAreaNodes[i].NetworkUpdate();
		}

		private void ProcessPacket(Packet packet)
		{
			uint destination = packet.DestinationAddress;

			NetworkInterface? mostSpecificNetwork = null;
			foreach (NetworkInterface iface in Neighbours)
			{
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

				mostSpecificNetwork = iface;
			}

			if (mostSpecificNetwork == null)
				mostSpecificNetwork = NetworkInterface;

			mostSpecificNetwork.Send(packet);
		}
		
		/// <inheritdoc />
		public IEnumerable<NetworkInterface> Neighbours => neighbourInterfaces;

		private uint? GetNextPublicAddress()
		{
			uint firstPublic = addressRange.FirstHost + 1;

			foreach (NetworkInterface neighbour in Neighbours)
			{
				if ((neighbour.NetworkAddress & neighbour.SubnetMask) != addressRange.NetworkAddress)
					continue;

				if (neighbour.NetworkAddress == firstPublic)
					firstPublic++;
			}

			if (firstPublic > addressRange.LastHost)
				return null;

			return firstPublic;
		}
		
		public void ConnectLan(LocalAreaNode node)
		{
			uint? addr = GetNextPublicAddress();
			if (addr == null)
				throw new InvalidOperationException("Maximum LAN count reached.");

			// Create an interface for the LAN to connect to
			lanInterfaces.Add(node, neighbourInterfaces.Count);
			var networkInterface = new NetworkInterface();
			this.neighbourInterfaces.Add(networkInterface);

			// Connect the LAN to our network
			node.NetworkInterface.Connect(networkInterface);
			
			// Make the two interfaces addressable
			networkInterface.MakeAddressable(addressRange, addr.Value);
			node.NetworkInterface.MakeAddressable(addressRange, addr.Value);
			
			// Keep track of the LAN so we can update it
			this.localAreaNodes.Add(node);
		}

		public void DropClient(LocalAreaNode node)
		{
			// Which interface was this node connected to? If none, we stop.
			if (!lanInterfaces.TryGetValue(node, out int index))
				return;
			
			// Get that interface.
			NetworkInterface ourInterface = this.neighbourInterfaces[index];
			
			// Remove it.
			this.neighbourInterfaces.RemoveAt(index);
			this.lanInterfaces.Remove(node);
			
			// Shift all interface indices above the one we just axed
			foreach (LocalAreaNode key in lanInterfaces.Keys)
			{
				if (lanInterfaces[key] > index)
					lanInterfaces[key]--;
			}
			
			// Drop the addresses on both interfaces
			ourInterface.MakeUnaddressable();
			node.NetworkInterface.MakeUnaddressable();
			
			// Disconnect them.
			node.NetworkInterface.Disconnect();
		}
	}
}