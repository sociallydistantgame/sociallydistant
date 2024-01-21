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
		
		public async Task<bool> ExecuteAsync(string name, string[] args, ITextConsole console)
		{
			try
			{
				await body.RunAsync(console);
			}
			catch (ScriptEndException exit)
			{
				if (exit.LocalScope)
					return true;

				throw;
			}

			return true;
		}
	}
}