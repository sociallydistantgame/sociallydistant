#nullable enable
using OS.Devices;
using System.Threading.Tasks;

namespace Core.Scripting
{
	public interface IInteractiveShellContext : IScriptExecutionContext
	{
		Task WritePrompt(ITextConsole console);
	}
}