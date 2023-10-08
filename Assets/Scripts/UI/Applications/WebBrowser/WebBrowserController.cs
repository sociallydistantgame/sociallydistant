#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using GamePlatform;
using GameplaySystems.WebPages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Applications.WebBrowser
{
	public class WebBrowserController : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManager = null!;
		
		[Header("UI")]
		[SerializeField]
		private Button backButton = null!;

		[SerializeField]
		private Button forwardButton = null!;

		[SerializeField]
		private Button homeButton = null!;

		[SerializeField]
		private Button navigateButton = null!;

		[SerializeField]
		private TMP_InputField addressBar = null!;

		[SerializeField]
		private RectTransform pageArea = null!;

		[Header("Settings")]
		[SerializeField]
		private WebPageAsset homepage = null!;

		public string CurrentUrl => addressBar.text;
		
		private readonly Stack<string> future = new Stack<string>();
		private readonly Stack<string> history = new Stack<string>();
		private WebSite? current;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(WebBrowserController));
		}

		private void Start()
		{
			backButton.onClick.AddListener(GoBack);
			forwardButton.onClick.AddListener(GoForward);
			homeButton.onClick.AddListener(GoHome);
			navigateButton.onClick.AddListener(Navigate);
			addressBar.onSubmit.AddListener(NavigateTo);

			NavigateTo(homepage, null, false);
		}

		private void UpdateUI()
		{
			backButton.enabled = history.Count > 0 || (current != null && current.CanGoBack);
			forwardButton.enabled = future.Count > 0 || (current != null && current.CanGoForward);

			if (current != null)
			{
				if (current.WebSiteAsset == homepage)
					addressBar.SetTextWithoutNotify(string.Empty);
				else
					addressBar.SetTextWithoutNotify(current.Url);
			}
		}

		private void GoBack()
		{
			if (current != null)
			{
				if (current.CanGoBack)
				{
					current.GoBack();
					UpdateUI();
					return;
				}

				Destroy(current.gameObject);
			}

			future.Push(CurrentUrl);

			NavigateTo(history.Pop(), false);
		}

		private void GoForward()
		{
			if (current != null)
			{
				if (current.CanGoForward)
				{
					current.GoForward();
					UpdateUI();
					return;
				}

				Destroy(current.gameObject);
			}

			history.Push(CurrentUrl);
			NavigateTo(future.Pop(), false);
		}

		private void GoHome()
		{
			NavigateTo(homepage, null, true);
		}

		private void Navigate()
		{
			NavigateTo(addressBar.text);
		}

		private bool ParseUrl(string text, out Uri uri)
		{
			if (text.StartsWith("http://") || text.StartsWith("https://"))
				return Uri.TryCreate(text, UriKind.Absolute, out uri);
			
            return Uri.TryCreate("https://" + text, UriKind.Absolute, out uri);
		}

		private void NavigateTo(string urlOrSearchQuery)
		{
			NavigateTo(urlOrSearchQuery, true);
		}
		
		private void NavigateTo(string urlOrSearchQuery, bool addToHistory)
		{
			if (!ParseUrl(urlOrSearchQuery, out Uri uri))
			{
				Search(urlOrSearchQuery, addToHistory);
				return;
			}

			// Find a website with a matching hostname
			WebPageAsset? asset = null;
			if (gameManager.Value != null)
			{
				asset = gameManager.Value.ContentManager.GetContentOfType<WebPageAsset>()
					.FirstOrDefault(x => x.HostName == uri.Host);
			}

			// TODO: 404: Code Not Found.
			if (asset == null)
				return;
			
			NavigateTo(asset, uri.PathAndQuery, addToHistory);
		}

		private void Search(string searchQuery, bool addToHistory)
		{
			if (searchQuery == homepage.HostName)
			{
				GoHome();
				return;
			}

			if (searchQuery == "about:blank")
			{
				if (addToHistory)
				{
					future.Clear();
					history.Push(CurrentUrl);
				}
				
				if (current != null)
					Destroy(current.gameObject);

				current = null;
				return;
			}
		}

		private void NavigateTo(WebPageAsset asset, string? path = null, bool pushHistory = false)
		{
			path ??= "/";

			if (pushHistory)
			{
				future.Clear();
				history.Push(CurrentUrl);
			}

			if (current != null)
			{
				Destroy(current.gameObject);
				current = null;
			}
			
			current = asset.InstantiateWebSite(pageArea, path);
			
			UpdateUI();
		}
	}
}