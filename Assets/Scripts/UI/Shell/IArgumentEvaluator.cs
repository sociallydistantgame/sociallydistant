#nullable enable

namespace UI.Shell
{
	public interface IArgumentEvaluator
	{
		string GetArgumentText(ICommandShell shell);
	}

	public class VariableAccessEvaluator : IArgumentEvaluator
	{
		private readonly string variableName;

		public VariableAccessEvaluator(string variableName)
		{
			this.variableName = variableName;
		}
		
		/// <inheritdoc />
		public string GetArgumentText(ICommandShell shell)
		{
			return shell.GetVariableValue(variableName);
		}
	}
}