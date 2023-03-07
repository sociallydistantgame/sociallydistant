using System;
using System.Collections.Generic;
using System.Linq;
using OS.Network;
using UnityEditor.VersionControl;

namespace GameplaySystems.Networld
{
	public sealed class NetworkSimulationController
	{
		private CoreRouter coreRouter;

		public NetworkSimulationController(CoreRouter coreRouter)
		{
			this.coreRouter = coreRouter;
		}

		private InternetServiceProvider CreateInternetServiceProvider()
		{
			// Create an ISP node in the simulation
			InternetServiceNode ispNode = coreRouter.CreateServiceProvider();
			
			// Create the ISP itself.
			return new InternetServiceProvider(this, ispNode);
		}

		public LocalAreaNetwork CreateLocalAreaNetwork()
		{
			// This creates a ghost node. The simulation will still update it so traffic
			// can traverse through the LAN, but there'll be no network connectivity outside 
			// of it.
			LocalAreaNode lanNode = coreRouter.CreateGhostLan();

			return new LocalAreaNetwork(this, lanNode);
		}
	}

	public class InternetServiceProvider
	{
		private readonly NetworkSimulationController simulation;
		private readonly InternetServiceNode node;
		private List<Edge<LocalAreaNetwork, InternetServiceProvider>> edges = new List<Edge<LocalAreaNetwork, InternetServiceProvider>>();

		public InternetServiceProvider(NetworkSimulationController simulation, InternetServiceNode node)
		{
			this.simulation = simulation;
			this.node = node;
		}

		public void ConnectLan(Edge<LocalAreaNetwork, InternetServiceProvider> connection)
		{
			if (connection.Side1 == null)
				return;

			if (connection.Token != connection.Side1.Token)
				throw new InvalidOperationException("The specified connection link object was not created by the LAN object defined as side 1 of the connection. You are using this API incorrectly and C# provides no friendly way for me to stop you from doing so, and so you got this exception instead. Fucking hell.");

			if (connection.Side2 != null)
				connection.Side1.DisconnectFromInternet();

			connection.Side2 = this;
			this.edges.Add(connection);
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
		}
	}

	public class LocalAreaNetwork
	{
		private Edge<LocalAreaNetwork, InternetServiceProvider> connection = new Edge<LocalAreaNetwork, InternetServiceProvider>();
		private NetworkSimulationController simulation;
		private LocalAreaNode node;
		
		public Guid Token => connection.Token;

		public InternetServiceProvider? InternetServiceProvider => connection.Side2;

		public LocalAreaNetwork(NetworkSimulationController simulation, LocalAreaNode node)
		{
			this.simulation = simulation;
			this.node = node;
		}
		
		public void ConnectToInternet(InternetServiceProvider isp)
		{
			DisconnectFromInternet();

			isp.ConnectLan(this.connection);
		}

		public void DisconnectFromInternet()
		{
			if (connection.Side2 == null)
				return;

			connection.Side2.DisconnectLan(this);
		}

		public NetworkConnection CreateDevice()
		{
			return this.node.SetUpNewDevice();
		}
	}
	
	public class Edge<T1, T2>
	{
		public T1? Side1 { get; set; }
		public T2? Side2 { get; set; }

		public Guid Token { get; } = Guid.NewGuid();
	}
}