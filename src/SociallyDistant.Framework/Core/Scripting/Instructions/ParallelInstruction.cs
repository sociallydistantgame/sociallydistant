using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting.Instructions
{
	public sealed class ParallelInstruction : ShellInstruction
	{
		private readonly ShellInstruction first;
		private readonly ShellInstruction next;

		public ParallelInstruction(ShellInstruction first, ShellInstruction next)
		{
			this.first = first;
			this.next = next;
		}
		
		/// <inheritdoc />
		public override async Task<int> RunAsync( ITextConsole console, IScriptExecutionContext context)
		{
			await Task.WhenAll(first.RunAsync(console, context), next.RunAsync(console, context));
			return 0;
		}
	}
}