#nullable enable
using System.Collections.Generic;
using UI.Theming;
using UnityEngine;

namespace Modding
{
	public class BasicThemeResourceStorage : IThemeResourceStorage
	{
		private readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
		
		/// <inheritdoc />
		public void SetTheme(OperatingSystemTheme theme)
		{
			// not needed, only needed if the theme's going somewhere.
		}

		/// <inheritdoc />
		public bool TryGetTexture(string textureName, out Texture2D? texture)
		{
			return textures.TryGetValue(textureName, out texture);
		}

		/// <inheritdoc />
		public void AddTexture(string textureName, Texture2D texture)
		{
			textures[textureName] = texture;
		}
	}
}