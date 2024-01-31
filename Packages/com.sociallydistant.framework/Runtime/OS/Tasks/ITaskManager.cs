#nullable enable

using System.Collections.Generic;
using OS.Devices;
using System.Threading.Tasks;

namespace OS.Tasks
{
	public interface ITaskManager
	{
		IEnumerable<ISystemProcess> GetTasksForUser(IUser user);
		IEnumerable<ISystemProcess> GetTasksOnComputer(IComputer computer);
		IEnumerable<ISystemProcess> GetChildProcesses(ISystemProcess parent);

		IInitProcess SetUpComputer(IComputer computer, IShellScript loginScript);
	}

	public interface IShellScript
	{
		Task Run(ISystemProcess process, string[] args, ITextConsole console);
	}
}