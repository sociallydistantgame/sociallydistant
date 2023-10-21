#nullable enable
using System;
using UnityEngine;

namespace UI.Theming
{
	public partial class OperatingSystemTheme
	{
		public Color GetGraphicColor(GraphicColor graphicColor, bool dark)
		{
			if (graphicColor == GraphicColor.Transparent)
				return default;
			
			var themeColor = graphicColor switch
			{
				GraphicColor.Background => widgetStyle.SystemColors.Background,
				GraphicColor.Text => widgetStyle.SystemColors.Text,
				GraphicColor.Selected => widgetStyle.SystemColors.Selection,
				GraphicColor.Panel => widgetStyle.SystemColors.Panel,
				GraphicColor.ToolBar => widgetStyle.SystemColors.ToolBar,
			};

			return this.colors.TranslateColor(themeColor, dark);
		}
	}
}