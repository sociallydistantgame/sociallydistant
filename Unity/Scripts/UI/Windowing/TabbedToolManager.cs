#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Architecture;
using GamePlatform;
using OS.Devices;
using Shell;
using Shell.Windowing;
using UI.Applications.WebBrowser;
using UI.Shell;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;
using System.Threading.Tasks;

namespace UI.Windowing
{
	public class TabbedToolManager : UIBehaviour
	{
		private MainToolGroup terminal = null!;

		
		private MainToolGroup browser = null!;
		
		private ITile tile;
		private GameManager gameManager = null!;

		/// <inheritdoc />
		protected override void Awake()
		{
			gameManager = GameManager.Instance;

			this.AssertAllFieldsAreSerialized(typeof(TabbedToolManager));
			this.MustGetComponent(out tile);
			this.MustGetComponentInParent(out shell);

			this.tools.Clear();

			this.BuildDock();

			base.Awake();
		}
		
		private void MustGetToolGui<T>(out T behaviour) where T : MonoBehaviour
		{
			if (this.tile.ActiveContent is not RectTransformContentPanel contentPanel)
				throw new InvalidOperationException("The current active tool is not a Unity UI-based tool.");

			contentPanel.RectTransform.MustGetComponentInChildren(out behaviour);
		}
		
		public async Task OpenWebBrowser(Uri uri)
		{
			if (uri.Scheme != "web")
				throw new InvalidOperationException("You cannot open a URL with this scheme in the Web Browser.");

			await this.SwitchTools(browser);

			this.MustGetToolGui(out WebBrowserController webBrowserController);

			webBrowserController.Navigate(uri);
		}
	}
}