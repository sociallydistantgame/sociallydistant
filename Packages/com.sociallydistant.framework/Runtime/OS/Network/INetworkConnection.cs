#nullable enable

using System;
using System.Collections;

namespace OS.Network
{
	public interface INetworkConnection : INetworkInterfaceEnumerator
	{
        // restitch-needed: This is a Unity coroutine and can't be called by mods. Switch to a Task<PingResult> instead.
		IEnumerator Ping(uint address, float timeout, Action<PingResult> callback);

		// restitch-needed: This is a Unity coroutine and can't be called by mods. Switch to a Task<PingResult> instead.
		IEnumerator Connect(uint remoteAddress, ushort remotePort, Action<ConnectionResult> callback);
		
		IListener Listen(ushort port, ServerType serverType = ServerType.Netcat, SecurityLevel secLevel = SecurityLevel.Open);
	}
}