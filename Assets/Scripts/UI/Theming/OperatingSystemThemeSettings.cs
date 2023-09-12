using AcidicGui.Theming;
using UnityEngine;

namespace UI.Theming
{
	[CreateAssetMenu(menuName = "ScriptableObject/OS Theme Settings")]
	public class OperatingSystemThemeSettings : ThemeSettings<OperatingSystemTheme, OperatingSystemThemeEngine>
	{
		[SerializeField]
		private OperatingSystemTheme defaultShellTheme = null!;
        
		/// <inheritdoc />
		protected override OperatingSystemTheme GetPreviewTheme()
		{
			return defaultShellTheme;
		}
	}
}