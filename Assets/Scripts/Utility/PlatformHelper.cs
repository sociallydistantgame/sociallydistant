#nullable enable

using System.Text;
using UnityEditor;
using UnityEngine;
using System;

namespace Utility
{
	public static class PlatformHelper
	{
		public const string ENV_SD_SHOWSTACKTRACES = "SD_SHOWSTACKTRACXES";
		
		public static bool ShowStackTraces
		{
			get
			{
				#if DEBUG
				return true;
				#endif
				
				return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ENV_SD_SHOWSTACKTRACES));
			}
		}
		
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

		public static void AppendException(this StringBuilder stringBuilder, Exception ex)
		{
			if (ShowStackTraces)
			{
				stringBuilder.Append(ex);
			}
			else
			{
				stringBuilder.Append(ex.Message);
			}
		}
	}
}