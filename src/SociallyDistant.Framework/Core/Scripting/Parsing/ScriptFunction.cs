#nullable enable
using SociallyDistant.Core.Core.Scripting.Instructions;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting.Parsing
{
	public class ScriptFunction : IScriptFunction
	{
		private readonly ShellInstruction body;

		public ScriptFunction(ShellInstruction body)
		{
			this.body = body;
		}
		
		public async Task<int> ExecuteAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext callSite)
		{
			int exitCode = 0;
			
			try
			{
				var scope = new LocalScriptExecutionContext(callSite, false);
				exitCode = await body.RunAsync(console, scope);
			}
			catch (ScriptEndException exit)
			{
				if (exit.LocalScope)
					return exit.ExitCode;

				throw;
			}

			return exitCode;
		}
	}
}