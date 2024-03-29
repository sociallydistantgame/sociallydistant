#nullable enable
using System.Collections.Generic;
using Shell.Windowing;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;
using System;

namespace UI.Windowing
{
	public class WindowTabRenderer : 
		UIBehaviour,
		ITAbRenderer
	{
		[SerializeField]
		private WindowTab tabTemplate = null!;

		[SerializeField]
		private Button newTabButton = null!;
		
		private readonly List<WindowTab> tabInstances = new List<WindowTab>();
		private bool showNewTab = false;

		public Action? NewTabCallback { get; set; }
		
		public bool ShowNewTab
		{
			get => showNewTab;
			set
			{
				if (showNewTab == value)
					return;

				showNewTab = value;
				this.UpdateTabs();
			}
		}
		
		/// <inheritdoc />
		public ITabbedContent? TabbedContent { get; set;  }

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.AssertAllFieldsAreSerialized(typeof(WindowTabRenderer));
			this.newTabButton.onClick.AddListener(OnNewTab);
			
			this.tabTemplate.gameObject.SetActive(false);
		}

		/// <inheritdoc />
		public void UpdateTabs()
		{
			this.newTabButton.gameObject.SetActive(this.showNewTab);
			
			int tabCount = 0;

			if (TabbedContent != null && TabbedContent.Tabs != null)
				tabCount = TabbedContent.Tabs.Count;

			while (this.tabInstances.Count > tabCount)
			{
				WindowTab tab = tabInstances[^1];
				tabInstances.RemoveAt(tabInstances.Count - 1);

				tab.Clicked = null;
				
				Destroy(tab.gameObject);
			}

			while (tabInstances.Count < tabCount)
			{
				WindowTab tab = Instantiate(tabTemplate, this.transform);
				this.tabInstances.Add(tab);
				tab.gameObject.SetActive(true);

				tab.Clicked = SwitchTab;
				tab.Closed = CloseTab;
				
			}

			if (TabbedContent == null)
				return;

			if (TabbedContent.Tabs == null)
				return;

			for (var i = 0; i < tabCount; i++)
			{
				WindowTab tab = tabInstances[i];
				IContentPanel panel = TabbedContent.Tabs[i];

				tab.Title = panel.Title;
				tab.TabIndex = i;
				tab.IsActiveTab = TabbedContent.ActiveContent == panel;
				tab.CanBeClosed = panel.CanClose;
			}
		}

		private void CloseTab(int index)
		{
			TabbedContent?.CloseTab(index);
		}
		
		private void SwitchTab(int index)
		{
			TabbedContent?.SwitchTab(index);
		}

		private void OnNewTab()
		{
			NewTabCallback?.Invoke();
		}
	}
}