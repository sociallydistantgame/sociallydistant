using System;
using AcidicGui.Widgets;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public abstract class SettingsWidget : IWidget
	{
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			if (assembler is not SystemWidgets sysWidgets)
				throw new InvalidOperationException("Cannot create settings widgets using an IWidgetAssembler other than the SystemWidgets one.");

			return BuildSettingsWidget(sysWidgets, destination);
		}

		public abstract WidgetController BuildSettingsWidget(SystemWidgets assembler, RectTransform destination);
	}
}