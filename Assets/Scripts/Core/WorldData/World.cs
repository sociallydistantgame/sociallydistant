#nullable enable

using System;
using Architecture;
using Core.DataManagement;
using Core.Serialization;
using Core.Systems;
using Core.WorldData.Data;

namespace Core.WorldData
{
	public class World : IWorld
	{
		private readonly WorldDataObject<ProtectedWorldState> protectedWorldState;
		private readonly WorldDataObject<GlobalWorldData> globalWorldState;
		private readonly WorldDataObject<WorldPlayerData> playerData;
		private readonly WorldDataTable<WorldComputerData> computers;
		private readonly WorldDataTable<WorldInternetServiceProviderData> internetProviders;
		private readonly WorldDataTable<WorldLocalNetworkData> localAreaNetworks;
		private readonly WorldDataTable<WorldNetworkConnection> networkConnections;
		private readonly WorldDataTable<WorldPortForwardingRule> portForwardingRules;
		private readonly WorldDataTable<WorldCraftedExploitData> craftedExploits;
		private readonly WorldDataTable<WorldHackableData> hackables;

		
        /// <inheritdoc />
        public IWorldDataObject<GlobalWorldData> GlobalWorldState => globalWorldState;
		
        /// <inheritdoc />
        public IWorldDataObject<WorldPlayerData> PlayerData => playerData;
		
        /// <inheritdoc />
        public IWorldTable<WorldComputerData> Computers => computers;
		
        /// <inheritdoc />
        public IWorldTable<WorldInternetServiceProviderData> InternetProviders => internetProviders;
		
        /// <inheritdoc />
        public IWorldTable<WorldLocalNetworkData> LocalAreaNetworks => localAreaNetworks;
		
        /// <inheritdoc />
        public IWorldTable<WorldNetworkConnection> NetworkConnections => networkConnections;
		
        /// <inheritdoc />
        public IWorldTable<WorldPortForwardingRule> PortForwardingRules => portForwardingRules;
		
        /// <inheritdoc />
        public IWorldTable<WorldCraftedExploitData> CraftedExploits => craftedExploits;
		
        /// <inheritdoc />
        public IWorldTable<WorldHackableData> Hackables => hackables;
		
		public World(UniqueIntGenerator instanceIdGenerator, DataEventDispatcher eventDispatcher)
		{
			protectedWorldState = new WorldDataObject<ProtectedWorldState>(eventDispatcher);
			globalWorldState = new WorldDataObject<GlobalWorldData>(eventDispatcher);
			playerData = new WorldDataObject<WorldPlayerData>(eventDispatcher);
			computers = new WorldDataTable<WorldComputerData>(instanceIdGenerator, eventDispatcher);
			internetProviders = new WorldDataTable<WorldInternetServiceProviderData>(instanceIdGenerator, eventDispatcher);
			localAreaNetworks = new WorldDataTable<WorldLocalNetworkData>(instanceIdGenerator, eventDispatcher);
			networkConnections = new WorldDataTable<WorldNetworkConnection>(instanceIdGenerator, eventDispatcher);
			portForwardingRules = new WorldDataTable<WorldPortForwardingRule>(instanceIdGenerator, eventDispatcher);
			craftedExploits = new WorldDataTable<WorldCraftedExploitData>(instanceIdGenerator, eventDispatcher);
			hackables = new WorldDataTable<WorldHackableData>(instanceIdGenerator, eventDispatcher);
			
		}

		public void Serialize(IWorldSerializer serializer)
		{
			protectedWorldState.Serialize(serializer);
			globalWorldState.Serialize(serializer);
			computers.Serialize(serializer, WorldRevision.AddedComputers);
			internetProviders.Serialize(serializer, WorldRevision.AddedInternetServiceProviders);
			localAreaNetworks.Serialize(serializer, WorldRevision.AddedInternetServiceProviders);
			networkConnections.Serialize(serializer, WorldRevision.AddedInternetServiceProviders);

			// serialize after networking stuff, because it relies on them.
			playerData.Serialize(serializer);

			hackables.Serialize(serializer, WorldRevision.AddedHackables);
			portForwardingRules.Serialize(serializer, WorldRevision.AddedPortForwarding);
			
			craftedExploits.Serialize(serializer, WorldRevision.AddedExploitCrafting);
		}

		public void Wipe()
		{
			// You must wipe the world in reverse order of how you would create or serialize it.
			// This ensures proper handling of deleting objects that depend on other objects.
			this.craftedExploits.Clear();
			this.portForwardingRules.Clear();
			this.hackables.Clear();
			this.playerData.Value = default;
			this.networkConnections.Clear();
			this.localAreaNetworks.Clear();
			this.internetProviders.Clear();
			this.computers.Clear();
			this.globalWorldState.Value = default;
			this.protectedWorldState.Value = default;
		}

		public void ChangePlayerLifepath(LifepathAsset asset)
		{
			ProtectedWorldState protectedState = this.protectedWorldState.Value;

			if (!string.IsNullOrWhiteSpace(protectedState.LifepathId))
				throw new InvalidOperationException("Changing the player's lifepath isn't possible, because a lifepath has already been chosen in this world. Updating the lifepath would disturb the space-time continuum.");

			protectedState.LifepathId = asset.Name;

			this.protectedWorldState.Value = protectedState;
		}
	}
}