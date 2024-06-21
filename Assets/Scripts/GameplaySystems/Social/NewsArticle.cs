#nullable enable
using System;
using System.Linq;
using ContentManagement;
using Core.WorldData.Data;
using DevTools;
using Social;

namespace GameplaySystems.Social
{
	public sealed class NewsArticle : INewsArticle
	{
		private readonly ISocialService socialService;
		private readonly IProfile emptyProfile = new EmptyProfile();
		private IArticleAsset? articleAsset;

		/// <inheritdoc />
		public string? HostName => articleAsset?.HostName;

		/// <inheritdoc />
		public IProfile Author => articleAsset != null ? socialService.GetNarrativeProfile(articleAsset.NarrativeAuthorId) : emptyProfile;

		/// <inheritdoc />
		public string Headline => articleAsset?.Title ?? "Unknown article";

		/// <inheritdoc />
		public DateTime Date { get; private set; }

		internal NewsArticle(ISocialService socialService)
		{
			this.socialService = socialService;
		}
		
		/// <inheritdoc />
		public DocumentElement[] GetBody()
		{
			if (articleAsset == null)
				return Array.Empty<DocumentElement>();

			return articleAsset.Body;
		}

		internal void Update(WorldNewsData data, IContentManager contentManager)
		{
			Date = data.Date;

			articleAsset = contentManager.GetContentOfType<IArticleAsset>().FirstOrDefault(x => x.NarrativeId == data.NarrativeId);
		}
	}
}