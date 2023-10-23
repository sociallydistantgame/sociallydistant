﻿#nullable enable
using UI.Widgets;
using UnityEngine;

namespace UI.Themes.Serialization
{
	public interface IThemeEditContext : IGraphicPickerSource
	{
		bool UseDarkMode { get; set; }
		
		bool ColorWithNameExists(string colorName);

		void RenameColor(string colorName, string newName);

		void CancelTemporaryColor();

		void UpdateColors(string colorName, Color dark, Color light);

		void DeleteColor(string colorName);

		Color? GetNamedColor(string colorName, bool useDark);

		string[] GetColorNames();
	}
}