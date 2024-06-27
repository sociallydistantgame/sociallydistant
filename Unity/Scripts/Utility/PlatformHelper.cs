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
			stringBuilder.AppendLine($"<b>{ex.GetType().FullName}:</b>");
			stringBuilder.AppendLine(ex.Message);
			
			if (ShowStackTraces)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("<b>Stack trace:</b>");
				
				string[] lines = ex.StackTrace.Split(Environment.NewLine);

				const int maxLines = 4;

				for (var i = 0; i < Math.Min(maxLines, lines.Length); i++)
					stringBuilder.AppendLine(lines[i]);
				
				if (lines.Length > maxLines)
				{
					int more = lines.Length - maxLines;
					stringBuilder.AppendLine($"<i>...{more} more</i>");
				}
			}
		}
	}
}