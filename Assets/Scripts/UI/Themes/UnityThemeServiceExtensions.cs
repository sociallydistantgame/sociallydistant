#nullable enable
using Shell.Common;

namespace UI.Themes
{
	public static class UnityThemeServiceExtensions
	{
		public static UnityEngine.Color AsUnityColor(this SimpleColor simpleColor)
		{
			return new UnityEngine.Color(simpleColor.r, simpleColor.g, simpleColor.b, simpleColor.a);
		}
	}
}