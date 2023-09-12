using AcidicGui.Theming;
using UnityEngine;

namespace UI.Theming
{
	public class OperatingSystemThemeEngine : 
		IThemeEngine,
		IAccentColorLookup<SystemAccentColor>
	{
		/// <inheritdoc />
		public Color GetAccentColor(SystemAccentColor accentColorName)
		{
			// TODO
			return default;
		}
	}
}