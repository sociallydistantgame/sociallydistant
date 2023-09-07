namespace OS.Network
{
	/// <summary>
	///		Represents a device that acts as a router in principle, but also has an interface
	///		that acts as a default gateway.
	/// </summary>
	public interface INetworkSwitch<TNetworkInterface> : 
		IRouter<TNetworkInterface>,
		IDeviceWithNetworkInterface<TNetworkInterface>
		where TNetworkInterface : ISimulationNetworkPort<TNetworkInterface>
	{
		
	}
}