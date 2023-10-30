#nullable enable
using System;
using TMPro;
using UI.PlayerUI;
using UI.Themes.ThemeData;
using UI.Theming;
using UnityEngine;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	public class ShellThemePreview : OperatingSystemThemeProvider
	{
		[SerializeField]
		private bool useDarkMode;

		private UiManager uiManager = null!;
		private OperatingSystemTheme? myTheme;

		private void Awake()
		{
			this.MustGetComponentInParent(out uiManager);
		}

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

		/// <inheritdoc />
		public override TMP_FontAsset GetFont(ThemeFont font)
		{
			return uiManager.GetFont(font);
		}

		public void SetPreviewTheme(OperatingSystemTheme theme, bool darkMode)
		{
			this.useDarkMode = darkMode;
			ChangeTheme(theme);
		}
	}
}