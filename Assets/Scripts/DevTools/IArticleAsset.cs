#nullable enable
using System.Threading.Tasks;
using Core.WorldData.Data;
using UnityEngine;

namespace DevTools
{
	public interface IArticleAsset
	{
		string NarrativeId { get; }
		string HostName { get; }
		string Title { get; }
		string NarrativeAuthorId { get; }
		string Excerpt { get; }

		DocumentElement[] Body { get; }
		ArticleFlags Flags { get; }
		
		Task<Texture2D?> GetFeaturedImage();
	}
}