#nullable enable
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Social;

namespace SociallyDistant.GameplaySystems.Social
{
	public class DirectMessageChannel : IDirectConversation
	{
		private readonly ChatMemberManager memberManager;
		private readonly IChatChannel dmChannel;
		private readonly IProfile viewingUser;

		/// <inheritdoc />
		public IEnumerable<IChatMember> Members => memberManager.GetMembersInGroup(Id, MemberGroupType.GroupDirectMessage);

		/// <inheritdoc />
		public string? NarrativeId => dmChannel.NarrativeId;

		/// <inheritdoc />
		public ObjectId Id => dmChannel.Id;

		/// <inheritdoc />
		public string Name
		{
			get
			{
				IChatMember[] nonViewingMembers = Members.Where(x => x.Profile != viewingUser).ToArray();
				if (nonViewingMembers.Length == 1)
					return nonViewingMembers.First().Profile.ChatName;

				return dmChannel.Name;
			}
		}

		/// <inheritdoc />
		public string Description
		{
			get
			{
				IChatMember[] nonViewingMembers = Members.Where(x => x.Profile != viewingUser).ToArray();
				if (nonViewingMembers.Length == 1)
					return $"@{nonViewingMembers.First().Profile.ChatUsername}";

				return $"{nonViewingMembers.Length + 1} members";
			}
		}

		/// <inheritdoc />
		public IEnumerable<IProfile> TypingUsers => dmChannel.TypingUsers;

		/// <inheritdoc />
		public IObservable<IUserMessage> SendObservable => dmChannel.SendObservable;

		/// <inheritdoc />
		public IObservable<IUserMessage> EditObservable => dmChannel.EditObservable;

		/// <inheritdoc />
		public IObservable<IUserMessage> DeleteObservable => dmChannel.DeleteObservable;

		/// <inheritdoc />
		public MessageChannelType ChannelType => dmChannel.ChannelType;

		/// <inheritdoc />
		public ObjectId? GuildId => null;

		/// <inheritdoc />
		public IEnumerable<IUserMessage> Messages => dmChannel.Messages;

		/// <inheritdoc />
		public IDisposable ObserveTypingUsers(Action<IEnumerable<IProfile>> callback)
		{
			return dmChannel.ObserveTypingUsers(callback);
		}

		/// <inheritdoc />
		public ChannelIconData GetIcon()
		{
			IEnumerable<IChatMember> otherMembers = this.Members.Where(x => x.Profile.ProfileId != viewingUser.ProfileId).ToArray();

			if (otherMembers.Count() != 1)
			{
				return new ChannelIconData
				{
					UseUnicodeIcon = true,
					UnicodeIcon = MaterialIcons.Group
				};
			}

			return new ChannelIconData
			{
				UseUnicodeIcon = false,
				UserAvatar = otherMembers.First().Profile.Picture,
				AvatarColor = Color.Yellow
			};
		}

		public DirectMessageChannel(ChatMemberManager memberManager, IChatChannel dmChannel, IProfile viewingUser)
		{
			this.memberManager = memberManager;
			this.dmChannel = dmChannel;
			this.viewingUser = viewingUser;
		}
	}
}