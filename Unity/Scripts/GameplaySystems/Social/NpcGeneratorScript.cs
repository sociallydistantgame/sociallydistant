#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Scripting;
using Core.Scripting.Instructions;
using DevTools;
using GamePlatform;
using UI.Shell;
using UnityEngine;

namespace GameplaySystems.Social
{
	public sealed class NpcGeneratorScript :
		ScriptableObject,
		ICharacterGenerator
	{
		[SerializeField]
		private string narrativeId = string.Empty;
		
		[SerializeField]
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