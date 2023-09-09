using System;
using AcidicGui.Widgets;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public class SettingsSliderWidget : SettingsWidget
	{
		public float MinimumValue { get; set; }
		public float MaximumValue { get; set; }
		public float Value { get; set; }
		public Action<float>? Callback { get; set; }
		public string Label { get; set; } = string.Empty;
		public string? Description { get; set; } = string.Empty;
		
		/// <inheritdoc />
		public override WidgetController BuildSettingsWidget(SystemWidgets assembler, RectTransform destination)
		{
			var slider = assembler.GetSettingsSlider(destination);

			slider.Setup(this);
			
			return slider;
		}
	}
}