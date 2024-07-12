#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Core.Scripting;
using GameplaySystems.Chat;
using GameplaySystems.Hacking.Assets;
using GameplaySystems.Social;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using File = UnityEngine.Windows.File;

namespace Editor.CustomImporters
{
	[ScriptedImporter(1, ".sh")]
	public class ShellScriptImporter : ScriptedImporter 
	{	
		private UnityEngine.Object NetworkImporter(string shebang, StringBuilder scriptText)
		{
			var asset = ScriptableObject.CreateInstance<NetworkAsset>();
			asset.NarrativeId = shebang;

			if (string.IsNullOrWhiteSpace(asset.NarrativeId))
				throw new InvalidOperationException("The shebang of a network script must specify the network's Narrative Identifier.");
			
			asset.SetScriptText(scriptText.ToString());
			return asset;
		}
		
		/// <inheritdoc />
		public override void OnImportAsset(AssetImportContext ctx)
		{
			string path = ctx.assetPath;

			var importers = new Dictionary<string, Func<string, StringBuilder, UnityEngine.Object>>()
			{
				{
					"chat",
					ChatImporter
				},
				{
					"npc",
					NpcImporter
				},
				{
					"network",
					NetworkImporter
				}
			};
			var sb = new StringBuilder();

			ScriptExecutionContext? executionContext = null;
			Func<string, StringBuilder, UnityEngine.Object>? importerDelegate = null;

			string? shebang = null;
			
			var useOSContext = false;
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
						string[] splitted = line.Substring(2).Split(' ', StringSplitOptions.RemoveEmptyEntries);
						if (splitted.Length == 0)
							throw new InvalidOperationException("Malformed shebang line.");

						string interpreterName = splitted[0];
						string args = string.Join(" ", splitted.Skip(1).ToArray());

						if (interpreterName == "/bin/sh")
							useOSContext = true;
						else if (importers.TryGetValue(interpreterName, out importerDelegate))
							shebang = args;
						else
							executionContext = FindScriptExecutionContext(interpreterName);
						
						continue;
					}

					sb.Append(line);
					sb.Append('\n');
				}
			}

			if (importerDelegate != null)
			{
				UnityEngine.Object imported = importerDelegate(shebang ?? string.Empty, sb);
				ctx.AddObjectToAsset(imported.name, imported);
				ctx.SetMainObject(imported);
				return;
			}
			
			if (executionContext == null && !useOSContext)
				return;

			if (useOSContext)
			{
				var scriptAsset = ScriptableObject.CreateInstance<OperatingSystemScript>();
                
    			scriptAsset.name = Path.GetFileName(path);
                scriptAsset.SetScriptText(sb.ToString());
                
                ctx.AddObjectToAsset(scriptAsset.name, scriptAsset);
                ctx.SetMainObject(scriptAsset);
			}
			else
			{
				var scriptAsset = ScriptableObject.CreateInstance<ShellScriptAsset>();

				scriptAsset.name = Path.GetFileName(path);
				scriptAsset.SetScriptContext(executionContext);
				scriptAsset.SetScriptText(sb.ToString());

				ctx.AddObjectToAsset(scriptAsset.name, scriptAsset);
				ctx.SetMainObject(scriptAsset);
			}
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