#nullable enable
using System;
using System.Collections.Generic;
using AcidicGui.Common;
using AcidicGui.Transitions;
using AcidicGui.Widgets;
using Core.Config;
using TMPro;
using UI.Widgets;
using UnityEngine;
using UnityExtensions;

namespace UI.SystemSettings
{
	public class SystemSettingsViewController : 
		MonoBehaviour, 
		IShowOrHide
	{
		
		private RectTransform headerArea = null!;

		
		private TextMeshProUGUI categoryTitle = null!;

		
		private WidgetList widgetList = null!;
		
		private TransitionController transition = null!;

		public bool ShowHeader
		{
			get => headerArea.gameObject.activeSelf;
			set => headerArea.gameObject.SetActive(value);
		}

		public string CategoryTitle
		{
			get => categoryTitle.text;
			set => categoryTitle.SetText(value);
		}
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SystemSettingsViewController));
			this.MustGetComponent(out transition);
		}


		/// <inheritdoc />
		public bool IsVisible => transition.IsVisible;

		/// <inheritdoc />
		public void Show(Action? callback = null)
		{
			transition.Show(callback);
		}

		/// <inheritdoc />
		public void Hide(Action? callback = null)
		{
			transition.Hide(callback);
		}

		public void HideWidgets()
		{
			this.widgetList.SetItems(new List<IWidget>());
		}

		public void ShowWidgets(SettingsCategory category)
		{
			var widgetBuilder = new WidgetBuilder();

			widgetBuilder.Begin();

			var settingsUiBuilder = new WidgetListSettingsUiBuilder(widgetBuilder);

			category.BuildSettingsUi(settingsUiBuilder);
			
			IList<IWidget>? widgets = widgetBuilder.Build();
			
			this.widgetList.SetItems(widgets);
		}
	}
}