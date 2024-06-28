namespace SociallyDistant.Core.Social
{
	public interface IChatGroup
	{
		IEnumerable<IChatMember> Members { get; }
	}
}