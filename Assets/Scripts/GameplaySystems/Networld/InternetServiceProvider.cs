using System;
using System.Collections.Generic;
using System.Linq;

namespace GameplaySystems.Networld
{
	public class InternetServiceProvider
	{
		private readonly CoreRouter coreRouter;
		private readonly NetworkSimulationController simulation;
		private readonly InternetServiceNode node;
		private List<Edge<LocalAreaNetwork, InternetServiceProvider>> edges = new List<Edge<LocalAreaNetwork, InternetServiceProvider>>();

		public InternetServiceProvider(NetworkSimulationController simulation, InternetServiceNode node, CoreRouter coreRouter)
		{
			this.simulation = simulation;
			this.node = node;
			this.coreRouter = coreRouter;
		}

		public bool IsConnectedWithLAN(LocalAreaNetwork lan)
		{
			return lan.InternetServiceProvider == this;
		}
		
		public void ConnectLan(Edge<LocalAreaNetwork, InternetServiceProvider> connection, uint publicAddress)
		{
			if (connection.Side1 == null)
				return;

			if (connection.Token != connection.Side1.Token)
				throw new InvalidOperationException("The specified connection link object was not created by the LAN object defined as side 1 of the connection. You are using this API incorrectly and C# provides no friendly way for me to stop you from doing so, and so you got this exception instead. Fucking hell.");

			if (connection.Side2 != null)
				connection.Side1.DisconnectFromInternet();

			connection.Side2 = this;
			this.edges.Add(connection);
			
			// Actually connect the nodes in the simulation.
			if (!simulation.LookupLanNode(connection.Side1, out LocalAreaNode localAreaNode))
				return;
			
			coreRouter.MakeRealNetwork(localAreaNode, this.node, publicAddress);
		}

		public void DisconnectLan(LocalAreaNetwork lan)
		{
			if (lan.InternetServiceProvider != this)
				return;
			
			// Find the edge that has the given LAN as side 1
			Edge<LocalAreaNetwork, InternetServiceProvider>? edge = edges.FirstOrDefault(x => x.Side1 == lan);
			if (edge == null)
				return;

			edges.Remove(edge);
			edge.Side2 = null;
			
			// Disconnect the nodes in the simulation
			if (!simulation.LookupLanNode(edge.Side1, out LocalAreaNode localAreaNode))
				return;
			
			// Drop the interface address for the LAN.
			localAreaNode.NetworkInterface.MakeUnaddressable();
			
			// Drop it on the ISP's end.
			this.node.DropClient(localAreaNode);
			
			// Allow the LAN to still receive network updates
			coreRouter.MakeGhostNetwork(localAreaNode);
		}
	}
}