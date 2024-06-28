#nullable enable
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.GameplaySystems.Social
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