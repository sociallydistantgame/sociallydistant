using UnityEngine;
using UnityEngine.UI;

namespace AcidicGui.Widgets
{
	public class RawImageWidgetController : WidgetController
	{
		[SerializeField]
		private RawImage rawImage = null!;
		
		public Texture2D? Texture { get; set; }
		public Color Color { get; set; } = Color.white;
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			rawImage.color = Color;
			rawImage.texture = Texture;
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			Texture = null;
			rawImage.texture = null;
		}
	}
}