#nullable enable
using System.Threading.Tasks;
using Core.Scripting.Parsing;
using OS.Devices;

namespace Core.Scripting.Instructions
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