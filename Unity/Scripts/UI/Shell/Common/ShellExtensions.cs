#nullable enable

using Shell.Common;

namespace UI.Shell.Common
{
	public static class ShellExtensions
	{
		public static ShellColor ToShellColor(this UnityEngine.Color color)
		{
			return new ShellColor(color.r, color.g, color.b, color.a);
		}
	}
}