#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting.Instructions
{
	public abstract class ShellInstruction
	{
		public abstract Task RunAsync(ITextConsole console);
	}

	public sealed class BranchInstruction : ShellInstruction
	{
		private readonly ShellInstruction condition;
		private readonly IReadOnlyCollection<ShellInstruction> body;
		
		public BranchInstruction(ShellInstruction condition, IReadOnlyCollection<ShellInstruction> body)
		{
			this.condition = condition;
			this.body = body;
		}

		/// <inheritdoc />
		public override async Task RunAsync(ITextConsole console)
		{
			// TODO: do this.
		}
	}
}