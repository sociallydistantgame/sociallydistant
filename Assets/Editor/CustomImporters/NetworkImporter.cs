#nullable enable

using System;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Core.Scripting;
using Editor.ExecutionContexts;
using GamePlatform;
using GameplaySystems.Hacking.Assets;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Windows;

namespace Editor.CustomImporters
{
	[ScriptedImporter(1, ".network")]
	public class NetworkImporter : ScriptedImporter
	{
		/// <inheritdoc />
		public override void OnImportAsset(AssetImportContext ctx)
		{
			string scriptText =  System.IO.File.ReadAllText(ctx.assetPath);

			var network = ScriptableObject.CreateInstance<NetworkAsset>();
			var scriptContext = new NetworkAssetExecutionContext(network);

			var scriptRunner = new InteractiveShell(scriptContext);

			// we want exceptions to be handled by Unity Editor.
			scriptRunner.HandleExceptionsGracefully = false;
			scriptRunner.Setup(new UnityTextConsole());
			
			try
			{
				Task.Run(async () =>
				{
					await scriptRunner.RunScript(scriptText);
				}).Wait();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				DestroyImmediate(network);
				return;
			}

			if (string.IsNullOrWhiteSpace(network.NarrativeId))
			{
				DestroyImmediate(network);
				throw new InvalidOperationException("The network's narrative ID must be set before the end of the script's execution.");
			}

			if (string.IsNullOrWhiteSpace(network.NetworkName))
			{
				DestroyImmediate(network);
				throw new InvalidOperationException("The network's friendly name must be set before the end of the script's execution.");
			}
			
			ctx.AddObjectToAsset($"network__{network.NarrativeId}", network);
			ctx.SetMainObject(network);
		}
	}
}