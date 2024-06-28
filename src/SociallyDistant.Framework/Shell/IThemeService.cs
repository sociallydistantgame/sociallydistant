#nullable enable

using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.Core.Shell
{
	public interface IThemeService
	{
		bool DarkMode { get; set; }
		
		SimpleColor GetColor(ShellColorName shellColor);
	}
}