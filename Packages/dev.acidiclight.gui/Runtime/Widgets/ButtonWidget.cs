using System;
using UnityEngine;

namespace AcidicGui.Widgets
{
	public class ButtonWidget : IWidget
	{
		public string Text { get; set; } = string.Empty;
		public Action? ClickAction { get; set; }
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			ButtonWidgetController controller = assembler.GetButtonWidget(destination);

			controller.Text = Text;
			controller.ClickAction = ClickAction;
			
			return controller;
		}
	}
}