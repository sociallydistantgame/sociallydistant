#nullable enable

using AcidicGUI.Widgets;
using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant.GameplaySystems.WebPages
{
	public class WebPageAsset : IGameContent
	{
		private readonly string hostname;
		private readonly Type   type;

		public string HostName => hostname;

		internal WebPageAsset(Type webSiteType, string hostname)
		{
			this.hostname = hostname;
			this.type = webSiteType;
		}
		
		public WebSite InstantiateWebSite(ContentWidget pageArea, string path)
		{
			WebSite? result = Activator.CreateInstance(type, null) as WebSite;
			if (result == null)
				throw new InvalidOperationException($"Cannot instantiate website {hostname} because the instantiated object was invalid.");

			result.Init(this);
			result.NavigateToPath(path);
			
			pageArea.Content = result;

			return result;
		}
	}
}