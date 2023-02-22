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
			return text;
		}
	}
}