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
using UniRx;

namespace UI.Applications.WebBrowser
{
	public class WebBrowserController : MonoBehaviour
	{
		private void Start()
		{
			backButton.onClick.AddListener(GoBack);
			forwardButton.onClick.AddListener(GoForward);
			homeButton.onClick.AddListener(GoHome);
			navigateButton.onClick.AddListener(Navigate);
			addressBar.onSubmit.AddListener(NavigateTo);
		}

		private void OnDestroy()
		{
			websitePathObserver?.Dispose();
		}
	}
}