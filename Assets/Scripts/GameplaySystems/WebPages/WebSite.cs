using System;
using System.Collections.Generic;
using AcidicGui.Mvc;
using UnityExtensions;

namespace GameplaySystems.WebPages
{
	public abstract class WebSite : Controller<WebPage>
	{
		private readonly Stack<WebPage> future = new Stack<WebPage>();
		
		public bool CanGoBack => ViewsListCount > 1;
		public bool CanGoForward => future.Count > 0;

		private WebPageAsset myAsset;

		public WebPageAsset WebSiteAsset => myAsset;
		
		public string Url
		{
			get
			{
				if (myAsset == null)
					return string.Empty;

				string baseUrl = myAsset.HostName;

				var pagePath = "/";

				if (this.CurrentView != null)
					pagePath = CurrentView.Path;

				return $"https://{baseUrl}{pagePath}";
			}
		}
		
		protected virtual void Awake()
		{
			if (this.myAsset == null)
				throw new InvalidOperationException("Website.Awake() called before Init() was called. Did you instantiate a website prefab as enabled?");
			this.AssertAllFieldsAreSerialized(typeof(WebSite));
		}

		public void GoBack()
		{
			if (CanGoBack)
				base.GoBack(null);
		}

		public void GoForward()
		{
			if (CanGoForward)
				NavigateTo(future.Pop());
		}

		public void Init(WebPageAsset asset)
		{
			if (this.myAsset != null)
				throw new InvalidOperationException("I wanna know who the hell called Init() twice. Because you shouldn't.");
			
			this.myAsset = asset;
		}

		public void NavigateToPath(string path)
		{
			
		}
	}
}