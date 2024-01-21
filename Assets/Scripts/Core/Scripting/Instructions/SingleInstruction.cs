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
		}

		/// <inheritdoc />
		public override async Task<int> RunAsync(ITextConsole console)
		{
			return await command.ExecuteAsync(console);
		}
	}
}