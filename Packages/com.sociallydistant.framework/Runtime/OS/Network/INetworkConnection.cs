#nullable enable

using System;
using System.Collections;
using System.Threading.Tasks;

namespace OS.Network
{
	public interface INetworkConnection : INetworkInterfaceEnumerator
	{
        // restitch-needed: This is a Unity coroutine and can't be called by mods. Switch to a Task<PingResult> instead.
		Task<PingResult> Ping(uint address, float timeout);

		// restitch-needed: This is a Unity coroutine and can't be called by mods. Switch to a Task<PingResult> instead.
		Task<ConnectionResult> Connect(uint remoteAddress, ushort remotePort);
		
		IListener Listen(ushort port, ServerType serverType = ServerType.Netcat, SecurityLevel secLevel = SecurityLevel.Open);
	}
}