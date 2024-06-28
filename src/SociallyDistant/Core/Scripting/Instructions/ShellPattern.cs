#nullable enable
using System.Text.RegularExpressions;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Scripting.Instructions
{
	public sealed class ShellPattern : ShellInstruction
	{
		private readonly IArgumentEvaluator pattern;
		private readonly ShellInstruction body;

		public ShellPattern(IArgumentEvaluator pattern, ShellInstruction body)
		{
			this.pattern = pattern;
			this.body = body;
		}

		public async Task<bool> Match(ITextConsole console, IScriptExecutionContext context, string evaluatedExpression)
		{
			string evaluatedPattern = await pattern.GetArgumentText(context, console);
			
			// I hate myself
			string regexPattern = "^" + Regex.Escape(evaluatedPattern)
				.Replace(@"\*", ".*")
				.Replace(@"\?", ".") + "$";

			return Regex.IsMatch(evaluatedExpression, regexPattern);
		}
		
		/// <inheritdoc />
		public override async Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context)
		{
			return await body.RunAsync(console, context);
		}
	}

	public sealed class ShellCaseStatement : ShellInstruction
	{
		private readonly IArgumentEvaluator expression;
		private readonly ShellPattern[] patterns;

		public ShellCaseStatement(IArgumentEvaluator expression, IEnumerable<ShellPattern> patterns)
		{
			this.expression = expression;
			this.patterns = patterns.ToArray();
		}

		/// <inheritdoc />
		public override async Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context)
		{
			string evaluatedExpression = await expression.GetArgumentText(context, console);

			foreach (ShellPattern pattern in patterns)
			{
				if (!await pattern.Match(console, context, evaluatedExpression))
					continue;

				return await pattern.RunAsync(console, context);
			}

			return -1;
		}
	}
}