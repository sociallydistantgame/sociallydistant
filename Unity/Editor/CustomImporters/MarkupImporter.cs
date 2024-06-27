#nullable enable
using System.IO;
using System.Text;
using Core;
using UnityEditor.AssetImporters;
using UnityEngine;

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
}