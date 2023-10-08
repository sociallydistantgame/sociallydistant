#nullable enable

using System.Collections.Generic;
using System.Linq;
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
				if (relationship.Type != RelationshipType.Friend)
					continue;

				if (relationship.Source != user.ProfileId && relationship.Target != user.ProfileId)
					continue;

				ObjectId friendId = relationship.Source == user.ProfileId ? relationship.Target : relationship.Source;
                
				if (!profiles.TryGetValue(friendId, out Profile target))
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
			if (worldManager.Value == null)
				yield break;
			
			// Find all of the user's friends.
			foreach (IProfile? friend in GetFriends(user))
			{
				// Find a DM channel where:
				// - there are two members
				// - one member is the user
				// - the other member is the friend
				// If we can't, we must create the channel.

				var foundChannel = false;
				IEnumerable<IChatChannel> channels = channelManager.GetDirectMessageChannels();
				foreach (IChatChannel channel in channels)
				{
					WorldMemberData[] members = worldManager.Value.World.Members.Where(x => x.GroupId == channel.Id && x.GroupType == MemberGroupType.GroupDirectMessage).ToArray();
					if (members.Length != 2)
						continue;

					if (!members.All(x => x.ProfileId == user.ProfileId || x.ProfileId == friend.ProfileId))
						continue;

					foundChannel = true;
				}

				if (!foundChannel)
				{
					var newChannel = new WorldChannelData
					{
						InstanceId = worldManager.Value.GetNextObjectId(),
						ChannelType = MessageChannelType.DirectMessage,
						Name = $"__DMChannel__",
						Description = "This is a DM channel"
					};
					
					worldManager.Value.World.Channels.Add(newChannel);

					worldManager.Value.World.Members.Add(new WorldMemberData()
					{
						InstanceId = worldManager.Value.GetNextObjectId(),
						GroupId = newChannel.InstanceId,
						GroupType = MemberGroupType.GroupDirectMessage,
						ProfileId = user.ProfileId
					});
					
					worldManager.Value.World.Members.Add(new WorldMemberData()
					{
						InstanceId = worldManager.Value.GetNextObjectId(),
						GroupId = newChannel.InstanceId,
						GroupType = MemberGroupType.GroupDirectMessage,
						ProfileId = friend.ProfileId
					});
				}
			}
			
			// Now, find all world member data for DM channels where the member is the user we're searching for.
			foreach (WorldMemberData memberData in worldManager.Value.World.Members)
			{
				if (memberData.GroupType != MemberGroupType.GroupDirectMessage)
					continue;

				if (memberData.ProfileId != user.ProfileId)
					continue;

				// Find the channel.
				IChatChannel? channel = channelManager.FindChannelById(memberData.GroupId, MessageChannelType.DirectMessage);

				if (channel != null)
					yield return new DirectMessageChannel(memberManager, channel, user);
			}
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