#nullable enable

using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant.GameplaySystems.WebPages
{
	public class WebSiteContentManager : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
		{
			foreach (WebPageAsset website in await finder.FindContentOfType<WebPageAsset>())
			{
				builder.AddContent(website);
			}
			
		}
	}
}