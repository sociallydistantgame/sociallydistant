#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentManagement
{
	public interface IContentManager
	{
		void AddContentGenerator(IContentGenerator generator);
		void AddContentSource(IGameContentSource source);

		void RemoveContentGenerator(IContentGenerator generator);
		void RemoveContentSource(IGameContentSource source);

		T AddContentSource<T>() where T : IGameContentSource, new();

		IEnumerable<T> GetContentOfType<T>();

		Task RefreshContentDatabaseAsync();
	}
}