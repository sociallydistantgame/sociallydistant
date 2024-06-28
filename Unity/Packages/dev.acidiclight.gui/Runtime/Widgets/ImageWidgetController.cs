using UnityEngine;
using UnityEngine.UI;

namespace AcidicGui.Widgets
{
	public class ImageWidgetController : WidgetController
	{
		
		private Image image = null!;
		
		public Sprite? Sprite { get; set; }
		public Color Color { get; set; } = Color.white;
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			image.color = Color;
			image.sprite = Sprite;
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			image.sprite = null;
			Sprite = null;
		}
	}
}