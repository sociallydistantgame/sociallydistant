using System;
using AcidicGui.Widgets;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public class SettingsDropdownWidget : SettingsWidget
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public int CurrentIndex { get; set; }
		public string[] Choices { get; set; } = Array.Empty<string>();
		public Action<int>? Callback { get; set; }
		
		/// <inheritdoc />
		public override WidgetController BuildSettingsWidget(SystemWidgets assembler, RectTransform destination)
		{
			SettingsDropdownWidgetController controller = assembler.GetSettingsDropdown(destination);

			controller.Setup(this);
			
			return controller;
		}
	}
}