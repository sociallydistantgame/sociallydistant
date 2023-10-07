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

		/// <inheritdoc />
		public IEnumerable<IChatMember> Members => memberManager.GetMembersInGroup(this.Id, MemberGroupType.Guild);

		/// <inheritdoc />
		public ObjectId Id { get; private set; }

		/// <inheritdoc />
		public string Name { get; private set; }

		/// <inheritdoc />
		public IEnumerable<IChatChannel> Channels { get; }

		internal Guild(ChatMemberManager memberManager)
		{
			this.memberManager = memberManager;
		}
		
		internal void SetData(WorldGuildData data)
		{
			Id = data.InstanceId;
			Name = data.Name;
		}
	}
}