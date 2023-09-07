#nullable enable
using Shell.Common;

namespace Shell
{
	public static class ThemeServiceExtensions
	{
		public static SimpleColor GetColor(this IThemeService themeService, ShellColor shellColor)
		{
			if (shellColor.name == ShellColorName.Custom)
				return new SimpleColor
				{
					r = shellColor.r,
					g = shellColor.g,
					b = shellColor.b,
					a = shellColor.a
				};

			return themeService.GetColor(shellColor.name);
		}
	}
}