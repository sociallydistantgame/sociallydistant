using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AcidicGui.Mvc;
using Codice.CM.WorkspaceServer.Tree;
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

				if (baseUrl.StartsWith("about:"))
					return baseUrl;
				
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

		public bool NavigateToPath(string pathAndQuery)
		{
			string path = pathAndQuery;

			int queryIndex = pathAndQuery.IndexOf('?');

			if (queryIndex != -1)
				path = pathAndQuery.Substring(0, queryIndex);

			while (path.StartsWith("/"))
				path = path.Substring(1);
			
			if (string.IsNullOrWhiteSpace(path) || path == "/")
			{
				GoToIndex();
				return true;
			}
			
			Type myType = this.GetType();
            
			var queryParameters = new Dictionary<string, string>();
			foreach (MethodInfo method in myType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
			{
				WebPageAttribute? attribute = method.GetCustomAttributes(false)
					.OfType<WebPageAttribute>()
					.FirstOrDefault();

				if (attribute == null)
					continue;

				if (!attribute.MatchPath(path, queryParameters))
					continue;
				
				// Ladies and gentlemen, we have a winner.
				var paramList = new List<object>();

				ParameterInfo[] methodParams = method.GetParameters();

				foreach (ParameterInfo parameter in methodParams)
				{
					if (queryParameters.TryGetValue(parameter.Name, out string rawValue))
					{
						try
						{
							paramList.Add(Convert.ChangeType(rawValue, parameter.ParameterType));
						}
						catch
						{
							continue;
						}
					}
					else
					{
						paramList.Add(null);
					}
				}

				method.Invoke(this, paramList.ToArray());
				return true;
			}

			return false;
		}

		protected virtual void GoToIndex()
		{
			
		}
	}
}