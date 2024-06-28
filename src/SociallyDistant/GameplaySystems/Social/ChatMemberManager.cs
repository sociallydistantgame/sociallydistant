#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Social;

namespace SociallyDistant.GameplaySystems.Social
{
	public class ChatMemberManager
	{
		private readonly SocialService socialService;
		private readonly IWorldManager world;
		private readonly Dictionary<ObjectId, ChatMember> members = new Dictionary<ObjectId, ChatMember>();

		public ChatMemberManager(SocialService service, IWorldManager world)
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

		public bool IsProfileInGroup(ObjectId groupId, ObjectId userId, MemberGroupType type)
		{
			return GetMembersInGroup(groupId, type).Any(x => x.Profile.ProfileId == userId);
		}
	}
}