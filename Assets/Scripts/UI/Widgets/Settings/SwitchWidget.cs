#nullable enable
using System;
using AcidicGui.Widgets;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public sealed class SwitchWidget : IWidget
	{
		public bool IsActive { get; set; }
		public Action<bool>? Callback { get; set; }
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			SwitchWidgetController switchToggle = ((SystemWidgets) assembler).GetSwitch(destination);

			switchToggle.IsActive = IsActive;
			switchToggle.Callback = Callback;

			return switchToggle;
		}
	}
}