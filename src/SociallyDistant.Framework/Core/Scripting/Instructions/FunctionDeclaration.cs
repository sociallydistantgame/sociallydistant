#nullable enable
using SociallyDistant.Core.Core.Scripting.Parsing;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting.Instructions
{
	public sealed class FunctionDeclaration : ShellInstruction
	{
		private readonly string functionName;
		private readonly ShellInstruction body;

		public FunctionDeclaration(string functionName, ShellInstruction body)
		{
			this.functionName = functionName;
			this.body = body;
		}
		
		/// <inheritdoc />
		public override Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context)
		{
			context.DeclareFunction(functionName, new ScriptFunction(body));
			return Task.FromResult(0);
		}
	}
}