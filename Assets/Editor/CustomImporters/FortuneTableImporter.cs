#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Misc.Fortune;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Editor.CustomImporters
{
	[ScriptedImporter(1, "fortunes")]
	public class FortuneTableImporter : ScriptedImporter
	{
		/// <inheritdoc />
		public override void OnImportAsset(AssetImportContext ctx)
		{
			string text = File.ReadAllText(ctx.assetPath);
			
			// Split text into lines and process them, removing comments.
			string[] lines = text.Split(Environment.NewLine);

			var fortuneList = new List<string>();
			var sb = new StringBuilder();
			foreach (string line in lines)
			{
				string current = line;
				int commentIndex = line.IndexOf("#", StringComparison.Ordinal);
				if (commentIndex > -1)
					current = line.Substring(0, commentIndex);

				if (current == "%")
				{
					string trimmed = sb.ToString().Trim();
					sb.Length = 0;

					if (!string.IsNullOrWhiteSpace(trimmed))
						fortuneList.Add(trimmed);

					continue;
				}
				
				if (string.IsNullOrWhiteSpace(current))
					if (sb.Length == 0)
						continue;

				sb.AppendLine(current);

			}
			
			// Make sure we get the last fortune!!!
			string last = sb.ToString().Trim();
			sb.Length = 0;
			if (!string.IsNullOrWhiteSpace(last))
				fortuneList.Add(last);
			
			// Create the asset and set its fortunes list
			var fortuneTableAsset = ScriptableObject.CreateInstance<FortunesTable>();
			fortuneTableAsset.SetFortunesList(fortuneList.ToArray());
			
			// Do some magic Unity stuff I don't understand.
			ctx.AddObjectToAsset("Fortune Table File", fortuneTableAsset);
			ctx.SetMainObject(fortuneTableAsset);
		}
	}
}