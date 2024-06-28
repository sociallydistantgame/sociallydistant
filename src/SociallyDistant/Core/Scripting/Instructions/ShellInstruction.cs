#nullable enable

using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Scripting.Instructions
{
	public abstract class ShellInstruction
	{
		public abstract Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context);
	}
}