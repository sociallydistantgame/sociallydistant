#nullable enable
using System;
using UnityEngine;

namespace UI.Themes.Serialization
{
	public abstract class ThemeAssets
	{
		public abstract void RequestTexture(string name, Action<Texture2D?> callback);
		public abstract void SaveTexture(string name, Texture2D? texture);
	}
}