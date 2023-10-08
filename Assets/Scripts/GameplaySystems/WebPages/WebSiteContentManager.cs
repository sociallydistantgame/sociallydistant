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
		public async Task LoadAllContent(ContentCollectionBuilder builder)
		{
			await Task.Yield();
			foreach (WebPageAsset website in Resources.LoadAll<WebPageAsset>("Websites"))
			{
				builder.AddContent(website);
				await Task.Yield();
			}
			
		}
	}
}