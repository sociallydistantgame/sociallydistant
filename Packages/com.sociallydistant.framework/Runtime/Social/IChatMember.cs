using Core;
using Core.WorldData.Data;

namespace Social
{
	public interface IChatMember
	{
		ObjectId GroupId { get; }
		MemberGroupType GroupType { get; }
		IProfile Profile { get; }
	}
}