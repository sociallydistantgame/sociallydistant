#nullable enable

using UnityEditor;
using UnityEngine;

namespace Utility
{
	public static class PlatformHelper
	{
		public static string GetClipboardText()
		{
			return GUIUtility.systemCopyBuffer;
		}

		public static void SetClipboardText(string text)
		{
			GUIUtility.systemCopyBuffer = text;
		}

		public static void QuitToDesktop()
		{
#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
		}
	}
}