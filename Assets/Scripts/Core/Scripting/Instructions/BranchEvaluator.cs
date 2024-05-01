#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting.Instructions
{
	public sealed class BranchEvaluator : ShellInstruction
	{
		private readonly IReadOnlyCollection<ShellInstruction> cases;
		private readonly IReadOnlyCollection<ShellInstruction> defaultBody;

		public BranchEvaluator(IReadOnlyCollection<ShellInstruction> cases, IReadOnlyCollection<ShellInstruction> defaultBody)
		{
			this.cases = cases;
			this.defaultBody = defaultBody;
		}
		
		/// <inheritdoc />
		public override async Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context)
		{
			foreach (ShellInstruction branch in cases)
			{
				int result = await branch.RunAsync(console, context);
				if (result == 0)
					return result;
			}

			foreach (ShellInstruction instruction in defaultBody)
				await instruction.RunAsync(console, context);
			
			return 0;
		}
	}
}