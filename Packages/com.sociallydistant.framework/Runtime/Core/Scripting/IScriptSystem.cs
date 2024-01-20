#nullable enable
using System.Threading.Tasks;

namespace Core.Scripting
{
	public interface IScriptSystem
	{
		Task RunHookAsync(string hookName);

		void RegisterHookListener(string hookName, IHookListener listener);
		void UnregisterHookListener(string hookName, IHookListener listener);

		void RegisterGlobalCommand(string commandName, IScriptCommand command);
		void UnregisterGlobalCommand(string commandName);

		IScriptCommand? GetGlobalCommand(string commandName);
	}
}