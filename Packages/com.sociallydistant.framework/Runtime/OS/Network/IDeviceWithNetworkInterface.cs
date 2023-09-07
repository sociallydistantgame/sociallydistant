#nullable enable
namespace OS.Network
{
	/// <summary>
	///		Represents a device with a single network interface.
	/// </summary>
	public interface IDeviceWithNetworkInterface<TNetworkInterface>
		where TNetworkInterface : ISimulationNetworkPort<TNetworkInterface>
	{
		TNetworkInterface NetworkInterface { get; }
	}

	public interface INetworkInterface
	{
		bool Connected { get; }
		bool Addressable { get; }
		string Name { get; }
		uint NetworkAddress { get; }
		uint SubnetMask { get; }
		long MacAddress { get; }

		void Send(Packet packet);
		Packet? Receive();
		
		NetworkInterfaceInformation GetInterfaceInformation();
	}
}