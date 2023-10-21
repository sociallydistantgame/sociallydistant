#nullable enable
using UI.Theming;
using UnityEngine;

namespace UI.Themes.ThemedElements
{
	public class ShellThemePreview : OperatingSystemThemeProvider
	{
		[SerializeField]
		private bool useDarkMode;

		private OperatingSystemTheme? myTheme;
		
		/// <inheritdoc />
		protected override OperatingSystemTheme? GetMyTheme()
		{
			return myTheme;
		}

		/// <inheritdoc />
		protected override bool OnChangeTheme(OperatingSystemTheme newTheme)
		{
			myTheme = newTheme;
			return true;
		}

		/// <inheritdoc />
		public override bool UseDarkMode => useDarkMode;

		public void SetPreviewTheme(OperatingSystemTheme theme, bool darkMode)
		{
			this.useDarkMode = darkMode;
			ChangeTheme(theme);
		}
	}
}