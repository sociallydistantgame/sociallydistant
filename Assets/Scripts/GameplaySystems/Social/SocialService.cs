#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Scripting;
using Core.WorldData;
using Core.WorldData.Data;
using DevTools;
using GamePlatform;
using Modules;
using Player;
using Social;
using UniRx;
using UnityEngine;
using UnityEngine.Analytics;
using UnityExtensions;

namespace GameplaySystems.Social
{
	public class SocialService :
		MonoBehaviour,
		ISocialService
	{
		[SerializeField]
		private PlayerInstanceHolder playerInstance = null!;

		private readonly CharacterUpdateHook characterUpdateHook = new();

		private Dictionary<ObjectId, Profile> profiles = new Dictionary<ObjectId, Profile>();
		private ObjectId playerProfileId;
		private EmptyProfile unloadedPlayerProfile = new EmptyProfile();
		private ChatMemberManager memberManager;
		private GuildList masterGuildList;
		private ChatChannelManager channelManager;
		private MessageManager messageManager;
		private SocialPostManager socialPostManager;
		private IWorldManager worldManager = null!;
		private IGameContext game;
		private NewsManager newsManager;

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
			game = GameManager.Instance;
			worldManager = GameManager.Instance.WorldManager;

			this.AssertAllFieldsAreSerialized(typeof(SocialService));
		}

		private void Start()
		{
			game.ScriptSystem.RegisterHookListener(CommonScriptHooks.AfterWorldStateUpdate, characterUpdateHook);
			messageManager = new MessageManager(worldManager, this);
			channelManager = new ChatChannelManager(this, this.worldManager, messageManager);
			memberManager = new ChatMemberManager(this, worldManager);
			masterGuildList = new GuildList(channelManager, memberManager, worldManager);
			socialPostManager = new SocialPostManager(this, worldManager);

			worldManager.Callbacks.AddModifyCallback<WorldPlayerData>(OnPlayerDataUpdated);

			worldManager.Callbacks.AddCreateCallback<WorldProfileData>(OnProfileCreated);
			worldManager.Callbacks.AddModifyCallback<WorldProfileData>(OnProfileModified);
			worldManager.Callbacks.AddDeleteCallback<WorldProfileData>(OnProfileDeleted);
			newsManager = new NewsManager(this, worldManager, game.ContentManager);
		}

		private void OnDestroy()
		{
			game.ScriptSystem.UnregisterHookListener(CommonScriptHooks.AfterWorldStateUpdate, characterUpdateHook);
			socialPostManager.Dispose();
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
			foreach (WorldRelationshipData relationship in worldManager.World.Relationships)
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
			foreach (WorldRelationshipData relationship in worldManager.World.Relationships)
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
			foreach (WorldRelationshipData relationship in worldManager.World.Relationships)
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
			foreach (WorldRelationshipData relationship in worldManager.World.Relationships)
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
					WorldMemberData[] members = worldManager.World.Members.Where(x => x.GroupId == channel.Id && x.GroupType == MemberGroupType.GroupDirectMessage).ToArray();
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
						InstanceId = worldManager.GetNextObjectId(),
						ChannelType = MessageChannelType.DirectMessage,
						Name = $"__DMChannel__",
						Description = "This is a DM channel"
					};

					worldManager.World.Channels.Add(newChannel);

					worldManager.World.Members.Add(new WorldMemberData()
					{
						InstanceId = worldManager.GetNextObjectId(),
						GroupId = newChannel.InstanceId,
						GroupType = MemberGroupType.GroupDirectMessage,
						ProfileId = user.ProfileId
					});

