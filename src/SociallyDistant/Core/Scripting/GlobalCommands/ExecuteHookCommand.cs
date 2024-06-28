#nullable enable

using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Scripting.GlobalCommands
{
	public class ExecuteHookCommand : IScriptCommand
	{
		/// <inheritdoc />
		public async Task ExecuteAsync(IScriptExecutionContext context, ITextConsole console, string name, string[] args)
		{
			if (args.Length < 1)
				throw new InvalidOperationException("Hook name is required.");

			string hookName = args[0];

			await SystemModule.GetSystemModule().Context.ScriptSystem.RunHookAsync(hookName);
		}
	}
}