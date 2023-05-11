#nullable enable

using Core.DataManagement;
using Core.Serialization;
using Core.Systems;
using Core.WorldData.Data;

namespace Core.WorldData
{
	public class World
	{
		public readonly DataObject<WorldRevision, GlobalWorldData> GlobalWorldState;
		public readonly DataObject<WorldRevision, WorldPlayerData> PlayerData;
		public readonly DataTable<WorldComputerData, WorldRevision> Computers;
		public readonly DataTable<WorldInternetServiceProviderData, WorldRevision> InternetProviders;
		public readonly DataTable<WorldLocalNetworkData, WorldRevision> LocalAreaNetworks;
		public readonly DataTable<WorldNetworkConnection, WorldRevision> NetworkConnections;
		public readonly DataTable<WorldPortForwardingRule, WorldRevision> PortForwardingRules;



		public World(UniqueIntGenerator instanceIdGenerator, DataEventDispatcher eventDispatcher)
		{
			GlobalWorldState = new DataObject<WorldRevision, GlobalWorldData>(eventDispatcher);
			PlayerData = new DataObject<WorldRevision, WorldPlayerData>(eventDispatcher);
			Computers = new DataTable<WorldComputerData, WorldRevision>(instanceIdGenerator, eventDispatcher);
			InternetProviders = new DataTable<WorldInternetServiceProviderData, WorldRevision>(instanceIdGenerator, eventDispatcher);
			LocalAreaNetworks = new DataTable<WorldLocalNetworkData, WorldRevision>(instanceIdGenerator, eventDispatcher);
			NetworkConnections = new DataTable<WorldNetworkConnection, WorldRevision>(instanceIdGenerator, eventDispatcher);
			PortForwardingRules = new DataTable<WorldPortForwardingRule, WorldRevision>(instanceIdGenerator, eventDispatcher);
		}

		public void Serialize(IRevisionedSerializer<WorldRevision> serializer)
		{
			GlobalWorldState.Serialize(serializer);
			Computers.Serialize(serializer, WorldRevision.AddedComputers);
			InternetProviders.Serialize(serializer, WorldRevision.AddedInternetServiceProviders);
			LocalAreaNetworks.Serialize(serializer, WorldRevision.AddedInternetServiceProviders);
			NetworkConnections.Serialize(serializer, WorldRevision.AddedInternetServiceProviders);

			// serialize after networking stuff, because it relies on them.
			PlayerData.Serialize(serializer);

			PortForwardingRules.Serialize(serializer, WorldRevision.AddedPortForwarding);
		}

		public void Wipe()
		{
			// You must wipe the world in reverse order of how you would create or serialize it.
			// This ensures proper handling of deleting objects that depend on other objects.
			this.PortForwardingRules.Clear();
			this.PlayerData.Value = default;
			this.NetworkConnections.Clear();
			this.LocalAreaNetworks.Clear();
			this.InternetProviders.Clear();
			this.Computers.Clear();
			this.GlobalWorldState.Value = default;
		}
	}
}