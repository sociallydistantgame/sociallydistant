#nullable enable

using System.Collections.Generic;
using OS.Devices;

namespace OS.Tasks
{
	public interface ITaskManager
	{
		IEnumerable<ISystemProcess> GetTasksForUser(IUser user);
		IEnumerable<ISystemProcess> GetTasksOnComputer(IComputer computer);
		IEnumerable<ISystemProcess> GetChildProcesses(ISystemProcess parent);

		IInitProcess SetUpComputer(IComputer computer);
	}
}