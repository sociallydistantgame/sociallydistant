#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting
{
	public interface IScriptFunction
	{
		Task<int> ExecuteAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext callSite);
	}
}