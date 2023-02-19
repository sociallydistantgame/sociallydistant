#nullable enable
using System;
using System.Collections.Generic;

namespace OS.Devices
{
	public interface ISystemProcess
	{
		int Id { get; }
		IUser User { get; }
		ISystemProcess? Parent { get;}
		IEnumerable<ISystemProcess> Children { get; }

		event Action<ISystemProcess>? Killed;

		ISystemProcess Fork();
		ISystemProcess ForkAsUser(IUser user);

		void Kill();
	}
}