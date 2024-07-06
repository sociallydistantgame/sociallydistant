#nullable enable

using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.OS.Tasks
{
	public interface ITaskManager
	{
		IEnumerable<ISystemProcess> GetTasksForUser(IUser user);
		IEnumerable<ISystemProcess> GetTasksOnComputer(IComputer computer);
		IEnumerable<ISystemProcess> GetChildProcesses(ISystemProcess parent);

		IInitProcess SetUpComputer(IComputer computer);
	}

	public interface IShellScript
	{
		Task Run(ISystemProcess process, string[] args, ITextConsole console);
	}
}