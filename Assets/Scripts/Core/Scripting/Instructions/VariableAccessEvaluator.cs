#nullable enable
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
		public string GetArgumentText(IScriptExecutionContext context)
		{
			return context.GetVariableValue(variableName);
		}
	}
}