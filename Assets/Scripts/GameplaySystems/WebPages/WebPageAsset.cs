#nullable enable

using ContentManagement;
using UnityEngine;

namespace GameplaySystems.WebPages
{
	[CreateAssetMenu(menuName = "ScriptableObject/The Internet/Web Site")]
	public class WebPageAsset : 
		ScriptableObject,
		IGameContent
	{
		[SerializeField]
		private string hostname = string.Empty;

		[SerializeField]
		private WebSite webSitePrefab = null!;

		public string HostName => hostname;
		
		public WebSite InstantiateWebSite(RectTransform destination, string path)
		{
			// Disable the prefab before instantiating it, so we can inject some stuff into it before Awake() is called.
			// Wouldn't fucking need to DO THIS SHIT if Unity just kinda....understood constructors exist. Whatever.
			this.webSitePrefab.gameObject.SetActive(false);
			
			// Instantiate
			WebSite website = Instantiate(this.webSitePrefab, destination);

			website.Init(this);
			website.gameObject.SetActive(true);
			
			website.NavigateToPath(path);
			
			this.webSitePrefab.gameObject.SetActive(true);
			
			return website;
		}
	}
}