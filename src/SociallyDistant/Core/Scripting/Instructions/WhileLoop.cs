#nullable enable
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Scripting.Instructions
{
	public sealed class WhileLoop : ShellInstruction
	{
		private readonly int maxLoopIterations = 1000;
		private readonly ShellInstruction condition;
		private readonly IReadOnlyCollection<ShellInstruction> body;

		public WhileLoop(ShellInstruction condition, IReadOnlyCollection<ShellInstruction> body)
		{
			this.condition = condition;
			this.body = body;
		}

		/// <inheritdoc />
		public override async Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context)
		{
			var iterations = 0;
			var exitCode = 0;
			while (await condition.RunAsync(console, context) == 0)
			{
				iterations++;
				if (iterations == maxLoopIterations)
				{
					console.WriteText($"[infinite loop prevention] Max loop iterations have been reached. Socially Distant has halted execution of this loop to prevent a deadlock.{Environment.NewLine}");
					return 0;
				}

				await Task.Yield();
				
				foreach (ShellInstruction instruction in body)
					exitCode = await instruction.RunAsync(console, context);
			}

			return exitCode;
		}
	}
}