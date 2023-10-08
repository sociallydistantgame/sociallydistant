using System;
using AcidicGui.Widgets;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public class SettingsInputFieldWidget : SettingsWidget
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string? CurrentValue { get; set; }
		public Action<string>? Callback { get; set; }

		/// <inheritdoc />
		public override WidgetController BuildSettingsWidget(SystemWidgets assembler, RectTransform destination)
		{
			SettingsInputFieldController inputField = assembler.GetSettingsInputField(destination);
            inputField.Setup(this);
			return inputField;
		}
	}
}