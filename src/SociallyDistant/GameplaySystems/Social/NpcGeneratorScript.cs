#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.Scripting.Instructions;
using SociallyDistant.Core.Core.Scripting.Parsing;
using SociallyDistant.Core.Social;
using SociallyDistant.GamePlatform;

namespace SociallyDistant.GameplaySystems.Social
{
	public sealed class NpcGeneratorScript :
		ICharacterGenerator
	{
		
		private string narrativeId = string.Empty;
		
		
		private string scriptText;

		private ShellInstruction? script;

		public string NarrativeId
		{
			get => narrativeId;
#if UNITY_EDITOR
			set => narrativeId = value;
#endif
		}

#if UNITY_EDITOR
		public void SetScriptText(string scriptText)
		{
			this.scriptText = scriptText;
		}
#endif
		
		/// <inheritdoc />
		public async Task GenerateNpcs(IWorldManager world)
		{
			var module = new NpcGeneratorFunctions(world, narrativeId);
			var context = new UserScriptExecutionContext();
			context.ModuleManager.RegisterModule(module);
			
			var console = new UnityTextConsole();

			var shell = new InteractiveShell(context);
			shell.Setup(console);

			if (script == null)
				script = await ParseScript(shell, scriptText);

			await shell.RunParsedScript(script);

			module.SavePendingChanges();
		}

		private async Task<ShellInstruction> ParseScript(InteractiveShell shell, string text)
		{
			var parsed = await shell.ParseScript(text);

			var runList = new List<ShellInstruction>();

			runList.Add(parsed);

			runList.Add(new SingleInstruction(new CommandData(new TextArgumentEvaluator("profile"), Array.Empty<IArgumentEvaluator>(), FileRedirectionType.None, null)));
			runList.Add(new SingleInstruction(new CommandData(new TextArgumentEvaluator("feed"), Array.Empty<IArgumentEvaluator>(), FileRedirectionType.None, null)));

			return new SequentialInstruction(runList);
		}
	}
}