#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting
{
	public interface IInteractiveShellContext : IScriptExecutionContext
	{
		Task WritePrompt(ITextConsole console);
	}
}