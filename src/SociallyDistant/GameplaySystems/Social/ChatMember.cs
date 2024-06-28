#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Social;

namespace SociallyDistant.GameplaySystems.Social
{
	public class ChatMember : IChatMember
	{
		private readonly SocialService socialService;
		private ObjectId profileId;

		/// <inheritdoc />
		public ObjectId GroupId { get; private set; }

		/// <inheritdoc />
		public MemberGroupType GroupType { get; private set; }

		/// <inheritdoc />
		public IProfile Profile => socialService.GetProfileById(profileId);

		internal ChatMember(SocialService service)
		{
			this.socialService = service;
		}

		internal void SetData(WorldMemberData data)
		{
			profileId = data.ProfileId;
			GroupId = data.GroupId;
			GroupType = data.GroupType;
		}
	}
}