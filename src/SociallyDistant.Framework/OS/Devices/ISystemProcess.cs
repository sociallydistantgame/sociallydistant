#nullable enable
using System.Runtime.CompilerServices;

namespace SociallyDistant.Core.OS.Devices
{
	public interface ISystemProcess
	{
		int Id { get; }
		int ExitCode { get; }
		string Name { get; set; }
		IUser User { get; }
		ISystemProcess? Parent { get;}
		IEnumerable<ISystemProcess> Children { get; }
		string WorkingDirectory { get; set; }
		bool IsAlive { get; }
		IEnvironmentVariableProvider Environment { get; }
		
		event Action<ISystemProcess>? Killed;

		ISystemProcess Fork();
		ISystemProcess ForkAsUser(IUser user);

		
		void Kill(int exitCode = 0);

		TaskAwaiter<int> GetAwaiter();
	}
}