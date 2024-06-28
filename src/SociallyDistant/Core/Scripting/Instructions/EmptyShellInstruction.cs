#nullable enable
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Scripting.Instructions
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