#nullable enable

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
	}
}