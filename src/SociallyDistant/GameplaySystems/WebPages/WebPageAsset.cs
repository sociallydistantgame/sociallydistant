#nullable enable

using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant.GameplaySystems.WebPages
{
	public class WebPageAsset : IGameContent
	{

		private string hostname = string.Empty;


		private WebSite webSitePrefab = null!;

		public string HostName => hostname;
	}
}