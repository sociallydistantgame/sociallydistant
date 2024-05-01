#nullable enable
using System;
using AcidicGui.Widgets;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public sealed class InputFieldWidget : IWidget
	{
		public string Placeholder { get; set; } = "Enter text...";
		public string Value { get; set; } = string.Empty;
		public Action<string>? Callback { get; set; }
		public bool UseFullBorder { get; set; } = true;

		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			var controller = ((SystemWidgets)assembler).GetInputField(destination);
			
			controller.UseFullBorder = UseFullBorder;
			controller.Placeholder = Placeholder;
			controller.Value = Value;
			controller.OnValueChanged = Callback;
            
			return controller;
		}
	}
}