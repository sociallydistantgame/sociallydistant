#nullable enable

using System.Threading.Tasks;
using ContentManagement;
using UnityEngine;
using UnityEngine.UI;

namespace GameplaySystems.WebPages
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