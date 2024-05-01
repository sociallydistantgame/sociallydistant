#nullable enable

using Shell.Common;

namespace Shell
{
	public interface IThemeService
	{
		bool DarkMode { get; set; }
		
		SimpleColor GetColor(ShellColorName shellColor);
	}
}