#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Social;

namespace SociallyDistant.GameplaySystems.Social
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

		/// <inheritdoc />
		public bool HasMember(IProfile profile)
		{
			return memberManager.IsProfileInGroup(this.Id, profile.ProfileId, MemberGroupType.Guild);
		}

		/// <inheritdoc />
		public IChatChannel GetNarrativeChannel(string channelId)
		{
			return channelManager.GetNarrativeChannel(this.Id, channelId);
		}

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