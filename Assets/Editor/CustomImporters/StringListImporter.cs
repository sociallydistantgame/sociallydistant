#nullable enable
using System;
using System.IO;
using System.Linq;
using Architecture.AssetTypes;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Editor.CustomImporters
{
	[ScriptedImporter(1, "stringlist")]
	public class StringListImporter : ScriptedImporter
	{
		/// <inheritdoc />
		public override void OnImportAsset(AssetImportContext ctx)
		{
			string text = File.ReadAllText(ctx.assetPath);

			string[]? entries = null;
			if (text.StartsWith("%"))
			{
				// Interpret as a multi-line input
				entries = text.Split('%', StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim())
					.ToArray();

			}
			else
			{
				// Interpret as a single-line list
				entries = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim())
					.ToArray();
			}

			var stringListObject = ScriptableObject.CreateInstance<StringList>();
			stringListObject.name = Path.GetFileName(ctx.assetPath);
			stringListObject.SetEntries(entries);

			ctx.AddObjectToAsset(Path.GetFileName(ctx.assetPath), stringListObject);
			ctx.SetMainObject(stringListObject);

		}
	}
}