#nullable enable

using System.Collections.Generic;
using Core;
using Core.WorldData.Data;
using Player;
using Social;
using UnityEngine;
using UnityExtensions;

namespace GameplaySystems.Social
{
	public class SocialService :
		MonoBehaviour,
		ISocialService
	{
		[Header("Dependencies")]
		[SerializeField]
		private WorldManagerHolder worldManager = null!;
        
		[SerializeField]
		private PlayerInstanceHolder playerInstance = null!;

		private Dictionary<ObjectId, Profile> profiles = new Dictionary<ObjectId, Profile>();
		private ObjectId playerProfileId;
		private EmptyProfile unloadedPlayerProfile = new EmptyProfile();
		private ChatMemberManager memberManager;
		private GuildList masterGuildList;
		private ChatChannelManager channelManager;
		private MessageManager messageManager;

		/// <inheritdoc />
		public IProfile PlayerProfile
		{
			get
			{
				if (!profiles.TryGetValue(playerProfileId, out Profile profile))
					return unloadedPlayerProfile;

				return profile;
			}
		}

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SocialService));
		}

		private void Start()
		{
			if (worldManager.Value == null)
				return;

			messageManager = new MessageManager(worldManager.Value, this);
			channelManager = new ChatChannelManager(this.worldManager.Value, messageManager);
			memberManager = new ChatMemberManager(this, worldManager.Value);
			masterGuildList = new GuildList(channelManager, memberManager, worldManager.Value);
			worldManager.Value.Callbacks.AddModifyCallback<WorldPlayerData>(OnPlayerDataUpdated);
			
			worldManager.Value.Callbacks.AddCreateCallback<WorldProfileData>(OnProfileCreated);
			worldManager.Value.Callbacks.AddModifyCallback<WorldProfileData>(OnProfileModified);
			worldManager.Value.Callbacks.AddDeleteCallback<WorldProfileData>(OnProfileDeleted);
		}

		private void OnDestroy()
		{
			masterGuildList.Dispose();
			masterGuildList = null;
		}

		private void OnPlayerDataUpdated(WorldPlayerData subjectprevious, WorldPlayerData subjectnew)
		{
			playerProfileId = subjectnew.PlayerProfile;
		}

		private void OnProfileDeleted(WorldProfileData subject)
		{
			if (!profiles.ContainsKey(subject.InstanceId))
				return;

			profiles.Remove(subject.InstanceId);
			if (subject.InstanceId == playerProfileId)
				playerProfileId = default;
		}

		private void OnProfileModified(WorldProfileData subjectprevious, WorldProfileData subjectnew)
		{
			if (!profiles.TryGetValue(subjectnew.InstanceId, out Profile profile))
				return;
			
			profile.SetData(subjectnew);
		}

		private void OnProfileCreated(WorldProfileData subject)
		{
			if (!profiles.TryGetValue(subject.InstanceId, out Profile profile))
			{
				profile = new Profile(this);
				profiles.Add(subject.InstanceId, profile);
			}
			
			profile.SetData(subject);
		}

		/// <inheritdoc />
		public IEnumerable<IProfile> GetFriends(IProfile user)
		{
			if (worldManager.Value == null)
				yield break;

			foreach (WorldRelationshipData relationship in worldManager.Value.World.Relationships)
			{
				if (relationship.Source != user.ProfileId)
					continue;

				if (relationship.Type != RelationshipType.Friend)
					continue;

				if (!profiles.TryGetValue(relationship.Target, out Profile target))
					continue;

				yield return target;
			}
		}

		/// <inheritdoc />
		public IEnumerable<IProfile> GetFollowers(IProfile user)
		{
			if (worldManager.Value == null)
				yield break;

			foreach (WorldRelationshipData relationship in worldManager.Value.World.Relationships)
			{
				if (relationship.Target != user.ProfileId)
					continue;

				if (relationship.Type != RelationshipType.Follow)
					continue;

				if (!profiles.TryGetValue(relationship.Source, out Profile target))
					continue;

				yield return target;
			}
		}

		/// <inheritdoc />
		public IEnumerable<IProfile> GetFollowing(IProfile user)
		{
			if (worldManager.Value == null)
				yield break;

			foreach (WorldRelationshipData relationship in worldManager.Value.World.Relationships)
			{
				if (relationship.Source != user.ProfileId)
					continue;

				if (relationship.Type != RelationshipType.Follow)
					continue;

				if (!profiles.TryGetValue(relationship.Target, out Profile target))
					continue;

				yield return target;
			}
		}

		/// <inheritdoc />
		public IEnumerable<IProfile> GetBlockedProfiles(IProfile user)
		{
			if (worldManager.Value == null)
				yield break;

			foreach (WorldRelationshipData relationship in worldManager.Value.World.Relationships)
			{
				if (relationship.Source != user.ProfileId)
					continue;

				if (relationship.Type != RelationshipType.Blocked)
					continue;

				if (!profiles.TryGetValue(relationship.Target, out Profile target))
					continue;

				yield return target;
			}
		}

		/// <inheritdoc />
		public IGuildList GetGuilds()
		{
			return masterGuildList;
		}

		/// <inheritdoc />
		public IEnumerable<IDirectConversation> GetDirectConversations(IProfile user)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public IEnumerable<IUserMessage> GetSocialPosts(IProfile profile)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public IEnumerable<IUserMessage> GetTimeline(IProfile profile)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public IProfile GetProfileById(ObjectId id)
		{
			return profiles[id];
		}
	}
}