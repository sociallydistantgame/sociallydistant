#nullable enable
using System;
using AcidicGui.Widgets;
using UnityEngine;

namespace UI.Widgets
{
	public class GraphicPickerWidget : IWidget
	{
		public string? GraphicName { get; set; }
		public Action<string?, Texture2D?>? Callback { get; set; }
		public IGraphicPickerSource? GraphicSource { get; set; }
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			GraphicPickerWidgetController controller = (assembler as SystemWidgets)!.GetGraphicPicker(destination);

			controller.GraphicName = GraphicName;
			controller.GraphicSource = GraphicSource;
			controller.Callback = Callback;
			
			return controller;
		}
	}
}