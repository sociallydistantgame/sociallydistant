#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContentManagement;
using Core.Scripting;
using Cysharp.Threading.Tasks;
using Modules;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GamePlatform.ContentManagement
{
	public class ContentManager : IContentManager
	{
		private static readonly string systemBundlesPath = Path.Combine(Application.streamingAssetsPath, "SystemAssets");

		private readonly IGameContext game;
		private readonly List<IContentGenerator> generators = new();
		private readonly List<IGameContent> allContent = new List<IGameContent>();
		private readonly List<IGameContentSource> contentSources = new List<IGameContentSource>();
		private readonly List<AssetBundle> loadedSystemBundles = new();

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
				foreach (AssetBundle systemBundle in loadedSystemBundles)
				{
					var loader = new AssetBundledContentFinder(systemBundle);
					await source.LoadAllContent(builder, loader);
				}
			}

			await this.game.ScriptSystem.RunHookAsync(CommonScriptHooks.AfterContentReload);
		}

		private async Task LoadSystemBundlesAsync()
		{
			if (hasLoadedSystemBundles)
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

			hasLoadedSystemBundles = true;
		}
		
#if UNITY_EDITOR
		[InitializeOnEnterPlayMode]
		public static void EditorPreFlight()
		{
			Debug.Log("About to enter Play Mode. Doing some pre-flight stuff...");
			BuildSystemAssetBundles();
			Debug.Log("Pre-flight checks completed - let's get this fucking party started.");
		}
		
		public static void BuildSystemAssetBundles()
		{
			if (!Directory.Exists(systemBundlesPath))
			{
				Directory.CreateDirectory(systemBundlesPath);
				Debug.Log("Created system bundles directory successfully.");
			}

			Debug.Log("Doing an asset bundle rebuild. If rebuild is necessary, this will take a bit.");
			BuildPipeline.BuildAssetBundles(systemBundlesPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
			Debug.Log("Asset bundle shit's done.");
		}
#endif
	}

	public sealed class AssetBundledContentFinder : IContentFinder
	{
		private readonly AssetBundle bundle;

		public AssetBundledContentFinder(AssetBundle bundle)
		{
			this.bundle = bundle;
		}

		/// <inheritdoc />
		public async Task<T[]> FindContentOfType<T>()
		{
			AssetBundleRequest? request = bundle.LoadAllAssetsAsync<T>();
			if (request == null)
				return Array.Empty<T>();

			try
			{
				await request;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				return Array.Empty<T>();
			}

			return request.allAssets.OfType<T>().ToArray();
		}
	}
}