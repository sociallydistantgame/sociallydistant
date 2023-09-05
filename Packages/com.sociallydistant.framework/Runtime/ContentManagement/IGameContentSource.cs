#nullable enable
using System.Threading.Tasks;

namespace ContentManagement
{
	public interface IGameContentSource
	{
		Task LoadAllContent(ContentCollectionBuilder builder);
	}
}