#nullable enable

using System;
using AcidicGui.Widgets;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public class SettingsToggleWidget : SettingsWidget
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public bool CurrentValue { get; set; }
		public Action<bool>? Callback { get; set; }
		
		/// <inheritdoc />
		public override WidgetController BuildSettingsWidget(SystemWidgets assembler, RectTransform destination)
		{
			SettingsToggleWidgetController toggle = assembler.GetSettingsToggle(destination);

			toggle.Title = Title;
			toggle.Description = Description;
			toggle.CurrentValue = this.CurrentValue;
			toggle.Callback = Callback;
			
			return toggle;
		}
	}
}