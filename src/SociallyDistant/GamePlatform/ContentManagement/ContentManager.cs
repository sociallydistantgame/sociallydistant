#nullable enable

using System.Diagnostics;
using System.Net.Mime;
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Scripting;

namespace SociallyDistant.GamePlatform.ContentManagement
{
	public class ContentManager : IContentManager
	{
		private readonly IGameContext game;
		private readonly List<IContentGenerator> generators = new();
		private readonly List<IGameContent> allContent = new List<IGameContent>();
		private readonly List<IGameContentSource> contentSources = new List<IGameContentSource>();

		private bool hasLoadedSystemBundles;
		
		public ContentManager(IGameContext game)
		{
			this.game = game;
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
			await LoadSystemBundlesAsync();
			
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
				/*foreach (AssetBundle systemBundle in loadedSystemBundles)
				{
					var loader = new AssetBundledContentFinder(systemBundle);
					await source.LoadAllContent(builder, loader);
				}*/
			}

			await this.game.ScriptSystem.RunHookAsync(CommonScriptHooks.AfterContentReload);
		}

		private async Task LoadSystemBundlesAsync()
		{
			/*if (hasLoadedSystemBundles)
				return;
		
			if (!Directory.Exists(systemBundlesPath))
				return;
			
			foreach (string bundleFile in Directory.EnumerateFiles(systemBundlesPath))
			{
				string manifestPath = $"{bundleFile}.manifest";
				if (!File.Exists(manifestPath))
					continue;
				
				Debug.Log($"Loading bundle: {Path.GetFileName(bundleFile)}");

				try
				{
					AssetBundle bundle = await AssetBundle.LoadFromFileAsync(bundleFile);
					loadedSystemBundles.Add(bundle);

					Debug.Log($"Successfully loaded bundle: {bundle.name}");
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}

			hasLoadedSystemBundles = true;*/
		}
	}
}