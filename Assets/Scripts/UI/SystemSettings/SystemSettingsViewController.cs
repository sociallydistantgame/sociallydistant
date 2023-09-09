#nullable enable
using System;
using System.Collections.Generic;
using AcidicGui.Common;
using AcidicGui.Transitions;
using AcidicGui.Widgets;
using Core.Config;
using Core.Systems;
using TMPro;
using UI.Widgets;
using UI.Widgets.Settings;
using UnityEngine;
using UnityExtensions;

namespace UI.SystemSettings
{
	public class SystemSettingsViewController : 
		MonoBehaviour, 
		IShowOrHide
	{
		[SerializeField]
		private RectTransform headerArea = null!;

		[SerializeField]
		private TextMeshProUGUI categoryTitle = null!;

		[SerializeField]
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

	public class WidgetListSettingsUiBuilder : ISettingsUiBuilder
	{
		private readonly WidgetBuilder widgets;
		private readonly UniqueIntGenerator idGenerator = new UniqueIntGenerator();
		private readonly Dictionary<int, SectionWidget> sectionMap = new Dictionary<int, SectionWidget>();
		
		public WidgetListSettingsUiBuilder(WidgetBuilder builder)
		{
			this.widgets = builder;
		}
		
		/// <inheritdoc />
		public ISettingsUiBuilder AddSection(string sectionTitle, out int sectionId)
		{
			sectionId = idGenerator.GetNextValue();

			widgets.AddSection(sectionTitle, out SectionWidget section);
			
			sectionMap.Add(sectionId, section);

			return this;
		}

		/// <inheritdoc />
		public ISettingsUiBuilder WithLabel(string labelText, int sectionId)
		{
			widgets.AddWidget(new LabelWidget
			{
				Text = labelText
			}, sectionMap[sectionId]);
			
			return this;
		}

		/// <inheritdoc />
		public ISettingsUiBuilder WithToggle(string title, string? description, bool value, Action<bool> changeCallback, int sectionId)
		{
			this.widgets.AddWidget(new SettingsToggleWidget
			{
				Title = title,
				Description = description,
				CurrentValue = value,
				Callback = changeCallback
			}, sectionMap[sectionId]);
			
			return this;
		}

		/// <inheritdoc />
		public ISettingsUiBuilder WithSlider(string title, string? description, float value, float minimum, float maximum, Action<float> changeCallback, int sectionId)
		{
			widgets.AddWidget(new SettingsSliderWidget()
			{
				Label = title,
				Description = description,
				MinimumValue = minimum,
				MaximumValue = maximum,
				Value = value,
				Callback = changeCallback
			}, sectionMap[sectionId]);
			
			return this;
		}

		/// <inheritdoc />
		public ISettingsUiBuilder WithSlider(string title, string? description, float value, int minimum, int maximum, Action<int> changeCallback, int sectionId)
		{
			widgets.AddWidget(new SettingsSliderWidget()
			{
				Label = title,
				Description = description,
				MinimumValue = minimum,
				MaximumValue = maximum,
				Value = value,
				Callback = (newValue) => changeCallback?.Invoke((int) newValue)
			}, sectionMap[sectionId]);
			
			return this;
		}

		/// <inheritdoc />
		public ISettingsUiBuilder WithTextField(string title, string? description, string? currentValue, Action<string?> changeCallbac, int sectionId)
		{
			widgets.AddWidget(new SettingsInputFieldWidget
			{
				Title = title,
				Description = description,
				CurrentValue = currentValue,
				Callback = changeCallbac
			}, sectionMap[sectionId]);
			
			return this;
		}
	}
}