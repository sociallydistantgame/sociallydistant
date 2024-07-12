#nullable enable

using Microsoft.Xna.Framework;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Social;
using SociallyDistant.GameplaySystems.Chat;
using SociallyDistant.Player;

namespace SociallyDistant.GameplaySystems.Social
{
	public class SocialService : GameComponent, 
		ISocialService
	{
		private readonly CharacterUpdateHook characterUpdateHook = new();
		private readonly ConversationManager conversationManager;
		
		private Dictionary<ObjectId, Profile> profiles = new Dictionary<ObjectId, Profile>();
		private ObjectId playerProfileId;
		private EmptyProfile unloadedPlayerProfile = new EmptyProfile();
		private ChatMemberManager memberManager;
		private GuildList masterGuildList;
		private ChatChannelManager channelManager;
		private MessageManager messageManager;
		private SocialPostManager socialPostManager;
		private IGameContext game;
		private NewsManager newsManager;

		private IWorldManager WorldManager => game.WorldManager;
		
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

		internal SocialService(SociallyDistantGame game) : base(game)
		{
			this.game = game;
			this.conversationManager = new ConversationManager(game);
		}
		
		
		public override void Initialize()
		{
			base.Initialize();

			conversationManager.Initialize();
			
			game.ScriptSystem.RegisterHookListener(CommonScriptHooks.AfterWorldStateUpdate, characterUpdateHook);
			messageManager = new MessageManager(WorldManager, this);
			channelManager = new ChatChannelManager(this, this.WorldManager, messageManager);
			memberManager = new ChatMemberManager(this, WorldManager);
			masterGuildList = new GuildList(channelManager, memberManager, WorldManager);
			socialPostManager = new SocialPostManager(this, WorldManager);

			WorldManager.Callbacks.AddModifyCallback<WorldPlayerData>(OnPlayerDataUpdated);

			WorldManager.Callbacks.AddCreateCallback<WorldProfileData>(OnProfileCreated);
			WorldManager.Callbacks.AddModifyCallback<WorldProfileData>(OnProfileModified);
			WorldManager.Callbacks.AddDeleteCallback<WorldProfileData>(OnProfileDeleted);
			newsManager = new NewsManager(this, WorldManager, game.ContentManager);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			conversationManager.Update(gameTime);
			newsManager.Update();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (!disposing)
				return;

			conversationManager.Dispose();
			
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
			foreach (WorldRelationshipData relationship in WorldManager.World.Relationships)
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
			foreach (WorldRelationshipData relationship in WorldManager.World.Relationships)
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
			foreach (WorldRelationshipData relationship in WorldManager.World.Relationships)
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
			foreach (WorldRelationshipData relationship in WorldManager.World.Relationships)
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
					WorldMemberData[] members = WorldManager.World.Members.Where(x => x.GroupId == channel.Id && x.GroupType == MemberGroupType.GroupDirectMessage).ToArray();
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
						InstanceId = WorldManager.GetNextObjectId(),
						ChannelType = MessageChannelType.DirectMessage,
						Name = $"__DMChannel__",
						Description = "This is a DM channel"
					};

					WorldManager.World.Channels.Add(newChannel);

					WorldManager.World.Members.Add(new WorldMemberData()
					{
						InstanceId = WorldManager.GetNextObjectId(),
						GroupId = newChannel.InstanceId,
						GroupType = MemberGroupType.GroupDirectMessage,
						ProfileId = user.ProfileId
					});

					WorldManager.World.Members.Add(new WorldMemberData()
					{
						InstanceId = WorldManager.GetNextObjectId(),
						GroupId = newChannel.InstanceId,
						GroupType = MemberGroupType.GroupDirectMessage,
						ProfileId = friend.ProfileId
					});
				}
			}

			// Now, find all world member data for DM channels where the member is the user we're searching for.
			foreach (WorldMemberData memberData in WorldManager.World.Members)
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

			WorldProfileData profileDAta = WorldManager!.World.Profiles.GetNarrativeObject(narrativeIdentifier);

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
				WorldManager.World.Profiles.Modify(profileDAta);

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

			IWorld world = WorldManager.World;

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
						threadData.InstanceId = WorldManager.GetNextObjectId();
						threadData.ChannelType = MessageChannelType.DirectMessage;

						world.Channels.Add(threadData);

						foreach (ObjectId profileId in profileIds)
						{
							world.Members.Add(new WorldMemberData
							{
								InstanceId = WorldManager.GetNextObjectId(),
								GroupId = threadData.InstanceId,
								ProfileId = profileId,
								GroupType = MemberGroupType.GroupDirectMessage
							});
						}
					}

					return new NarrativeThreadController(WorldManager, this.PlayerProfile, threadData.InstanceId);
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
							InstanceId = WorldManager.GetNextObjectId(),
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
								InstanceId = WorldManager.GetNextObjectId(),
								GroupId = guild.Id,
								ProfileId = npcProfile.ProfileId,
								GroupType = MemberGroupType.Guild
							});
					}

					// Now we can grab a channel to create the thread in
					// It'll also be created if it doesn't exist.
					IChatChannel channel = guild.GetNarrativeChannel(channelId);

					return new NarrativeThreadController(WorldManager, this.PlayerProfile, channel.Id);
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
	