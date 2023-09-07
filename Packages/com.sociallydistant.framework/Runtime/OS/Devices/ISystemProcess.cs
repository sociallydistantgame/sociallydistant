#nullable enable
using System;
using System.Collections.Generic;

namespace OS.Devices
{
	public interface ISystemProcess
	{
		int Id { get; }
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

		
		void Kill();
	}
}