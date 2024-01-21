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
		public override async Task RunAsync(ITextConsole console)
		{
			// TODO: do this.
		}
	}
}