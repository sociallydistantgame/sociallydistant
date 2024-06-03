#nullable enable

using System;
using System.Linq;
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
		private readonly DataEventDispatcher eventDispatcher;
		private readonly WorldFlagCollection worldFlags = new WorldFlagCollection();
		private readonly WorldDataObject<ProtectedWorldState> protectedWorldState;
		private readonly WorldDataObject<GlobalWorldData> globalWorldState;
		private readonly WorldDataObject<WorldPlayerData> playerData;
		private readonly NarrativeObjectTable<WorldComputerData> computers;
		private readonly NarrativeObjectTable<WorldInternetServiceProviderData> internetProviders;
		private readonly NarrativeObjectTable<WorldLocalNetworkData> localAreaNetworks;
		private readonly WorldDataTable<WorldNetworkConnection> networkConnections;
		private readonly WorldDataTable<WorldPortForwardingRule> portForwardingRules;
		private readonly WorldDataTable<WorldCraftedExploitData> craftedExploits;
		private readonly WorldDataTable<WorldHackableData> hackables;
		private readonly NarrativeObjectTable<WorldProfileData> profiles;
		private readonly NarrativeObjectTable<WorldPostData> posts;
		private readonly WorldDataTable<WorldMessageData> messages;
		private readonly NarrativeObjectTable<WorldChannelData> channels;
		private readonly NarrativeObjectTable<WorldGuildData> guilds;
		private readonly WorldDataTable<WorldMemberData> members;
		private readonly WorldDataTable<WorldRelationshipData> relationships;
		private readonly WorldDataTable<WorldDomainNameData> domains;
		private readonly NarrativeObjectTable<WorldMailData> emails;
		private readonly WorldDataTable<WorldWitnessedObjectData> witnessedObjects;
		private readonly WorldDataTable<WorldNotificationData> notifications;
		

		internal IWorldDataObject<ProtectedWorldState> ProtectedWorldData => this.protectedWorldState;

		/// <inheritdoc />
		public string GameVersion => ProtectedWorldData.Value.GameVersion;

		/// <inheritdoc />
        public IWorldDataObject<GlobalWorldData> GlobalWorldState => globalWorldState;

		/// <inheritdoc />
		public IWorldTable<WorldNotificationData> Notifications => notifications;

		/// <inheritdoc />
        public IWorldFlagCollection WorldFlags => worldFlags;

        /// <inheritdoc />
        public ulong PlayerExperience => this.ProtectedWorldData.Value.Experience;

        /// <inheritdoc />
        public string CurrentMissionId => this.ProtectedWorldData.Value.CurrentMissionId;

        /// <inheritdoc />
        public string NarrativeLifePath => this.ProtectedWorldData.Value.LifepathId;

        /// <inheritdoc />
        public bool IsMissionCompleted(string missionId)
        {
	        return this.ProtectedWorldData.Value.CompletedMissions?.Contains(missionId) == true;
        }

        /// <inheritdoc />
        public bool IsInteractionCompleted(string interactionId)
        {
	        return this.protectedWorldState.Value.CompletedInteractions.Contains(interactionId);
        }

        /// <inheritdoc />
        public bool WasMissionFailed(string missionId)
        {
	        return this.ProtectedWorldData.Value.FailedMissions.Contains(missionId);
        }

        /// <inheritdoc />
        public IWorldDataObject<WorldPlayerData> PlayerData => playerData;
		
        /// <inheritdoc />
        public INarrativeObjectTable<WorldComputerData> Computers => computers;
		
        /// <inheritdoc />
        public INarrativeObjectTable<WorldInternetServiceProviderData> InternetProviders => internetProviders;
		
        /// <inheritdoc />
        public INarrativeObjectTable<WorldLocalNetworkData> LocalAreaNetworks => localAreaNetworks;
		
        /// <inheritdoc />
        public IWorldTable<WorldNetworkConnection> NetworkConnections => networkConnections;
		
        /// <inheritdoc />
        public IWorldTable<WorldPortForwardingRule> PortForwardingRules => portForwardingRules;
		
        /// <inheritdoc />
        public IWorldTable<WorldCraftedExploitData> CraftedExploits => craftedExploits;
		
        /// <inheritdoc />
        public IWorldTable<WorldHackableData> Hackables => hackables;

        /// <inheritdoc />
        public INarrativeObjectTable<WorldProfileData> Profiles => profiles;

        /// <inheritdoc />
        public INarrativeObjectTable<WorldPostData> Posts => posts;

        /// <inheritdoc />
        public IWorldTable<WorldWitnessedObjectData> WitnessedObjects => witnessedObjects;

        /// <inheritdoc />
        public IWorldTable<WorldMessageData> Messages => messages;

        /// <inheritdoc />
        public INarrativeObjectTable<WorldChannelData> Channels => channels;

        /// <inheritdoc />
        public INarrativeObjectTable<WorldGuildData> Guilds => guilds;

        /// <inheritdoc />
        public IWorldTable<WorldMemberData> Members => members;
        
        /// <inheritdoc />
        public IWorldTable<WorldDomainNameData> Domains => domains;

        /// <inheritdoc />
        public IWorldTable<WorldRelationshipData> Relationships => relationships;
        public INarrativeObjectTable<WorldMailData> Emails => emails;
        
        public World(UniqueIntGenerator instanceIdGenerator, DataEventDispatcher eventDispatcher)
        {
	        this.eventDispatcher = eventDispatcher;
			protectedWorldState = new WorldDataObject<ProtectedWorldState>(eventDispatcher);
			globalWorldState = new WorldDataObject<GlobalWorldData>(eventDispatcher);
			playerData = new WorldDataObject<WorldPlayerData>(eventDispatcher);
			computers = new NarrativeObjectTable<WorldComputerData>(instanceIdGenerator, eventDispatcher);
			internetProviders = new NarrativeObjectTable<WorldInternetServiceProviderData>(instanceIdGenerator, eventDispatcher, false);
			localAreaNetworks = new NarrativeObjectTable<WorldLocalNetworkData>(instanceIdGenerator, eventDispatcher);
			networkConnections = new WorldDataTable<WorldNetworkConnection>(instanceIdGenerator, eventDispatcher);
			portForwardingRules = new WorldDataTable<WorldPortForwardingRule>(instanceIdGenerator, eventDispatcher);
			craftedExploits = new WorldDataTable<WorldCraftedExploitData>(instanceIdGenerator, eventDispatcher);
			hackables = new WorldDataTable<WorldHackableData>(instanceIdGenerator, eventDispatcher);
        	profiles = new NarrativeObjectTable<WorldProfileData>(instanceIdGenerator, eventDispatcher);
        	posts = new NarrativeObjectTable<WorldPostData>(instanceIdGenerator, eventDispatcher);
        	messages = new WorldDataTable<WorldMessageData>(instanceIdGenerator, eventDispatcher);
        	channels = new NarrativeObjectTable<WorldChannelData>(instanceIdGenerator, eventDispatcher);
        	guilds = new NarrativeObjectTable<WorldGuildData>(instanceIdGenerator, eventDispatcher);
			members = new WorldDataTable<WorldMemberData>(instanceIdGenerator, eventDispatcher);
			relationships = new WorldDataTable<WorldRelationshipData>(instanceIdGenerator, eventDispatcher);
			domains = new WorldDataTable<WorldDomainNameData>(instanceIdGenerator, eventDispatcher);
			emails = new NarrativeObjectTable<WorldMailData>(instanceIdGenerator, eventDispatcher);
			witnessedObjects = new WorldDataTable<WorldWitnessedObjectData>(instanceIdGenerator, eventDispatcher);
			notifications = new WorldDataTable<WorldNotificationData>(instanceIdGenerator, eventDispatcher);
		}

		public void Serialize(IWorldSerializer serializer)
		{
			eventDispatcher.PauseEvents = true;
			
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
			domains.Serialize(serializer, WorldRevision.DomainNames);
			emails.Serialize(serializer, WorldRevision.Email);
			witnessedObjects.Serialize(serializer, WorldRevision.MissionFailures);
			notifications.Serialize(serializer, WorldRevision.Notifications);
			
			
			eventDispatcher.PauseEvents = false;
		}

		public void Wipe()
		{
			// You must wipe the world in reverse order of how you would create or serialize it.
			// This ensures proper handling of deleting objects that depend on other objects.
			notifications.Clear();
			witnessedObjects.Clear();
			emails.Clear();
			domains.Clear();
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