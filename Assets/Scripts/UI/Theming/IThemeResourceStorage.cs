using UnityEngine;

namespace UI.Theming
{
	public interface IThemeResourceStorage
	{
		void SetTheme(OperatingSystemTheme theme);
		bool TryGetTexture(string textureName, out Texture2D? texture);
		void AddTexture(string textureName, Texture2D texture);
	}
}