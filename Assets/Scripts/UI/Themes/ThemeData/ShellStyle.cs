﻿#nullable enable
using System;
using UI.Themes.Serialization;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class ShellStyle : IThemeData
	{
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
		}
	}
}