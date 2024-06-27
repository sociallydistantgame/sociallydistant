#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shell.Windowing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;
using UniRx;
using UnityEngine.UI;

namespace UI.Windowing
{
	public class WindowTabManager : 
		UIBehaviour,
		ITabbedContent
	{
		[SerializeField]
		private RectTransform windowContentArea = null!;

		[SerializeField]
		private WindowTabRenderer tabRenderer = null!;
		
		private readonly Dictionary<RectTransformContentPanel, IDisposable> titleObservers = new Dictionary<RectTransformContentPanel, IDisposable>();
		private readonly List<RectTransformContentPanel> panels = new List<RectTransformContentPanel>();
		private bool closeOnEmpty = true;
		private IWindow window;
		private int tabIndex = 0;
		
		internal bool CloseWhenAllTabsAreClosed
		{
			get => closeOnEmpty;
			set => closeOnEmpty = value;
		}
		
		/// <inheritdoc />
		public IContentPanel ActiveContent => panels[tabIndex];

		/// <inheritdoc />
		public Action? NewTabCallback
		{
			get => tabRenderer.NewTabCallback;
			set => tabRenderer.NewTabCallback = value;
		}
		
		/// <inheritdoc />
		public bool ShowNewTab
		{
			get => this.tabRenderer.ShowNewTab;
			set => this.tabRenderer.ShowNewTab = value;
		}

		/// <inheritdoc />
		public IReadOnlyList<IContentPanel> Tabs => panels;

		/// <inheritdoc />
		public void NextTab()
		{
			SwitchTab(tabIndex + 1);
		}

		/// <inheritdoc />
		public void PreviousTab()
		{
			SwitchTab(tabIndex - 1);
		}

		/// <inheritdoc />
		public async void CloseTab(int index)
		{
			await this.RemoveTab(this.Tabs[index]);
		}

		/// <inheritdoc />
		public IContentPanel CreateTab()
		{
			CreateTabInternal(tabIndex+1);

			return panels[tabIndex];
		}

		/// <inheritdoc />
		public async Task<bool> RemoveTab(IContentPanel panel)
		{
			int index = -1;

			foreach (IContentPanel ourPanel in Tabs)
			{
				index++;
				
				if (panel == ourPanel)
					break;
			}

			if (index == -1 || index == panels.Count)
				return false;

			RectTransformContentPanel panelToDestroy = panels[index];
			titleObservers[panelToDestroy].Dispose();
			titleObservers.Remove(panelToDestroy);
			Destroy(panelToDestroy.gameObject);
			
			panels.RemoveAt(index);

			if (index == tabIndex)
			{
				// We removed the current tab.
				if (tabIndex == panels.Count)
				{
					if (tabIndex == 0)
					{
						if (closeOnEmpty)
							this.window?.ForceClose();
						
						tabRenderer.ScheduleUpdateTabs();
						return true;
					}

					tabIndex--;
				}
				
				EnableCurrentTab();
			}
			else if (tabIndex > index)
			{
				tabIndex--;
			}

			tabRenderer.ScheduleUpdateTabs();
			
			return true;
		}

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(WindowTabManager));
			base.Awake();

			this.MustGetComponentInParent(out window);
			
			tabRenderer.TabbedContent = this;
			
			CreateTabInternal(0);
		}

		private void CreateTabInternal(int index)
		{
			var go = new GameObject(nameof(RectTransformContentPanel));
			go.SetActive(false);

			var rt = go.AddComponent<RectTransform>();
			rt.SetParent(this.windowContentArea);

			go.layer = this.windowContentArea.gameObject.layer;
			
			rt.localScale = Vector3.one;

			var panel = go.AddComponent<RectTransformContentPanel>();
			panel.Title = "Tab";
			
			this.panels.Insert(index, panel);

			this.SwitchTabInternal(index);

			this.titleObservers[panel] = panel.TitleObservable.Subscribe(OnTitleChanged);
		}

		/// <inheritdoc />
		public void SwitchTab(int index)
		{
			if (index == tabIndex)
				return;

			SwitchTabInternal(index);
		}

		private void DisableCurrentTab()
		{
			if (tabIndex < 0 && tabIndex <= panels.Count)
				return;

			RectTransformContentPanel panel = panels[tabIndex];
			panel.gameObject.SetActive(false);
		}
		
		private void EnableCurrentTab()
		{
			if (tabIndex < 0 && tabIndex <= panels.Count)
				return;

			RectTransformContentPanel panel = panels[tabIndex];
			panel.gameObject.SetActive(true);
		}
		
		private void SwitchTabInternal(int index)
		{
			if (index < 0 || index >= panels.Count)
				throw new ArgumentOutOfRangeException(nameof(index));
			
			DisableCurrentTab();

			this.tabIndex = index;

			EnableCurrentTab();

			tabRenderer.ScheduleUpdateTabs();
		}

		private void OnTitleChanged(string title)
		{
			tabRenderer.ScheduleUpdateTabs();
		}
	}
}