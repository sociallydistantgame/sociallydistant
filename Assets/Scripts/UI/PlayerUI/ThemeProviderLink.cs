#nullable enable
using TMPro;
using UI.Themes.ThemeData;
using UI.Themes.ThemedElements;
using UI.Theming;
using UnityEngine;
using UnityExtensions;

namespace UI.PlayerUI
{
	public class ThemeProviderLink : OperatingSystemThemeProvider
	{
		[SerializeField]
		private ShellElement linkedElement = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ThemeProviderLink));
		}

		/// <inheritdoc />
		protected override OperatingSystemTheme? GetMyTheme()
		{
			return linkedElement.Provider.CurrentTheme;
		}

		/// <inheritdoc />
		protected override bool OnChangeTheme(OperatingSystemTheme newTheme)
		{
			return true;
		}

		/// <inheritdoc />
		public override bool UseDarkMode => linkedElement.Provider.UseDarkMode;

		/// <inheritdoc />
		public override TMP_FontAsset GetFont(ThemeFont font)
		{
			return linkedElement.Provider.GetFont(font);
		}
	}
}