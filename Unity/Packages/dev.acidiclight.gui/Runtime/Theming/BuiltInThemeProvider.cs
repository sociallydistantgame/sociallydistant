using UnityEngine;

namespace AcidicGui.Theming
{
	public class BuiltInThemeProvider : ThemeProviderComponent<BuiltInTheme, BuiltInThemeEngine>
	{
		
		private BuiltInTheme theme = null!;
		
		/// <inheritdoc />
		protected override BuiltInTheme GetMyTheme()
		{
			return theme;
		}

		/// <inheritdoc />
		protected override bool OnChangeTheme(BuiltInTheme newTheme)
		{
			theme = newTheme;
			return true;
		}
	}
}