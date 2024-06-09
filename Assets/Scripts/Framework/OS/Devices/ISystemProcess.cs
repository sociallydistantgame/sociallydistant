#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OS.Devices
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

		Task<ISystemProcess> Fork();
		Task<ISystemProcess> ForkAsUser(IUser user);

		
		void Kill(int exitCode = 0);
	}
}