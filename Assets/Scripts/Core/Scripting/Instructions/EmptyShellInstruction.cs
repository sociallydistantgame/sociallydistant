#nullable enable
using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting.Instructions
{
	public sealed class EmptyShellInstruction : ShellInstruction
	{
		/// <inheritdoc />
		public override Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context)
		{
			return Task.FromResult(0);
		}
	}
}