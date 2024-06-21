#nullable enable
using System.Collections.Generic;
using System.Linq;
using ContentManagement;
using Core;
using Core.WorldData.Data;
using Social;

namespace GameplaySystems.Social
{
	public sealed class NewsManager : INewsManager
	{
		private readonly ISocialService socialService;
		private readonly IWorldManager worldManager;
		private readonly IContentManager contentManager;
		private readonly List<NewsArticle> articles = new();
		private readonly Dictionary<ObjectId, int> articlesById = new();

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
	}
}