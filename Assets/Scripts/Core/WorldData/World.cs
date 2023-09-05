#nullable enable

using Core.DataManagement;
using Core.Serialization;
using Core.Systems;
using Core.WorldData.Data;

namespace Core.WorldData
{
	public class World : IWorld
	{
		public readonly WorldDataObject<GlobalWorldData> GlobalWorldState;
		public readonly WorldDataObject<WorldPlayerData> PlayerData;
		public readonly WorldDataTable<WorldComputerData> Computers;
		public readonly WorldDataTable<WorldInternetServiceProviderData> InternetProviders;
		public readonly WorldDataTable<WorldLocalNetworkData> LocalAreaNetworks;
		public readonly WorldDataTable<WorldNetworkConnection> NetworkConnections;
		public readonly WorldDataTable<WorldPortForwardingRule> PortForwardingRules;
		public readonly WorldDataTable<WorldCraftedExploitData> CraftedExploits;
		public readonly WorldDataTable<WorldHackableData> Hackables;

		public World(UniqueIntGenerator instanceIdGenerator, DataEventDispatcher eventDispatcher)
		{
			GlobalWorldState = new WorldDataObject<GlobalWorldData>(eventDispatcher);
			PlayerData = new WorldDataObject<WorldPlayerData>(eventDispatcher);
			Computers = new WorldDataTable<WorldComputerData>(instanceIdGenerator, eventDispatcher);
			InternetProviders = new WorldDataTable<WorldInternetServiceProviderData>(instanceIdGenerator, eventDispatcher);
			LocalAreaNetworks = new WorldDataTable<WorldLocalNetworkData>(instanceIdGenerator, eventDispatcher);
			NetworkConnections = new WorldDataTable<WorldNetworkConnection>(instanceIdGenerator, eventDispatcher);
			PortForwardingRules = new WorldDataTable<WorldPortForwardingRule>(instanceIdGenerator, eventDispatcher);
			CraftedExploits = new WorldDataTable<WorldCraftedExploitData>(instanceIdGenerator, eventDispatcher);
			Hackables = new WorldDataTable<WorldHackableData>(instanceIdGenerator, eventDispatcher);
			
		}

		public void Serialize(IWorldSerializer serializer)
		{
			GlobalWorldState.Serialize(serializer);
			Computers.Serialize(serializer, WorldRevision.AddedComputers);
			InternetProviders.Serialize(serializer, WorldRevision.AddedInternetServiceProviders);
			LocalAreaNetworks.Serialize(serializer, WorldRevision.AddedInternetServiceProviders);
			NetworkConnections.Serialize(serializer, WorldRevision.AddedInternetServiceProviders);

			// serialize after networking stuff, because it relies on them.
			PlayerData.Serialize(serializer);

			Hackables.Serialize(serializer, WorldRevision.AddedHackables);
			PortForwardingRules.Serialize(serializer, WorldRevision.AddedPortForwarding);
			
			CraftedExploits.Serialize(serializer, WorldRevision.AddedExploitCrafting);
		}

		public void Wipe()
		{
			// You must wipe the world in reverse order of how you would create or serialize it.
			// This ensures proper handling of deleting objects that depend on other objects.
			this.CraftedExploits.Clear();
			this.PortForwardingRules.Clear();
			this.Hackables.Clear();
			this.PlayerData.Value = default;
			this.NetworkConnections.Clear();
			this.LocalAreaNetworks.Clear();
			this.InternetProviders.Clear();
			this.Computers.Clear();
			this.GlobalWorldState.Value = default;
		}
	}
}