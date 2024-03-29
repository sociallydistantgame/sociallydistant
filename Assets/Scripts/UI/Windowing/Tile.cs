#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Player;
using Shell.Common;
using Shell.Windowing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace UI.Windowing
{
	public class Tile : 
		UIBehaviour,
		ITile
	{
		[SerializeField]
		private PlayerInstanceHolder playerHolder = null!;

		[SerializeField]
		private bool recreateWindowOnClose = false;
		
		private RectTransform rectTRansform;
		private IWorkspaceDefinition workspace;
		private IFloatingGui window;
		private WindowTabManager tabManager;
		private CanvasGroup canvasGroup = null!;

		/// <inheritdoc />
		public bool CanClose
		{
			get => window.CanClose;
			set => window.CanClose = value;
		}

		/// <inheritdoc />
		public void Close()
		{
			window.Close();
		}

		/// <inheritdoc />
		public void ForceClose()
		{
			window.ForceClose();
		}

		/// <inheritdoc />
		public event Action<IWindow>? WindowClosed;

		/// <inheritdoc />
		public WindowHints Hints => window.Hints;

		/// <inheritdoc />
		public IWorkspaceDefinition? Workspace
		{
			get => workspace;
			set => throw new NotSupportedException();
		}

		/// <inheritdoc />
		public CompositeIcon Icon
		{
			get => window.Icon;
			set => window.Icon = value;
		}

		/// <inheritdoc />
		public bool IsActive => window.IsActive;

		/// <inheritdoc />
		public IWorkspaceDefinition CreateWindowOverlay()
		{
			return window.CreateWindowOverlay();
		}

		/// <inheritdoc />
		public void SetWindowHints(WindowHints hints)
		{
			window.SetWindowHints(hints);
		}

		/// <inheritdoc />
		public IContentPanel ActiveContent => window.ActiveContent;

		/// <inheritdoc />
		public IReadOnlyList<IContentPanel> Tabs => tabManager.Tabs;

		/// <inheritdoc />
		public void NextTab()
		{
			tabManager.NextTab();
		}

		/// <inheritdoc />
		public void PreviousTab()
		{
			tabManager.PreviousTab();
		}

		/// <inheritdoc />
		public void SwitchTab(int index)
		{
			tabManager.SwitchTab(index);
		}

		/// <inheritdoc />
		public void CloseTab(int index)
		{
			this.tabManager.CloseTab(index);
		}

		/// <inheritdoc />
		public IContentPanel CreateTab()
		{
			return tabManager.CreateTab();
		}

		/// <inheritdoc />
		public Task<bool> RemoveTab(IContentPanel panel)
		{
			return tabManager.RemoveTab(panel);
		}

		/// <inheritdoc />
		public IReadOnlyList<IWindow> WindowList => workspace.WindowList;

		/// <inheritdoc />
		public IFloatingGui CreateFloatingGui(string title)
		{
			// TODO: Use the desktop's floating workspace in place of the WM's fallback
			return playerHolder.Value.UiManager.WindowManager.FallbackWorkspace.CreateFloatingGui(title);
		}

		/// <inheritdoc />
		public IMessageDialog CreateMessageDialog(string title)
		{
			return playerHolder.Value.UiManager.WindowManager.CreateMessageDialog(title, this);
		}

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			
			this.AssertAllFieldsAreSerialized(typeof(Tile));
			this.MustGetComponent(out canvasGroup);
			this.MustGetComponent(out rectTRansform);
			
			// Creates the underlying tile window
			this.workspace = this.playerHolder.Value.UiManager.WindowManager.DefineWorkspace(this.rectTRansform);
			CreateWindow();
		}

		/// <inheritdoc />
		public Action? NewTabCallback
		{
			get => tabManager.NewTabCallback;
			set => tabManager.NewTabCallback = value;
		}

		/// <inheritdoc />
		public bool ShowNewTab
		{
			get => tabManager.ShowNewTab;
			set => tabManager.ShowNewTab = value;
		}

		/// <inheritdoc />
		public void Hide()
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			canvasGroup.alpha = 0;
		}

		/// <inheritdoc />
		public void Show()
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = 1;
		}

		private void CreateWindow()
		{
			this.window = this.workspace.CreateFloatingGui("Tile title");

			// Sets it up as a maximized window
			this.window.WindowState = WindowState.Maximized;
			
			// Prevents user resizing and hiding
			window.EnableCloseButton = false;
			window.EnableMaximizeButton = false;
			window.EnableMinimizeButton = false;
			
			// Cursed shit to get the tab manager for the underlying window.
			// You shouldn't do this unless your name is Ritchie and you're the lead programmer of the game.
			tabManager = (window as MonoBehaviour)!.MustGetComponent<WindowTabManager>();

			window.WindowClosed += OnUnderlyingWindowClosed;
		}

		private void OnUnderlyingWindowClosed(IWindow win)
		{
			this.window.WindowClosed -= OnUnderlyingWindowClosed;
			this.WindowClosed?.Invoke(this);

			if (recreateWindowOnClose)
			{
				Hide();
				this.CreateWindow();
			}
		}
	}
}