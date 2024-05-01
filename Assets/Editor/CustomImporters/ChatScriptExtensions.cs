#nullable enable
using System;
using System.Text;
using System.Threading.Tasks;
using Core.Scripting;
using GamePlatform;
using GameplaySystems.Chat;
using UnityEngine;

namespace Editor.CustomImporters
{
	public static class ChatScriptExtensions
	{
		public static void SetScriptText(this ChatConversationAsset asset, string text)
		{
			asset.ScriptText = text;

			var sb = new StringBuilder();
			sb.AppendLine(asset.ScriptText);
			sb.Append("metadata");

			var console = new UnityTextConsole();
			var context = new ChatImportContext(asset);
			var shell = new InteractiveShell(context);

			shell.Setup(console);

			Task.Run(async () =>
			{
				try
				{
					await shell.RunScript(sb.ToString());
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}).Wait();
		}
	}
}