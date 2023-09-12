using UnityEngine;

namespace AcidicGui.Theming
{
	[CreateAssetMenu(menuName = "Acidic GUI/Theme Settings/Theme Settings (Built-In Theme Engine)")]
	public class BuiltInThemeSettings : ThemeSettings<BuiltInTheme, BuiltInThemeEngine>
	{
		[SerializeField]
		private BuiltInTheme previewTheme = null!;
		
		/// <inheritdoc />
		protected override BuiltInTheme GetPreviewTheme()
		{
			return previewTheme;
		}
	}
}