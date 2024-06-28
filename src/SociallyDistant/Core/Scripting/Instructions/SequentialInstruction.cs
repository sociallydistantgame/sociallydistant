using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Scripting.Instructions
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
			instructions = instructionSource.Where(x=>x is not null).ToArray();
		}

		/// <inheritdoc />
		public override async Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context)
		{
			int result = 0;
			foreach (ShellInstruction instruction in instructions)
			{
				result = await instruction.RunAsync(console, context);
			}

			return result;
		}
	}
}