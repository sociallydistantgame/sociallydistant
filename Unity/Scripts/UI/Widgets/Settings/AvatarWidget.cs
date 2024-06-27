#nullable enable
using AcidicGui.Widgets;
using Social;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public sealed class AvatarWidget : IWidget
	{
		public AvatarSize Size { get; set; }
		public Color AvatarColor { get; set; }
		public Texture2D? AvatarTexture { get; set; }
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			AvatarWidgetController controller = ((SystemWidgets) assembler).GetAvatar(destination);

			controller.AvatarSize = Size;
			controller.DefaultAvatarColor = AvatarColor;
			controller.AvatarTexture = AvatarTexture;
			
			return controller;
		}
        
		public static AvatarWidget FromSprite(Sprite sprite)
		{
			return new AvatarWidget
			{
				AvatarColor = Color.white,
				AvatarTexture = sprite.texture
			};
		}
	}
}