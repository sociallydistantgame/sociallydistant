#nullable enable

using System.Text;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Scripting.Instructions
{
	public interface IArgumentEvaluator
	{
		Task<string> GetArgumentText(IScriptExecutionContext context, ITextConsole console);
	}

	public sealed class ExpressionEvaluator : IArgumentEvaluator
	{
		private readonly IReadOnlyList<IArgumentEvaluator> subExpressions;

		public ExpressionEvaluator(IReadOnlyList<IArgumentEvaluator> expressions)
		{
			this.subExpressions = expressions;
		}


		/// <inheritdoc />
		public async Task<string> GetArgumentText(IScriptExecutionContext context, ITextConsole console)
		{
			var stringBuilder = new StringBuilder();

			foreach (IArgumentEvaluator expression in subExpressions)
			{
				string result = await expression.GetArgumentText(context, console);
				stringBuilder.Append(result);
			}
			
			return stringBuilder.ToString();
		}
	}
}