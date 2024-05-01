#nullable enable
using System;
using System.IO;
using System.Text;
using GameplaySystems.Missions;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Editor.CustomImporters
{
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
}