#nullable enable
using UnityEngine;

namespace UI.Backdrop
{
	public class BackdropSettings
	{
		public Color ColorTint { get; }
		public Texture2D? Texture { get; }

		public BackdropSettings(Color tint, Texture2D? image)
		{
			this.ColorTint = tint;
			this.Texture = image;

		}
		
		public BackdropSettings(Color color)
			: this(color, null)
		{
			
		}

		public static BackdropSettings Default => new (Color.black);
	}
}