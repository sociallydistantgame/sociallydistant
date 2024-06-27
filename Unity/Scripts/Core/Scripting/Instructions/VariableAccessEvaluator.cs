#nullable enable
using OS.Devices;
using System.Threading.Tasks;

namespace Core.Scripting.Instructions
{
	public class VariableAccessEvaluator : IArgumentEvaluator
	{
		private readonly string variableName;

		public VariableAccessEvaluator(string variableName)
		{
			this.variableName = variableName;
		}
		
		/// <inheritdoc />
		public Task<string >GetArgumentText(IScriptExecutionContext context, ITextConsole console)
		{
			return Task.FromResult(context.GetVariableValue(variableName));
		}
	}
}