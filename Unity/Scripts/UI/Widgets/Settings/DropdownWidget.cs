#nullable enable
using System;
using AcidicGui.Widgets;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public sealed class DropdownWidget : IWidget
	{
		public int CurrentIndex { get; set; } = -1;
		public string[] Choices { get; set; } = Array.Empty<string>();
		public Action<int>? Callback { get; set; }
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			DropdownController controller = ((SystemWidgets) assembler).GetDropdown(destination);

			controller.Callback = Callback;
			controller.Choices = Choices;
			controller.CurrentIndex = CurrentIndex;
			
			return controller;
		}
	}
}