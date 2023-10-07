#nullable enable
using System.Collections.Generic;
using Core;
using Core.WorldData.Data;
using Social;

namespace GameplaySystems.Social
{
	public class Guild : IGuild
	{
		private readonly ChatMemberManager memberManager;
		private readonly ChatChannelManager channelManager;

		/// <inheritdoc />
		public IEnumerable<IChatMember> Members => memberManager.GetMembersInGroup(this.Id, MemberGroupType.Guild);

		/// <inheritdoc />
		public ObjectId Id { get; private set; }

		/// <inheritdoc />
		public string Name { get; private set; }

		/// <inheritdoc />
		public IEnumerable<IChatChannel> Channels => channelManager.GetGuildChannels(this.Id);

		internal Guild(ChatMemberManager memberManager, ChatChannelManager channelManager)
		{
			this.memberManager = memberManager;
			this.channelManager = channelManager;
		}
		
		internal void SetData(WorldGuildData data)
		{
			Id = data.InstanceId;
			Name = data.Name;
		}
	}
}