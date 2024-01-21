#nullable enable
using System.Threading.Tasks;
using Core.Scripting.Instructions;
using OS.Devices;

namespace Core.Scripting.Parsing
{
	public class ScriptFunction
	{
		private readonly ShellInstruction body;

		public ScriptFunction(ShellInstruction body)
		{
			this.body = body;
		}
		
		public async Task<int> ExecuteAsync(string name, string[] args, ITextConsole console)
		{
			int exitCode = 0;
			
			try
			{
				exitCode = await body.RunAsync(console);
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