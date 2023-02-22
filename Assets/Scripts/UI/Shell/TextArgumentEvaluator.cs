#nullable enable
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
		public string GetArgumentText(ICommandShell shell)
		{
			// We add a trailing space for shell builtins to use, but the shell will automatically trim this.
			return text + " ";
		}
	}
}