#nullable enable

using UnityEngine;

namespace UI.Theming
{
	[CreateAssetMenu(menuName = "ScriptableObject/OS Theme")]
	public class OperatingSystemTheme :
		ScriptableObject,
		IOperatingSystemTheme
	{
		private readonly OperatingSystemThemeEngine engine = new OperatingSystemThemeEngine();
		
		/// <inheritdoc />
		public Color GetAccentColor(SystemAccentColor accentColorName)
		{
			return engine.GetAccentColor(accentColorName);
		}
	}
}