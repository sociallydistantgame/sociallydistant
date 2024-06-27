#nullable enable

using System;
using System.Threading.Tasks;
using Core.Scripting;
using GamePlatform;
using GameplaySystems.Social;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Editor.CustomImporters
{
	[ScriptedImporter(1, "article")]
	public class ArticleImporter : ScriptedImporter
	{
		/// <inheritdoc />
		public override void OnImportAsset(AssetImportContext ctx)
		{
			string text = System.IO.File.ReadAllText(ctx.assetPath);

			var article = ScriptableObject.CreateInstance<NewsArticleAsset>();
			var console = new UnityTextConsole();
			var userContext = new UserScriptExecutionContext();
			var context = new NewsArticleImportContext(userContext, article);

			userContext.ModuleManager.RegisterModule(new NewsArticleCommands(article));

			try
			{
				Task.Run(async () =>
				{
					var shell = new InteractiveShell(context);
					shell.Setup(console);

					await shell.RunScript(text);
				}).Wait();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				DestroyImmediate(article);
				return;
			}

			ctx.AddObjectToAsset(nameof(article), article);
			ctx.SetMainObject(article);
		}
	}
}