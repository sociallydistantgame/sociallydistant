using UnityEngine;

namespace AcidicGui.Widgets
{
	public class RawImageWidget : IWidget
	{
		public Texture2D? Texture { get; set; }
		public Color Color { get; set; } = Color.white;


		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			RawImageWidgetController widget = assembler.GetRawImage(destination);

			widget.Texture = Texture;
			widget.Color = Color;
			
			return widget;
		}
	}
}