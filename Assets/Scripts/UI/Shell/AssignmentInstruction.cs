using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OS.Devices;

namespace UI.Shell
{
	public class AssignmentInstruction : InteractiveShell.ShellInstruction
	{
		private readonly ICommandShell shell;
		private readonly string identifier;
		private readonly IArgumentEvaluator[] argumentList;
		private bool isComplete;

		public AssignmentInstruction(ICommandShell shell, string identifier, IEnumerable<IArgumentEvaluator> argumentSource)
		{
			this.shell = shell;
			this.identifier = identifier;
			this.argumentList = argumentSource.ToArray();
		}

		/// <inheritdoc />
		public override bool IsComplete => isComplete;

		/// <inheritdoc />
		public override void Update()
		{
			// Stub...nothing to do.
		}

		/// <inheritdoc />
		public override void Begin(ISystemProcess process, ITextConsole consoleDevice)
		{
			// Evaluate the argument list
			string[] args = argumentList.Select(x => x.GetArgumentText(shell))
				.ToArray();

			string newValue = string.Join(string.Empty, args);
			shell.SetVariableValue(identifier, newValue);
				
			isComplete = true;
		}

		/// <inheritdoc />
		public override Task RunAsync(ISystemProcess process, ITextConsole console)
		{
			// TODO: Async argument evaluation, e.g. command substitution
			
			// Evaluate the argument list
			string[] args = argumentList.Select(x => x.GetArgumentText(shell))
				.ToArray();

			string newValue = string.Join(string.Empty, args);
			shell.SetVariableValue(identifier, newValue);

			return Task.CompletedTask;
		}
	}
}