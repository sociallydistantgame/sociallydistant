#nullable enable
namespace SociallyDistant.Core.Core
{
	public interface ICachedScript
	{
		Task RebuildScriptTree();
	}
}