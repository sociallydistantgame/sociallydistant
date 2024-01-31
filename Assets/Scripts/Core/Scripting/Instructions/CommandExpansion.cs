#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting.Instructions
{
	public sealed class CommandExpansion : IArgumentEvaluator
	{
		private readonly string text;

		public CommandExpansion(string text)
		{
			this.text = text;
		}


		/// <inheritdoc />
		public async Task<string> GetArgumentText(IScriptExecutionContext context, ITextConsole console)
		{
			var expansionConsole = new CommandExpansionConsole(console);
			var shell = new InteractiveShell(context);

			shell.Setup(expansionConsole);
			
			await shell.RunScript(this.text);

			return expansionConsole.Text;
		}
	}
}