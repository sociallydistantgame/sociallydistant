#nullable enable

using UI.Shell;

namespace Core.Scripting.Instructions
{
	public interface IArgumentEvaluator
	{
		string GetArgumentText(IScriptExecutionContext context);
	}
}