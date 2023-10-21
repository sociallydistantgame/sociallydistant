#nullable enable
using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class ThemeGraphic : IThemeData
	{
		[SerializeField]
		private ThemeColor color = new ThemeColor();

		[SerializeField]
		private ThemeMargins spriteMargins = new ThemeMargins();

		[SerializeField]
		private string graphicName = string.Empty;

		[SerializeField]
		private Texture2D? texture;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(color, assets, nameof(color));
			serializer.Serialize(spriteMargins, assets, nameof(spriteMargins));
            serializer.Serialize(ref graphicName, nameof(graphicName), string.Empty);

            if (serializer.IsReading)
            {
	            assets.RequestTexture(graphicName, (tex) =>
	            {
		            this.texture = tex;
	            });
            }
            else
            {
	            assets.SaveTexture(graphicName, texture);
            }
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.AddLabel("Graphic");

			builder.Name = "Graphic color";
			builder.Description = string.Empty;
			color.BuildWidgets(builder, markDirtyAction, editContext);
			
			builder.AddLabel("Sprite margins");
			spriteMargins.BuildWidgets(builder, markDirtyAction, editContext);
		}
	}
}