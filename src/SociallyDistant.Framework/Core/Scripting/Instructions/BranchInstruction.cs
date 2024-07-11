#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting.Instructions
{
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
		public override async Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context)
		{
			int conditionResult = await condition.RunAsync(console, context);

			if (conditionResult == 0)
			{
				foreach (ShellInstruction instruction in body)
					await instruction.RunAsync(console, context);
			}
			
			return conditionResult;
		}
	}
}