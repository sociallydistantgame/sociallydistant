#nullable enable

using System;
using System.IO;
using System.Text;
using System.Threading;
using Core.Scripting;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using File = UnityEngine.Windows.File;

namespace Editor.CustomImporters
{
	[ScriptedImporter(1, ".sh")]
	public class ShellScriptImporter : ScriptedImporter 
	{
		/// <inheritdoc />
		public override void OnImportAsset(AssetImportContext ctx)
		{
			string path = ctx.assetPath;

			var sb = new StringBuilder();

			ScriptExecutionContext? executionContext = null;
			using (Stream stream = System.IO.File.OpenRead(path))
			{
				using var streamReader = new StreamReader(stream);
				
				while (!streamReader.EndOfStream)
				{
					string? line = streamReader.ReadLine();
					if (string.IsNullOrWhiteSpace(line))
						continue;

					if (line.StartsWith("#!") && sb.Length == 0)
					{
						executionContext = FindScriptExecutionContext(line.Substring(2));
						continue;
					}

					sb.AppendLine(line);
				}
			}

			if (executionContext == null)
				return;

			var scriptAsset = ScriptableObject.CreateInstance<ShellScriptAsset>();

			scriptAsset.name = Path.GetFileName(path);
			scriptAsset.SetScriptContext(executionContext);
			scriptAsset.SetScriptText(sb.ToString());
			
			ctx.AddObjectToAsset(scriptAsset.name, scriptAsset);
			ctx.SetMainObject(scriptAsset);
		}

		private ScriptExecutionContext FindScriptExecutionContext(string shebang)
		{
			var filter = $"t: {nameof(ScriptExecutionContext)}";

			string[]? guids = AssetDatabase.FindAssets(filter);

			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				string assetName = Path.GetFileNameWithoutExtension(path);
				if (assetName != shebang)
					continue;

				var ctx = AssetDatabase.LoadAssetAtPath<ScriptExecutionContext>(path);
				if (ctx == null)
					continue;

				return ctx;
			}

			throw new InvalidOperationException($"Execution context {shebang} does not exist.");
		}
	}
}