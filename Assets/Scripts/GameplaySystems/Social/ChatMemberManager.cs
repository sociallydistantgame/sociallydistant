#nullable enable
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.WorldData.Data;
using Social;

namespace GameplaySystems.Social
{
	public class ChatMemberManager
	{
		private readonly SocialService socialService;
		private readonly WorldManager world;
		private readonly Dictionary<ObjectId, ChatMember> members = new Dictionary<ObjectId, ChatMember>();

		public ChatMemberManager(SocialService service, WorldManager world)
		{
			this.socialService = service;
			this.world = world;

			this.world.Callbacks.AddCreateCallback<WorldMemberData>(OnMemberCreate);
			this.world.Callbacks.AddDeleteCallback<WorldMemberData>(OnMemberDelete);
			
		}

		private void OnMemberDelete(WorldMemberData subject)
		{
			if (!members.TryGetValue(subject.InstanceId, out ChatMember member))
				return;

			members.Remove(subject.InstanceId);
		}
		
		private void OnMemberCreate(WorldMemberData subject)
		{
			if (!members.TryGetValue(subject.InstanceId, out ChatMember member))
			{
				member = new ChatMember(socialService);
				members.Add(subject.InstanceId, member);
			}

			member.SetData(subject);
		}

		public IEnumerable<IChatMember> GetMembersInGroup(ObjectId groupId, MemberGroupType groupType)
		{
			return members.Values.Where(x => x.GroupType == groupType && x.GroupId == groupId);
		}
	}
}