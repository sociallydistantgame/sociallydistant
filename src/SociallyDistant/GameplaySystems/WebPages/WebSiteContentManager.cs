#nullable enable

using System.Collections.Immutable;
using System.Reflection;
using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant.GameplaySystems.WebPages
{
	public class WebSiteContentManager : IContentGenerator
	{
		public IEnumerable<IGameContent> CreateContent()
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (!type.IsAssignableTo(typeof(WebSite)))
						continue;

					if (type.GetConstructor(Type.EmptyTypes) == null)
						continue;

					var attribute = type.GetCustomAttributes(false).OfType<WebSiteAttribute>().FirstOrDefault();
					if (attribute == null)
						continue;

					yield return new WebPageAsset(type, attribute.HostName);
				}
			}
		}
	}
}