using System;

namespace AcidicGui.Theming
{
	public interface IThemeWithAccentColor<TThemeEngine, TAccentColor> : 
		ITheme<TThemeEngine>,
		IAccentColorLookup<TAccentColor>
		where TThemeEngine : IThemeEngine, IAccentColorLookup<TAccentColor>
		where TAccentColor : Enum
	{}
}