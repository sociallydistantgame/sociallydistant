#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Scripting.Consoles;
using OS.Devices;
using OS.FileSystems;
using OS.FileSystems.Host;
using UI.Shell;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Core.Scripting.Instructions
{
	public class CommandData
	{
		public IArgumentEvaluator Name { get; }
		public IArgumentEvaluator[] ArgumentList { get; }
		public FileRedirectionType RedirectionType { get; }
		public IArgumentEvaluator? FilePath { get; }
		
		public CommandData(IArgumentEvaluator name, IEnumerable<IArgumentEvaluator> argumentSource, FileRedirectionType redirectionType, IArgumentEvaluator? path)
		{
			Name = name;
			ArgumentList = argumentSource.ToArray();
			this.RedirectionType = redirectionType;
			this.FilePath = path;
		}
		
		public async Task<int> ExecuteAsync(ITextConsole console, IScriptExecutionContext context)
		{
			await Task.Yield();
			
			string evaluatedFilePath = string.Empty;
			string realName = await Name.GetArgumentText(context, console);

			if (string.IsNullOrWhiteSpace(realName))
				return 0;
			
			if (this.FilePath != null && this.RedirectionType != FileRedirectionType.None)
				evaluatedFilePath = (await this.FilePath.GetArgumentText(context, console)).Trim();
			
			// Evaluate arguments now.
			var args = new string[ArgumentList.Length];
			for (var i = 0; i < args.Length; i++)
			{
				IArgumentEvaluator expr = ArgumentList[i];
				args[i] = await expr.GetArgumentText(context, console);
			}
			
			// What console are we going to send to the command?
			ITextConsole realConsole = context.OpenFileConsole(console, evaluatedFilePath, this.RedirectionType);
			
			// Process text arguments to make sure we remove their trailing spaces
			ShellUtility.TrimTrailingSpaces(ref args);
			
			// Hand execution off to the execution context.
			int? commandExitStatus = await context.TryExecuteCommandAsync(realName, args, realConsole);
			if (!commandExitStatus.HasValue)
				context.HandleCommandNotFound(realName, realConsole);
			
			if (realConsole is IDisposable disposable)
				disposable.Dispose();

			return commandExitStatus.HasValue ? commandExitStatus.Value : -1;
		}
	}
}