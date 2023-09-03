#nullable enable
using System.Threading.Tasks;

namespace GamePlatform.ContentManagement
{
	public interface IGameContentSource
	{
		Task LoadAllContent(ContentCollectionBuilder builder);
	}
}