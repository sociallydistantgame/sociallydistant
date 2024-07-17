using System.Reactive.Subjects;
using System.Reflection;
using AcidicGUI.Widgets;

namespace SociallyDistant.GameplaySystems.WebPages
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class WebSiteAttribute : Attribute
	{
		public string HostName { get; }

		public WebSiteAttribute(string hostname)
		{
			this.HostName = hostname;
		}
	}
	
	public abstract class WebSite : Widget
	{
		private readonly Stack<SavedState> history = new Stack<SavedState>();
		private readonly Stack<SavedState> future = new Stack<SavedState>();
		private readonly Subject<string> urlSubject = new Subject<string>();

		public bool CanGoBack => history.Count > 0;
		public bool CanGoForward => future.Count > 0;
		public IObservable<string> UrlObservable => urlSubject;
		
		private WebPageAsset myAsset;
		private SavedState? currentState;

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

				if (this.currentState != null)
					pagePath = currentState.Path;

				return $"web://{baseUrl}{pagePath}";
			}
		}
		
		protected virtual void Awake()
		{
			if (this.myAsset == null)
				throw new InvalidOperationException("Website.Awake() called before Init() was called. Did you instantiate a website prefab as enabled?");
		}

		public void GoBack()
		{
			if (!CanGoBack)
				return;
			
			if (currentState != null)
				future.Push(currentState);

			currentState = history.Pop();

			if (currentState != null)
			{
				currentState.Method?.Invoke();
				urlSubject.OnNext(this.Url);
			}
		}

		public void GoForward()
		{
			if (!CanGoForward)
				return;
            
			if (currentState != null)
				history.Push(currentState);

			currentState = future.Pop();

			if (currentState != null)
			{
				currentState.Method?.Invoke();
				urlSubject.OnNext(Url);
			}
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
				SetHistoryState(pathAndQuery, GoToIndex);
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

				SetHistoryState(pathAndQuery, () =>
				{
					method.Invoke(this, paramList.ToArray());
				});
				return true;
			}

			return false;
		}

		protected virtual void GoToIndex()
		{
			
		}

		public void SetHistoryState(string url, Action methodToExecute)
		{
			if (currentState != null)
				history.Push(currentState);

			future.Clear();

			currentState = new SavedState()
			{
				Path = url,
				Method = methodToExecute
			};

			currentState.Method.Invoke();
			urlSubject.OnNext(Url);
		}
		
		private class SavedState
		{
			public string Path { get; set; } = string.Empty;
			public Action? Method { get; set; }
		}
    }
}