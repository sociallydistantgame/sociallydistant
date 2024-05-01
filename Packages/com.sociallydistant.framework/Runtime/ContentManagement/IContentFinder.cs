#nullable enable
using System.Threading.Tasks;

namespace ContentManagement
{
	public interface IContentFinder
	{
		Task<T[]> FindContentOfType<T>();
	}
}