					worldManager.World.Members.Add(new WorldMemberData()
					{
						InstanceId = worldManager.GetNextObjectId(),
						GroupId = newChannel.InstanceId,
						GroupType = MemberGroupType.GroupDirectMessage,
						ProfileId = friend.ProfileId
					});
				}
			}

			// Now, find all world member data for DM channels where the member is the user we're searching for.
			foreach (WorldMemberData memberData in worldManager.World.Members)
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
		public IEnumerable<IProfile> Profiles => profiles.Values;

		/// <inheritdoc />
		public IEnumerable<IUserMessage> GetSocialPosts(IProfile profile)
		{
			return socialPostManager.GetPostsByUser(profile);
		}

		/// <inheritdoc />
		public IEnumerable<IUserMessage> GetTimeline(IProfile profile)
		{
			return GetTimelineInternal(profile).OrderByDescending(x => x.Date);
		}

		private IEnumerable<IUserMessage> GetTimelineInternal(IProfile profile)
		{
			foreach (IUserMessage ownPost in GetSocialPosts(profile))
				yield return ownPost;

			foreach (IProfile follow in GetFollowing(profile))
			{
				foreach (IUserMessage post in GetSocialPosts(follow))
					yield return post;
			}
		}

		/// <inheritdoc />
		public IProfile GetProfileById(ObjectId id)
		{
			if (id.IsInvalid)
				return new EmptyProfile();

			return profiles[id];
		}

		/// <inheritdoc />
		public IProfile GetNarrativeProfile(string narrativeIdentifier)
		{
			if (narrativeIdentifier == "player")
				return PlayerProfile;

			WorldProfileData profileDAta = worldManager!.World.Profiles.GetNarrativeObject(narrativeIdentifier);

			var modify = false;
			if (string.IsNullOrWhiteSpace(profileDAta.ChatUsername))
			{
				modify = true;
				profileDAta.ChatUsername = narrativeIdentifier;
			}

			if (string.IsNullOrWhiteSpace(profileDAta.ChatName))
			{
				modify = true;
				profileDAta.ChatName = narrativeIdentifier;
			}

			if (modify)
				worldManager.World.Profiles.Modify(profileDAta);

			return profiles[profileDAta.InstanceId];
		}

		/// <inheritdoc />
		public INarrativeThread? GetNarrativeThread(NarrativeThread threadType, string guildId, string channelId, bool create, params string[] nonPlayerActors)
		{
			if (nonPlayerActors.Length < 1)
			{
				if (create)
					throw new InvalidOperationException("A narrative thread must have at least one non-player actor. Do not pass an empty array to this function!");
				else return null;
			}

			IWorld world = worldManager.World;

			IProfile[] nonPlayerProfiles = nonPlayerActors.Select(GetNarrativeProfile).ToArray();
			IProfile playerProfile = this.PlayerProfile;

			var profileIds = new ObjectId[nonPlayerProfiles.Length + 1];

			profileIds[0] = playerProfile.ProfileId;

			for (var i = 0; i < nonPlayerProfiles.Length; i++)
			{
				profileIds[i + 1] = nonPlayerProfiles[i].ProfileId;
			}

			switch (threadType)
			{
				case NarrativeThread.DirectMessage:
				{
					if (nonPlayerActors.Length > 1)
					{
						if (create)
							throw new InvalidOperationException("Direct message threads may only have one non-player actor.");
						else return null;
					}

					WorldChannelData threadData = world.Channels.FirstOrDefault(x => x.ChannelType == MessageChannelType.DirectMessage &&
					                                                                 world.Members.Where(m => m.GroupId == x.InstanceId).All(m => profileIds.Contains(m.ProfileId)));

					if (threadData.InstanceId.IsInvalid)
					{
						threadData.InstanceId = worldManager.GetNextObjectId();
						threadData.ChannelType = MessageChannelType.DirectMessage;

						world.Channels.Add(threadData);

						foreach (ObjectId profileId in profileIds)
						{
							world.Members.Add(new WorldMemberData
							{
								InstanceId = worldManager.GetNextObjectId(),
								GroupId = threadData.InstanceId,
								ProfileId = profileId,
								GroupType = MemberGroupType.GroupDirectMessage
							});
						}
					}

					return new NarrativeThreadController(worldManager, this.PlayerProfile, threadData.InstanceId);
				}
				case NarrativeThread.Channel:
				{
					if (string.IsNullOrWhiteSpace(guildId) || string.IsNullOrWhiteSpace(channelId))
					{
						if (create)
							throw new InvalidOperationException("When requesting a Guild-type narrative thread, you must specify a Narrative ID for the guild and channel");
						return null;
					}

					// Find the guild by narrative ID, it'll be created if non already exists.
					IGuild guild = this.masterGuildList.GetNarrativeGuild(guildId);

					// Invite the player into the guild if they're not in it already
					if (!guild.HasMember(playerProfile))
					{
						// We do not invite players to guilds unless a script is starting.
						if (!create)
							return null;

						world.Members.Add(new WorldMemberData()
						{
							InstanceId = worldManager.GetNextObjectId(),
							GroupId = guild.Id,
							ProfileId = playerProfile.ProfileId,
							GroupType = MemberGroupType.Guild
						});
					}

					// And do the same for the NPCs
					foreach (IProfile npcProfile in nonPlayerProfiles)
					{
						if (!guild.HasMember(npcProfile))
							world.Members.Add(new WorldMemberData()
							{
								InstanceId = worldManager.GetNextObjectId(),
								GroupId = guild.Id,
								ProfileId = npcProfile.ProfileId,
								GroupType = MemberGroupType.Guild
							});
					}

					// Now we can grab a channel to create the thread in
					// It'll also be created if it doesn't exist.
					IChatChannel channel = guild.GetNarrativeChannel(channelId);

					return new NarrativeThreadController(worldManager, this.PlayerProfile, channel.Id);
				}
				default:
				{
					if (create)
						throw new NotImplementedException();
					else return null;
				}
			}
		}

		/// <inheritdoc />
		public INewsManager News => newsManager;
	}

	public sealed class CharacterUpdateHook : IHookListener
	{
		/// <inheritdoc />
		public async Task ReceiveHookAsync(IGameContext game)
		{
			foreach (ICharacterGenerator generator in game.ContentManager.GetContentOfType<ICharacterGenerator>())
			{
				await generator.GenerateNpcs(game.WorldManager);
			}
		}
	}
}
	