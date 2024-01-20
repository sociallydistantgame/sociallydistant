#nullable enable
using OS.Devices;
using System.Threading.Tasks;

namespace Core.Scripting
{
	public interface IScriptExecutionContext
	{
		string GetVariableValue(string variableName);
		void SetVariableValue(string variableName, string value);

		Task<bool> TryExecuteCommandAsync(string name, string[] args, ITextConsole console);

		ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode);

		void HandleCommandNotFound(string name, ITextConsole console);
	}
}