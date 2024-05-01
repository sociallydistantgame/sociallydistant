#nullable enable
using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting
{
	public interface IScriptSystem
	{
		Task RunCommandAsync(string name, string[] args, IScriptExecutionContext context, ITextConsole? console = null);
		Task RunHookAsync(string hookName);

		void RegisterHookListener(string hookName, IHookListener listener);
		void UnregisterHookListener(string hookName, IHookListener listener);

		void RegisterGlobalCommand(string commandName, IScriptCommand command);
		void UnregisterGlobalCommand(string commandName);

		IScriptCommand? GetGlobalCommand(string commandName);
	}
}