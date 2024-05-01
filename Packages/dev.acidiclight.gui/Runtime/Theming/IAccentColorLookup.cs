#nullable enable

using System;
using UnityEngine;

namespace AcidicGui.Theming
{
	/// <summary>
	///		Interface for a theme engine that supports named accent colors.
	/// </summary>
	/// <typeparam name="TAccentColor">Specify an enum containing a list of accent color names.</typeparam>
	public interface IAccentColorLookup<in TAccentColor>
		where TAccentColor : Enum
	{
		/// <summary>
		///		Gets the RGBA color for a specified accent color name.
		/// </summary>
		/// <param name="accentColorName">The name of the accent color to look up.</param>
		/// <returns>The resulting RGBA color that represents the specified accent color.</returns>
		Color GetAccentColor(TAccentColor accentColorName);
	}

	public abstract class ThemeSettings<TTheme, TThemeEngine> :
		ThemeSettings
		where TTheme : ITheme<TThemeEngine>
		where TThemeEngine : IThemeEngine
	{
		protected abstract TTheme GetPreviewTheme();

		/// <inheritdoc />
		protected internal  override ITheme PreviewTheme => GetPreviewTheme();
	}
}