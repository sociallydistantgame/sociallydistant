#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting
{
	public interface IScriptCommand
	{
		Task ExecuteAsync(IScriptExecutionContext context, ITextConsole console, string name, string[] args);
	}
}