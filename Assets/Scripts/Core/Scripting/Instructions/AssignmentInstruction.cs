using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OS.Devices;
using UI.Shell;

namespace Core.Scripting.Instructions
{
	public class AssignmentInstruction : ShellInstruction
	{
		private readonly IScriptExecutionContext context;
		private readonly string identifier;
		private readonly IArgumentEvaluator[] argumentList;
		private bool isComplete;

		public AssignmentInstruction(IScriptExecutionContext context, string identifier, IEnumerable<IArgumentEvaluator> argumentSource)
		{
			this.context = context;
			this.identifier = identifier;
			this.argumentList = argumentSource.ToArray();
		}
		
		/// <inheritdoc />
		public override Task<int> RunAsync(ITextConsole console)
		{
			// TODO: Async argument evaluation, e.g. command substitution
			
			// Evaluate the argument list
			string[] args = argumentList.Select(x => x.GetArgumentText(context))
				.ToArray();

			string newValue = string.Join(string.Empty, args);
			context.SetVariableValue(identifier, newValue);

			return Task.FromResult(0);
		}
	}
}