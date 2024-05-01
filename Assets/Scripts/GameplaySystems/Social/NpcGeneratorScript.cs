#nullable enable
using System.Threading.Tasks;
using Core;
using Core.Scripting;
using Core.Scripting.Instructions;
using DevTools;
using GamePlatform;
using UnityEngine;

namespace GameplaySystems.Social
{
	public sealed class NpcGeneratorScript :
		ScriptableObject,
		ICharacterGenerator
	{
		[SerializeField]
		private string scriptText;

		private ShellInstruction? script;
		
#if UNITY_EDITOR
		public void SetScriptText(string scriptText)
		{
			this.scriptText = scriptText;
		}
#endif
		
		/// <inheritdoc />
		public async Task GenerateNpcs(IWorldManager world)
		{
			var module = new NpcGeneratorFunctions(world);
			var context = new UserScriptExecutionContext();
			context.ModuleManager.RegisterModule(module);
			
			var console = new UnityTextConsole();

			var shell = new InteractiveShell(context);
			shell.Setup(console);

			if (script == null)
				script = await shell.ParseScript(scriptText);

			await shell.RunParsedScript(script);

			module.SavePendingChanges();
		}
	}
}