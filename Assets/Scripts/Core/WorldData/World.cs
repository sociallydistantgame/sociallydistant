#nullable enable

using System;
using Architecture;
using Core.DataManagement;
using Core.Serialization;
using Core.Systems;
using Core.WorldData.Data;
using GameplaySystems.Networld;

namespace Core.WorldData
{
	public class World : IWorld
	{
		private readonly WorldFlagCollection worldFlags = new WorldFlagCollection();
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
		private readonly WorldDataTable<WorldProfileData> profiles;
		private readonly WorldDataTable<WorldPostData> posts;
		private readonly WorldDataTable<WorldMessageData> messages;
		private readonly WorldDataTable<WorldChannelData> channels;
		private readonly WorldDataTable<WorldGuildData> guilds;
		private readonly WorldDataTable<WorldMemberData> members;
		private readonly WorldDataTable<WorldRelationshipData> relationships;
		
        /// <inheritdoc />
        public IWorldDataObject<GlobalWorldData> GlobalWorldState => globalWorldState;

        /// <inheritdoc />
        public IWorldFlagCollection WorldFlags => worldFlags;
        
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

        /// <inheritdoc />
        public IWorldTable<WorldProfileData> Profiles => profiles;

        /// <inheritdoc />
        public IWorldTable<WorldPostData> Posts => posts;

        /// <inheritdoc />
        public IWorldTable<WorldMessageData> Messages => messages;

        /// <inheritdoc />
        public IWorldTable<WorldChannelData> Channels => channels;

        /// <inheritdoc />
        public IWorldTable<WorldGuildData> Guilds => guilds;

        /// <inheritdoc />
        public IWorldTable<WorldMemberData> Members => members;

        /// <inheritdoc />
        public IWorldTable<WorldRelationshipData> Relationships => relationships;
        
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
        	profiles = new WorldDataTable<WorldProfileData>(instanceIdGenerator, eventDispatcher);
        	posts = new WorldDataTable<WorldPostData>(instanceIdGenerator, eventDispatcher);
        	messages = new WorldDataTable<WorldMessageData>(instanceIdGenerator, eventDispatcher);
        	channels = new WorldDataTable<WorldChannelData>(instanceIdGenerator, eventDispatcher);
        	guilds = new WorldDataTable<WorldGuildData>(instanceIdGenerator, eventDispatcher);
			members = new WorldDataTable<WorldMemberData>(instanceIdGenerator, eventDispatcher);
			relationships = new WorldDataTable<WorldRelationshipData>(instanceIdGenerator, eventDispatcher);
		}

		public void Serialize(IWorldSerializer serializer)
		{
			worldFlags.Serialize(serializer);
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
			
			profiles.Serialize(serializer, WorldRevision.ChatAndSocialMedia);
			posts.Serialize(serializer, WorldRevision.ChatAndSocialMedia);
			messages.Serialize(serializer, WorldRevision.ChatAndSocialMedia);
			channels.Serialize(serializer, WorldRevision.ChatAndSocialMedia);
			guilds.Serialize(serializer, WorldRevision.ChatAndSocialMedia);
			members.Serialize(serializer, WorldRevision.ChatAndSocialMedia);
			relationships.Serialize(serializer, WorldRevision.ChatAndSocialMedia);
		}

		public void Wipe()
		{
			// You must wipe the world in reverse order of how you would create or serialize it.
			// This ensures proper handling of deleting objects that depend on other objects.
			relationships.Clear();
			members.Clear();
			guilds.Clear();
			channels.Clear();
			messages.Clear();
			posts.Clear();
			profiles.Clear();
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
			this.worldFlags.Clear();
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