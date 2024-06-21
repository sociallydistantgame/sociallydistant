#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using ContentManagement;
using Core;
using Core.WorldData.Data;
using DevTools;
using Social;
using Time = UnityEngine.Time;

namespace GameplaySystems.Social
{
	public sealed class NewsManager : INewsManager
	{
		private readonly ISocialService socialService;
		private readonly IWorldManager worldManager;
		private readonly IContentManager contentManager;
		private readonly List<NewsArticle> articles = new();
		private readonly Dictionary<ObjectId, int> articlesById = new();

		private const float UpdateIntervalSeconds = 2;
		
		private float updateTime = 0;

		public NewsManager(ISocialService socialService, IWorldManager worldManager, IContentManager contentManager)
		{
			this.socialService = socialService;
			this.worldManager = worldManager;
			this.contentManager = contentManager;

			InstallEvents();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			UninstallEvents();
		}

		private void InstallEvents()
		{
			worldManager.Callbacks.AddCreateCallback<WorldNewsData>(OnCreateNewsArticle);
			worldManager.Callbacks.AddModifyCallback<WorldNewsData>(OnModifyNewsArticle);
			worldManager.Callbacks.AddDeleteCallback<WorldNewsData>(OnDeleteNewsArticle);
		}

		private void UpdateArticle(ObjectId target, WorldNewsData subject)
		{
			if (!articlesById.TryGetValue(target, out int index))
				return;

			articles[index].Update(subject, contentManager);
		}
		
		private void OnCreateNewsArticle(WorldNewsData subject)
		{
			if (!articlesById.TryGetValue(subject.InstanceId, out int index))
			{
				index = articles.Count;
				articles.Add(new NewsArticle(this.socialService));
				articlesById.Add(subject.InstanceId, index);
			}

			UpdateArticle(subject.InstanceId, subject);
		}

		private void OnDeleteNewsArticle(WorldNewsData subject)
		{
			if (!articlesById.TryGetValue(subject.InstanceId, out int index))
				return;
			
			articles.RemoveAt(index);
			articlesById.Remove(subject.InstanceId);

			foreach (ObjectId key in articlesById.Keys.ToArray())
			{
				if (articlesById[key] >= index)
					articlesById[key]--;
			}
		}

		private void OnModifyNewsArticle(WorldNewsData subjectprevious, WorldNewsData subjectnew)
		{
			UpdateArticle(subjectprevious.InstanceId, subjectnew);
		}
		
		private void UninstallEvents()
		{
			worldManager.Callbacks.RemoveCreateCallback<WorldNewsData>(OnCreateNewsArticle);
			worldManager.Callbacks.RemoveModifyCallback<WorldNewsData>(OnModifyNewsArticle);
			worldManager.Callbacks.RemoveDeleteCallback<WorldNewsData>(OnDeleteNewsArticle);
		}

		/// <inheritdoc />
		public INewsArticle? LatestNews => articles.OrderByDescending(x => x.Date).FirstOrDefault();

		/// <inheritdoc />
		public IEnumerable<INewsArticle> AllArticles => articles;

		/// <inheritdoc />
		public IEnumerable<INewsArticle> GetArticlesForHost(string hostname)
		{
			return articles.Where(x => x.HostName == hostname)
				.OrderByDescending(x => x.Date);
		}

		internal void Update()
		{
			updateTime += Time.deltaTime;

			if (updateTime < UpdateIntervalSeconds)
				return;

			updateTime = 0;
			PostNewArticles();
		}

		private void PostNewArticles()
		{
			foreach (IArticleAsset asset in contentManager.GetContentOfType<IArticleAsset>())
			{
				if (asset.Flags.HasFlag(ArticleFlags.Scripted))
					continue;

				bool shouldPost = asset.Flags.HasFlag(ArticleFlags.Old);

				if (shouldPost)
				{
					if (worldManager.World.NewsArticles.ContainsNarrativeId(asset.NarrativeId))
						continue;

					ObjectId id = worldManager.GetNextObjectId();

					var data = new WorldNewsData()
					{
						InstanceId = id,
						NarrativeId = asset.NarrativeId,
						Date = asset.Flags.HasFlag(ArticleFlags.Old)
							? DateTime.MinValue.AddMonths(id.Id)
							: worldManager.World.GlobalWorldState.Value.Now
					};
					
					worldManager.World.NewsArticles.Add(data);
				}
				else
				{
					if (!worldManager.World.NewsArticles.ContainsNarrativeId(asset.NarrativeId))
						continue;

					WorldNewsData data = worldManager.World.NewsArticles.GetNarrativeObject(asset.NarrativeId);
					worldManager.World.NewsArticles.Remove(data);
				}
				
			}
		}
	}
}