#nullable enable
using Core.Scripting;
using Core.Scripting.Instructions;

namespace UI.Shell
{
	public class TextArgumentEvaluator : IArgumentEvaluator
	{
		private readonly string text;

		public TextArgumentEvaluator(string text)
		{
			this.text = text;
		}
		
		/// <inheritdoc />
		public string GetArgumentText(IScriptExecutionContext context)
		{
			// We add a trailing space for shell builtins to use, but the shell will automatically trim this.
			return text + " ";
		}
	}
}