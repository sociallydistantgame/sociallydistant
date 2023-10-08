#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.WorldData.Data;
using Social;

namespace GameplaySystems.Social
{
	public class DirectMessageChannel : IDirectConversation
	{
		private readonly ChatMemberManager memberManager;
		private readonly IChatChannel dmChannel;
		private readonly IProfile viewingUser;

		/// <inheritdoc />
		public IEnumerable<IChatMember> Members => memberManager.GetMembersInGroup(Id, MemberGroupType.GroupDirectMessage);

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

		public DirectMessageChannel(ChatMemberManager memberManager, IChatChannel dmChannel, IProfile viewingUser)
		{
			this.memberManager = memberManager;
			this.dmChannel = dmChannel;
			this.viewingUser = viewingUser;
		}
	}
}