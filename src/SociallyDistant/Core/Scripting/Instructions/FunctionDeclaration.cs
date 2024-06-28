#nullable enable
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Scripting.Parsing;

namespace SociallyDistant.Core.Scripting.Instructions
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