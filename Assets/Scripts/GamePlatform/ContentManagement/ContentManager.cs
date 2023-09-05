#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentManagement;

namespace GamePlatform.ContentManagement
{
	public class ContentManager : IContentManager
	{
		private readonly List<IGameContent> allContent = new List<IGameContent>();
		private readonly List<IGameContentSource> contentSources = new List<IGameContentSource>();

		public void AddContentSource(IGameContentSource source)
		{
			this.contentSources.Add(source);
		}

		public void RemoveContentSource(IGameContentSource source)
		{
			this.contentSources.Remove(source);
		}

		public T AddContentSource<T>()
			where T : IGameContentSource, new()
		{
			var source = new T();
			AddContentSource(source);
			return source;
		}

		public IEnumerable<T> GetContentOfType<T>()
			=> allContent.OfType<T>();

		public async Task RefreshContentDatabaseAsync()
		{
			var builder = new ContentCollectionBuilder(this.allContent);
			foreach (IGameContentSource source in contentSources)
			{
				await source.LoadAllContent(builder);
			}
		}
	}
}