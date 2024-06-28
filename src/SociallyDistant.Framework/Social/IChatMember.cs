using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.Core.Social
{
	public interface IChatMember
	{
		ObjectId GroupId { get; }
		MemberGroupType GroupType { get; }
		IProfile Profile { get; }
	}
}