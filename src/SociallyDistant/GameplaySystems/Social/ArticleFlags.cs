#nullable enable
namespace SociallyDistant.GameplaySystems.Social
{
	[Flags]
	public enum ArticleFlags : byte
	{
		None = 0,
		Scripted = 1,
		Old = Scripted << 1,
		Hidden = Old << 1,
	}
}