#nullable enable
using OS.Devices;
using System.Threading.Tasks;

namespace Core.Scripting
{
	public interface IScriptExecutionContext
	{
		string Title { get; }
		string GetVariableValue(string variableName);
		void SetVariableValue(string variableName, string value);

		Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null);

		ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode);

		void HandleCommandNotFound(string name, ITextConsole console);
		void DeclareFunction(string name, IScriptFunction body);
	}
}