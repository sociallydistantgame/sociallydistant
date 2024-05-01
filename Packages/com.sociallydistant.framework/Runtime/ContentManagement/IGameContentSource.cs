#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentManagement
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