using System.Collections.Generic;
using System.Linq;
using OS.Devices;
using System.Threading.Tasks;

namespace Core.Scripting.Instructions
{
	public sealed class SequentialInstruction : ShellInstruction
	{
		private readonly ShellInstruction[] instructions;
		private int currentInstruction;
		private bool hasStarted;
		private ISystemProcess? shellProcess;
		private ITextConsole? console;

		public SequentialInstruction(IEnumerable<ShellInstruction> instructionSource)
		{
			instructions = instructionSource.ToArray();
		}

		/// <inheritdoc />
		public override async Task RunAsync(ITextConsole console)
		{
			foreach (ShellInstruction instruction in instructions)
			{
				await instruction.RunAsync(console);
			}
		}
	}
}