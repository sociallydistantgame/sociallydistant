using System.Collections.Generic;

namespace OS.Network
{
	public interface INetworkInterfaceEnumerator
	{
		IEnumerable<NetworkInterfaceInformation> GetInterfaceInformation();
	}
}