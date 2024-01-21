using System;
using System.Collections.Generic;
using System.Linq;
using Core.Scripting.Consoles;
using OS.Devices;
using OS.FileSystems;
using OS.FileSystems.Host;
using UI.Shell;
using System.Threading.Tasks;

namespace Core.Scripting.Instructions
{
	public class CommandData
	{
		private readonly IScriptExecutionContext context;

		public string Name { get; }
		public IArgumentEvaluator[] ArgumentList { get; }
		public FileRedirectionType RedirectionType { get; }
		public string FilePath { get; }
		
		public CommandData(IScriptExecutionContext context, string name, IEnumerable<IArgumentEvaluator> argumentSource, FileRedirectionType redirectionType, string path)
		{
			this.context = context;

			if (string.IsNullOrWhiteSpace(name))
				throw new InvalidOperationException("Command name can't be whitespace. Likely a parser bug.");

			Name = name;
			ArgumentList = argumentSource.ToArray();
			this.RedirectionType = redirectionType;
			this.FilePath = path;
		}
		
		public async Task<int> ExecuteAsync(ITextConsole console)
		{
			// Evaluate arguments now.
			string[] args = ArgumentList.Select(x => x.GetArgumentText(this.context)).ToArray();
			
			// What console are we going to send to the command?
			ITextConsole realConsole = this.context.OpenFileConsole(console, this.FilePath, this.RedirectionType);
			
			// Process text arguments to make sure we remove their trailing spaces
			ShellUtility.TrimTrailingSpaces(ref args);
			
			// Hand execution off to the execution context.
			int? commandExitStatus = await this.context.TryExecuteCommandAsync(this.Name, args, realConsole);
			if (!commandExitStatus.HasValue)
				context.HandleCommandNotFound(this.Name, realConsole);
			
			if (realConsole is IDisposable disposable)
				disposable.Dispose();

			return commandExitStatus.HasValue ? commandExitStatus.Value : -1;
		}
	}
}