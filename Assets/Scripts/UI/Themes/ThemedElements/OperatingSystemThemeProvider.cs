#nullable enable
using AcidicGui.Theming;
using UI.Theming;

namespace UI.Themes.ThemedElements
{
	public abstract class OperatingSystemThemeProvider : ThemeProviderComponent<OperatingSystemTheme, OperatingSystemThemeEngine>
	{
		public abstract bool UseDarkMode { get; }
	}
}