#nullable enable
namespace SociallyDistant.Core.ContentManagement
{
	public interface IContentFinder
	{
		Task<T[]> FindContentOfType<T>();
	}
}