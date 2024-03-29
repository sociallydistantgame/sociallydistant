#nullable enable

using System;
using System.IO;
using System.Text;
using System.Threading;
using Core;
using Core.Scripting;
using GameplaySystems.Missions;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using File = UnityEngine.Windows.File;

namespace Editor.CustomImporters
{
	[ScriptedImporter(1, ".markup")]
	public class MarkupImporter : ScriptedImporter
	{
		/// <inheritdoc />
		public override void OnImportAsset(AssetImportContext ctx)
		{
			string path = ctx.assetPath;

			var sb = new StringBuilder();
			using (Stream stream = System.IO.File.OpenRead(path))
			{
				using var streamReader = new StreamReader(stream);

				while (!streamReader.EndOfStream)
				{
					sb.Append(streamReader.ReadToEnd());
				}
			}

			var asset = ScriptableObject.CreateInstance<MarkupAsset>();
			asset.SetMarkup(sb.ToString());

			asset.name = Path.GetFileName(ctx.assetPath);
			
			ctx.AddObjectToAsset(asset.name, asset);
			ctx.SetMainObject(asset);
		}
	}

	[ScriptedImporter(1, ".mission")]
	public sealed class MissionScriptImporter : ScriptedImporter
	{
		/// <inheritdoc />
		public override void OnImportAsset(AssetImportContext ctx)
		{
			var sb = new StringBuilder();
			var uniqueId = string.Empty;
			
			using (var stream = System.IO.File.OpenRead(ctx.assetPath))
			{
				using (var reader = new StreamReader(stream))
				{
					var isFirstLine = true;
					
					while (!reader.EndOfStream)
					{
						string? nextLine = reader.ReadLine();
						if (string.IsNullOrWhiteSpace(nextLine))
							continue;

						if (isFirstLine)
						{
							if (!nextLine.StartsWith("#!"))
								throw new InvalidOperationException("The first line of a mission script must be a shebang specifying the unique ID of the mission.");

							uniqueId = nextLine.Substring(2).Trim();
							if (string.IsNullOrWhiteSpace(uniqueId))
								throw new InvalidOperationException("The first line of a mission script must be a shebang specifying the unique ID of the mission.");

							isFirstLine = false;
							continue;
						}
						
						sb.Append(nextLine);
						sb.Append("\n");
					}
				}
			}

			var mission = ScriptableObject.CreateInstance<MissionScriptAsset>();
			mission.name = uniqueId;

			mission.SetScriptText(sb.ToString());
			mission.ImportMetadata();
			
			ctx.AddObjectToAsset(uniqueId, mission);
			ctx.SetMainObject(mission);
		}
	}
	
	[ScriptedImporter(1, ".sh")]
	public class ShellScriptImporter : ScriptedImporter 
	{
		/// <inheritdoc />
		public override void OnImportAsset(AssetImportContext ctx)
		{
			string path = ctx.assetPath;

			var sb = new StringBuilder();

			ScriptExecutionContext? executionContext = null;

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
						if (line.Substring(2).Trim() == "/bin/sh")
							useOSContext = true;
						else 
							executionContext = FindScriptExecutionContext(line.Substring(2));
						continue;
					}

					sb.Append(line);
					sb.Append('\n');
				}
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