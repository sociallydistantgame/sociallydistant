#nullable enable

using System.Diagnostics;
using System.Net.Mime;
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.GamePlatform.ContentManagement
{
	public class ContentManager : IContentManager
	{
		private readonly IGameContext game;
		private readonly List<IContentGenerator> generators = new();
		private readonly List<IGameContent> allContent = new List<IGameContent>();
		private readonly List<IGameContentSource> contentSources = new List<IGameContentSource>();
		private readonly ContentPipeline contentPipeline;

		private bool hasLoadedSystemBundles;
		
		public ContentManager(IGameContext game, ContentPipeline pipeline)
		{
			this.game = game;
			this.contentPipeline = pipeline;
		}
		
		/// <inheritdoc />
		public void AddContentGenerator(IContentGenerator generator)
		{
			generators.Add(generator);
		}

		public void AddContentSource(IGameContentSource source)
		{
			this.contentSources.Add(source);
		}

		/// <inheritdoc />
		public void RemoveContentGenerator(IContentGenerator generator)
		{
			generators.Remove(generator);
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
			
			// Run content generators. Make sure we offload this to another thread
			await Task.Run(() =>
			{
				foreach (IContentGenerator generator in generators)
				{
					foreach (IGameContent content in generator.CreateContent())
					{
						builder.AddContent(content);
					}
				}
			});
			
			foreach (IGameContentSource source in contentSources)
			{
				// load system assets
					var loader = new XnaContentFinder(contentPipeline);
					await source.LoadAllContent(builder, loader);
			}

			await this.game.ScriptSystem.RunHookAsync(CommonScriptHooks.AfterContentReload);
		}
	}
}