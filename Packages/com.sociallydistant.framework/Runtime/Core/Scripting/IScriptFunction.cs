#nullable enable
using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting
{
	public interface IScriptFunction
	{
		Task<int> ExecuteAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext callSite);
	}
}