#nullable enable
using UnityEngine.UI;
using ThisOtherThing.UI.Shapes;
using ThisOtherThing.UI.ShapeUtils;
using UI.Themes.ThemeData;
using UI.Theming;
using UnityEngine;

namespace UI.Themes.ThemedElements
{
	public static class ShellElementUtility
	{
		public static void ApplyThemeGraphic(this Image image, OperatingSystemTheme theme, ThemeGraphic graphic, bool useDarkMode)
		{
			image.color = theme.TranslateColor(graphic.Color, useDarkMode);

			Texture2D? texture = graphic.Texture;

			if (texture == null)
			{
				image.sprite = null;
				return;
			}

			float width = texture.width;
			float height = texture.height;
			float ppu = 100;
			var pivot = new Vector2(0.5f, 0.5f);
			var bounds = new Rect(0, 0, width, height);

			image.sprite = Sprite.Create(texture, bounds, pivot, ppu, 0, SpriteMeshType.FullRect, graphic.SpriteMargins.ToVector4());
		}
		
		public static void ApplyRectangleStyle(this Rectangle rectangle, OperatingSystemTheme theme, ThemeRectangleStyle rectStyle, bool useDarkMode)
		{
			rectangle.RoundedProperties.Type = rectStyle.Rounding > 0 ? RoundedRects.RoundedProperties.RoundedType.Uniform : RoundedRects.RoundedProperties.RoundedType.None;
			rectangle.ShapeProperties.FillColor = theme.TranslateColor(rectStyle.MainColor, useDarkMode);
			rectangle.ShapeProperties.OutlineColor = theme.TranslateColor(rectStyle.BorderColor, useDarkMode);
			rectangle.ShapeProperties.DrawOutline = rectStyle.BorderThickness > 0;
			rectangle.OutlineProperties.LineWeight = rectStyle.BorderThickness;
			rectangle.RoundedProperties.UniformRadius = rectStyle.Rounding;
			rectangle.ForceMeshUpdate();
		}
	}
}