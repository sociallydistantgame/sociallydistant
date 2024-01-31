using System;
using OS.Devices;
using System.Threading.Tasks;

namespace Core.Scripting.Instructions
{
	public sealed class SingleInstruction : ShellInstruction
	{
		private readonly CommandData command;
		private ISystemProcess? shellProcess;
		private ISystemProcess? currentCommandProcess;
		private ITextConsole? console;
		private bool waiting;
		private bool isCompleted;

		public SingleInstruction(CommandData command)
		{
			this.command = command;
			if (this.command == null)
				throw new InvalidOperationException("JESUS FUCKING CHRIST WHY DID THIS COME IN AS A NULL POINTER");
		}

		/// <inheritdoc />
		public override async Task<int> RunAsync(ITextConsole console)
		{
			return await command.ExecuteAsync(console);
		}
	}
}