#nullable enable
namespace SociallyDistant.Core.ContentManagement
{
	public interface IGameContentSource
	{
		Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder);
	}

	public interface IContentGenerator
	{
		IEnumerable<IGameContent> CreateContent();
	}
}