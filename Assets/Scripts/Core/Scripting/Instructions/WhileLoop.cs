#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting.Instructions
{
	public sealed class WhileLoop : ShellInstruction
	{
		private readonly ShellInstruction condition;
		private readonly IReadOnlyCollection<ShellInstruction> body;

		public WhileLoop(ShellInstruction condition, IReadOnlyCollection<ShellInstruction> body)
		{
			this.condition = condition;
			this.body = body;
		}

		/// <inheritdoc />
		public override async Task<int> RunAsync(ITextConsole console)
		{
			var exitCode = 0;
			while (await condition.RunAsync(console) != 0)
			{
				foreach (ShellInstruction instruction in body)
					exitCode = await instruction.RunAsync(console);
			}

			return exitCode;
		}
	}
}