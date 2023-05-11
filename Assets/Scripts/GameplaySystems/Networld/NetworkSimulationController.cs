using System.Collections.Generic;

namespace GameplaySystems.Networld
{
	public sealed class NetworkSimulationController
	{
		private CoreRouter coreRouter;
		private readonly Dictionary<InternetServiceProvider, InternetServiceNode> isps = new Dictionary<InternetServiceProvider, InternetServiceNode>();
		private readonly Dictionary<LocalAreaNetwork, LocalAreaNode> lans = new Dictionary<LocalAreaNetwork, LocalAreaNode>();

		public NetworkSimulationController(CoreRouter coreRouter)
		{
			this.coreRouter = coreRouter;
		}

		internal bool LookupLanNode(LocalAreaNetwork lan, out LocalAreaNode node)
		{
			return lans.TryGetValue(lan, out node);
		}
		
		public InternetServiceProvider CreateInternetServiceProvider(string cidrAddressRange)
		{
			// Create an ISP node in the simulation
			InternetServiceNode ispNode = coreRouter.CreateServiceProvider(cidrAddressRange);
			
			// Create the ISP itself.
			var ispController = new InternetServiceProvider(this, ispNode, coreRouter);

			this.isps.Add(ispController, ispNode);
			
			return ispController;
		}

		public LocalAreaNetwork CreateLocalAreaNetwork()
		{
			// This creates a ghost node. The simulation will still update it so traffic
			// can traverse through the LAN, but there'll be no network connectivity outside 
			// of it.
			LocalAreaNode lanNode = coreRouter.CreateGhostLan();

			var lanController = new LocalAreaNetwork(this, lanNode);

			this.lans.Add(lanController, lanNode);
			
			return lanController;
		}

		public IEnumerable<InternetServiceProvider> GetInternetProviders()
		{
			return isps.Keys;
		}

		public IEnumerable<LocalAreaNetwork> GetLans()
		{
			return lans.Keys;
		}

		public void DeleteLan(LocalAreaNetwork lan)
		{
			if (lan.InternetServiceProvider != null)
				lan.DisconnectFromInternet();

			if (this.lans.TryGetValue(lan, out LocalAreaNode lanNode))
			{
				coreRouter.DeleteLocalNode(lanNode);
				lans.Remove(lan);
			}
		}
	}
}