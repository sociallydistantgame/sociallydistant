using System;
using System.Collections.Generic;
using OS.Network;
using Utility;

namespace GameplaySystems.Networld
{
	public sealed class CoreRouter : IRouter
	{
		private readonly List<Subnet> localSubnetTemplates = new List<Subnet>();
		private int templateIndex;
		private readonly List<LocalAreaNode> localAreaNodes = new List<LocalAreaNode>();
		private readonly List<InternetServiceNode> neighbours = new List<InternetServiceNode>();
		private readonly List<NetworkInterface> neighbourInterfaces = new List<NetworkInterface>();

		public CoreRouter()
		{
			// Create some subnets to use as local address spaces
			var localAddressSpaces = new string[]
			{
				"192.168.1.0/24",
				"192.168.2.0/24",
				"192.168.122.0/24",
				"10.0.0.0/24",
				"10.1.0.0/24"
			};

			foreach (string cidrNetwork in localAddressSpaces)
			{
				if (NetUtility.TryParseCidrNotation(cidrNetwork, out Subnet subnet))
					this.localSubnetTemplates.Add(subnet);
			}
		}
		
		/// <inheritdoc />
		public void NetworkUpdate()
		{
			// Update any local area nodes not connected to a router.
			// This lets us create networks, like the player's, that don't
			// get immediately connected to the Internet.
			for (var i = 0; i < localAreaNodes.Count; i++)
				localAreaNodes[i].NetworkUpdate();
			
			// Update our neighbours. This updates all ISPs in the world.
			for (var i = 0; i < neighbours.Count; i++)
				neighbours[i].NetworkUpdate();
		}

		/// <inheritdoc />
		public IEnumerable<NetworkInterface> Neighbours => neighbourInterfaces;
		
		public LocalAreaNode CreateGhostLan()
		{
			Subnet subnet = localSubnetTemplates[templateIndex];
			templateIndex++;
			if (templateIndex == localSubnetTemplates.Count)
				templateIndex = 0;
			
			var node = new LocalAreaNode(subnet);
			this.localAreaNodes.Add(node);
			return node;
		}

		public void MakeRealNetwork(LocalAreaNode ghostNode, InternetServiceNode internetServiceProvider)
		{
			if (!this.localAreaNodes.Contains(ghostNode))
				throw new InvalidOperationException("Cannot turn the specified LAN into a real network inside this network world, because it is not a ghost node created by this networld.");

			if (!this.neighbours.Contains(internetServiceProvider))
				throw new InvalidOperationException("Cannot convert the specified gost node to a real LAN because the target Internet Service Node isn't part of this network world.");
			
			// Prevent the ghost node from being converted again.
			this.localAreaNodes.Remove(ghostNode);
			
			// Connect the LAN to the ISP.
			internetServiceProvider.ConnectLan(ghostNode);
		}

		public InternetServiceNode CreateServiceProvider(string cidrAddressRange)
		{
			if (!NetUtility.TryParseCidrNotation(cidrAddressRange, out Subnet subnet))
				throw new InvalidOperationException("Cannot parse the CIDR string for the internet service provider's address range.");

			if (localSubnetTemplates.Contains(subnet))
				throw new InvalidOperationException("Cannot use a local address space for an internet service provider.");

			var node = new InternetServiceNode(subnet);
			var neighbourInterface = new NetworkInterface();
			neighbourInterface.MakeAddressable(subnet, subnet.FirstHost);
			node.NetworkInterface.Connect(neighbourInterface);
			this.neighbours.Add(node);
			this.neighbourInterfaces.Add(neighbourInterface);
			return node;
		}

		public void DeleteLocalNode(LocalAreaNode node)
		{
			this.localAreaNodes.Remove(node);
		}

		public void MakeGhostNetwork(LocalAreaNode node)
		{
			if (node.NetworkInterface.Addressable || node.NetworkInterface.Connected)
				throw new InvalidOperationException("LAN nodes cannot be turned into ghost nodes if they're connected to something.");

			if (this.localAreaNodes.Contains(node))
				return;
			
			this.localAreaNodes.Add(node);
		}
	}
}