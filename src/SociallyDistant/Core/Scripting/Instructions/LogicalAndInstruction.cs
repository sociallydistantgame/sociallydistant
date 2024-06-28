#nullable enable
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Scripting.Instructions
{
	public sealed class LogicalAndInstruction : ShellInstruction
	{
		private readonly ShellInstruction left;
		private readonly ShellInstruction right;

		public LogicalAndInstruction(ShellInstruction left, ShellInstruction right)
		{
			this.left = left;
			this.right = right;
		}

		/// <inheritdoc />
		public override async Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context)
		{
			int exitCode = await left.RunAsync(console, context);
			if (exitCode != 0)
				return exitCode;
			
			return await right.RunAsync(console, context);
		}
	}
}