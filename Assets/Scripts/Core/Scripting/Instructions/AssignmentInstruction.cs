using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OS.Devices;
using UI.Shell;
using UniRx;

namespace Core.Scripting.Instructions
{
	public class AssignmentInstruction : ShellInstruction
	{
		private readonly string identifier;
		private readonly IArgumentEvaluator[] argumentList;
		private bool isComplete;

		public AssignmentInstruction(IScriptExecutionContext context, string identifier, IEnumerable<IArgumentEvaluator> argumentSource)
		{
			this.identifier = identifier;
			this.argumentList = argumentSource.ToArray();
		}
		
		/// <inheritdoc />
		public override async Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context)
		{
			// Evaluate the argument list
			var args = new string[argumentList.Length];
			for (var i = 0; i < args.Length; i++)
			{
				IArgumentEvaluator expression = argumentList[i];
				args[i] = await expression.GetArgumentText(context, console);
			}

			string newValue = string.Join(string.Empty, args);
			context.SetVariableValue(identifier, newValue);

			return 0;
		}
	}
}