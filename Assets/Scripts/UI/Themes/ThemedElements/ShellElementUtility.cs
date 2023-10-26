#nullable enable
using ThisOtherThing.UI.Shapes;
using UI.Themes.ThemeData;
using UI.Theming;

namespace UI.Themes.ThemedElements
{
	public static class ShellElementUtility
	{
		public static void ApplyRectangleStyle(this Rectangle rectangle, OperatingSystemTheme theme, ThemeRectangleStyle rectStyle, bool useDarkMode)
		{
			rectangle.ShapeProperties.FillColor = theme.TranslateColor(rectStyle.MainColor, useDarkMode);
			rectangle.ShapeProperties.OutlineColor = theme.TranslateColor(rectStyle.BorderColor, useDarkMode);
			rectangle.ShapeProperties.DrawOutline = rectStyle.BorderThickness > 0;
			rectangle.OutlineProperties.LineWeight = rectStyle.BorderThickness;
			rectangle.RoundedProperties.UniformRadius = rectStyle.Rounding;
			rectangle.ForceMeshUpdate();
		}
	}
}