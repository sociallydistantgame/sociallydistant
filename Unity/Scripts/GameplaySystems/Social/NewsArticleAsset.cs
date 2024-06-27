#nullable enable
using System;
using System.Threading.Tasks;
using Core.WorldData.Data;
using DevTools;
using UnityEngine;

namespace GameplaySystems.Social
{
	public sealed class NewsArticleAsset : 
		ScriptableObject,
		IArticleAsset
	{
		[SerializeField]
		private string title = string.Empty;

		[SerializeField]
		private string hostName = string.Empty;

		[SerializeField]
		private string narrativeId = string.Empty;

		[SerializeField]
		private string authorId = string.Empty;

		[SerializeField]
		private string excerpt = string.Empty;

		[SerializeField]
		private DocumentElement[] body = Array.Empty<DocumentElement>();

		[SerializeField]
		private ArticleFlags articleFlags;
		
		/// <inheritdoc />
		public string NarrativeId
		{
			get => narrativeId;
#if UNITY_EDITOR
			set => narrativeId = value;
#endif
		}

		/// <inheritdoc />
		public string HostName
		{
			get => hostName;
#if UNITY_EDITOR
			set => hostName = value;
#endif
		}

		/// <inheritdoc />
		public string Title
		{
			get => title;
#if UNITY_EDITOR
			set => title = value;
#endif
		}

		/// <inheritdoc />
		public string NarrativeAuthorId
		{
			get => authorId;
#if UNITY_EDITOR
			set => authorId = value;
#endif
		}

		/// <inheritdoc />
		public string Excerpt
		{
			get => excerpt;
#if UNITY_EDITOR
			set => excerpt = value;
#endif
		}

		/// <inheritdoc />
		public DocumentElement[] Body
		{
			get => body;
#if UNITY_EDITOR
			set => body = value;
#endif
		}

		/// <inheritdoc />
		public ArticleFlags Flags
		{
			get => articleFlags;
#if UNITY_EDITOR
			set => articleFlags = value;
#endif
		}


		/// <inheritdoc />
		public Task<Texture2D?> GetFeaturedImage()
		{
			return Task.FromResult<Texture2D?>(null);
		}
	}
}