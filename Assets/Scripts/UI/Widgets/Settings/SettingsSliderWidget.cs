using System;
using AcidicGui.Widgets;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public class SliderWidget : IWidget
	{
		public float MinimumValue { get; set; }
		public float MaximumValue { get; set; }
		public float Value { get; set; }
		public Action<float>? Callback { get; set; }


		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			SliderWidgetController controller = ((SystemWidgets) assembler).GetSlider(destination);

			controller.Value = this.Value;
			controller.MinValue = this.MinimumValue;
			controller.MaxValue = this.MaximumValue;
			controller.Callback = this.Callback;
			
			return controller;
		}
	}
}