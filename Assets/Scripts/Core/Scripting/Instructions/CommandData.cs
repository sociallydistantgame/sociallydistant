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

		public IArgumentEvaluator Name { get; }
		public IArgumentEvaluator[] ArgumentList { get; }
		public FileRedirectionType RedirectionType { get; }
		public string FilePath { get; }
		
		public CommandData(IScriptExecutionContext context, IArgumentEvaluator name, IEnumerable<IArgumentEvaluator> argumentSource, FileRedirectionType redirectionType, string path)
		{
			this.context = context;

			Name = name;
			ArgumentList = argumentSource.ToArray();
			this.RedirectionType = redirectionType;
			this.FilePath = path;
		}
		
		public async Task<int> ExecuteAsync(ITextConsole console)
		{
			string realName = await Name.GetArgumentText(context, console);
			
			// Evaluate arguments now.
			var args = new string[ArgumentList.Length];
			for (var i = 0; i < args.Length; i++)
			{
				IArgumentEvaluator expr = ArgumentList[i];
				args[i] = await expr.GetArgumentText(context, console);
			}
			
			// What console are we going to send to the command?
			ITextConsole realConsole = this.context.OpenFileConsole(console, this.FilePath, this.RedirectionType);
			
			// Process text arguments to make sure we remove their trailing spaces
			ShellUtility.TrimTrailingSpaces(ref args);
			
			// Hand execution off to the execution context.
			int? commandExitStatus = await this.context.TryExecuteCommandAsync(realName, args, realConsole);
			if (!commandExitStatus.HasValue)
				context.HandleCommandNotFound(realName, realConsole);
			
			if (realConsole is IDisposable disposable)
				disposable.Dispose();

			return commandExitStatus.HasValue ? commandExitStatus.Value : -1;
		}
	}
}