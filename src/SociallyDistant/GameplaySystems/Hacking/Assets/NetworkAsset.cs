#nullable enable

using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.Scripting.Instructions;
using SociallyDistant.Core.Core.Scripting.Parsing;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.OS.Network;
using SociallyDistant.GamePlatform;

namespace SociallyDistant.GameplaySystems.Hacking.Assets
{
	public class NetworkAsset : ShellScriptAsset, 
		INetworkAsset
	{
		private readonly string scriptText = string.Empty;
		private readonly string narrativeId = string.Empty;

		internal NetworkAsset(string narrativeId, string scriptText)
		{
			this.narrativeId = narrativeId;
			this.scriptText = scriptText;
		}

		private ShellInstruction? scriptTree;

		public string NarrativeId => narrativeId;

		public async Task RebuildScriptTree()
		{
			var context = new UserScriptExecutionContext();
			var console = new UnityTextConsole();

			var runner = new InteractiveShell(context);
			runner.Setup(console);

			ShellInstruction script = await runner.ParseScript(this.scriptText);

			var call = new SingleInstruction(new CommandData(new TextArgumentEvaluator("build"), Array.Empty<IArgumentEvaluator>(), FileRedirectionType.None, null));

			var finalScript = new SequentialInstruction(new[] { script, call });

			this.scriptTree = finalScript;
		}

		public async Task Build(IWorldManager worldManager)
		{
			if (this.scriptTree == null)
				await this.RebuildScriptTree();

			var context = new UserScriptExecutionContext();

			context.ModuleManager.RegisterModule(new NetworkScriptFunctions(worldManager, narrativeId));

			var console = new UnityTextConsole();

			var runner = new InteractiveShell(context);
			runner.Setup(console);

			await runner.RunParsedScript(this.scriptTree!);
		}
	}
}