#nullable enable
using OS.Devices;

namespace OS.Network
{
	public interface INetworkInterface
	{
		long MacAddress { get; }
		uint LocalAddress { get; }
		Subnet Network { get; }
		
		IComputer Computer { get; }
	}
}