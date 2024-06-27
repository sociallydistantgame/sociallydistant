#nullable enable
using System;
using System.Text;
using UnityEngine;

namespace UI.ScriptableCommands
{
	public sealed class UnityLogReceiver
	{
		private readonly StringBuilder logBuilder = new StringBuilder();
		
		public UnityLogReceiver()
		{
			Application.logMessageReceived += OnLogMessageReceived;
		}

		public string GetLog()
		{
			return logBuilder.ToString();
		}

		private void OnLogMessageReceived(string condition, string stacktrace, LogType type)
		{
			DateTime raphaelTime = DateTime.UtcNow;
			
			logBuilder.Append("[");
			logBuilder.Append(raphaelTime.ToLongTimeString());
			logBuilder.Append(" UTC] ");
			
			logBuilder.Append(type switch
			{
				LogType.Log => "OK",
				LogType.Error => "ERROR",
				LogType.Assert => "Debug Assertion",
				LogType.Warning => "Warning",
				LogType.Exception => "PANIC!",
				_ => "Log message"
			});
			logBuilder.Append(": ");
			
			logBuilder.AppendLine(condition);
		}
	}
}