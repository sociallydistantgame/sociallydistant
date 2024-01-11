#nullable enable
using System.Collections.Generic;
using Shell.Windowing;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace UI.Windowing
{
	public class WindowTabRenderer : 
		UIBehaviour,
		ITAbRenderer
	{
		[SerializeField]
		private WindowTab tabTemplate = null!;
		
		private readonly List<WindowTab> tabInstances = new List<WindowTab>();
		
		/// <inheritdoc />
		public ITabbedContent? TabbedContent { get; set;  }

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.AssertAllFieldsAreSerialized(typeof(WindowTabRenderer));
			
			this.tabTemplate.gameObject.SetActive(false);
		}

		/// <inheritdoc />
		public void UpdateTabs()
		{
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
			}
		}

		private void SwitchTab(int index)
		{
			TabbedContent?.SwitchTab(index);
		}
	}
}