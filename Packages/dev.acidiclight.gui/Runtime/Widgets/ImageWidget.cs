using UnityEngine;

namespace AcidicGui.Widgets
{
	public class ImageWidget : IWidget
	{
		public Sprite? Sprite { get; set; }
		public Color Color { get; set; } = Color.white;
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			var widget = assembler.GetImage(destination);

			widget.Sprite = Sprite;
			widget.Color = Color;
			
			return widget;
		}
	}
}