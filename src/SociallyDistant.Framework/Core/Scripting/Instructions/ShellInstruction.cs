#nullable enable

using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting.Instructions
{
	public abstract class ShellInstruction
	{
		public abstract Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context);
	}
}