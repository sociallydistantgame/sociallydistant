using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting.Instructions
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
		public override async Task RunAsync( ITextConsole console)
		{
			await Task.WhenAll(first.RunAsync(console), next.RunAsync(console));
		}
	}
}