using System.Collections.Generic;
using OS.Network;

namespace GameplaySystems.Networld
{
	public class InternetServiceNode : INetworkSwitch
	{
		private readonly List<NetworkInterface> neighbourInterfaces = new List<NetworkInterface>();
		private readonly List<LocalAreaNode> localAreaNodes = new List<LocalAreaNode>();
		private readonly Dictionary<LocalAreaNode, int> lanInterfaces = new Dictionary<LocalAreaNode, int>();
		private readonly NetworkInterface coreRouter = new NetworkInterface();

		/// <inheritdoc />
		public NetworkInterface NetworkInterface => coreRouter;
		
		/// <inheritdoc />
		public void NetworkUpdate()
		{
			// Update all connected LANs
			for (var i = 0; i < localAreaNodes.Count; i++)
				localAreaNodes[i].NetworkUpdate();
		}

		/// <inheritdoc />
		public IEnumerable<NetworkInterface> Neighbours => neighbourInterfaces;

		public void ConnectLan(LocalAreaNode node)
		{
			// Create an interface for the LAN to connect to
			lanInterfaces.Add(node, neighbourInterfaces.Count);
			var networkInterface = new NetworkInterface();
			this.neighbourInterfaces.Add(networkInterface);

			// Connect the LAN to our network
			node.NetworkInterface.Connect(networkInterface);
			
			// Keep track of the LAN so we can update it
			this.localAreaNodes.Add(node);
		}
	}
}