#nullable enable
using AcidicGui.Theming;
using TMPro;
using UI.Themes.ThemeData;
using UI.Theming;

namespace UI.Themes.ThemedElements
{
	public abstract class OperatingSystemThemeProvider : ThemeProviderComponent<OperatingSystemTheme, OperatingSystemThemeEngine>
	{
		public abstract bool UseDarkMode { get; }

		public abstract TMP_FontAsset GetFont(ThemeFont font);
	}
}