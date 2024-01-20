#nullable enable

using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting
{
	public interface IScriptCommand
	{
		Task ExecuteAsync(IScriptExecutionContext context, ITextConsole console, string name, string[] args);
	}
}