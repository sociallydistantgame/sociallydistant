namespace SociallyDistant.Core.OS.Network
{
	/// <summary>
	///		Represents a device with many network interfaces that decides
	///		how to route packets as they come in on each interface.
	/// </summary>
	public interface IRouter<TNetworkInterface> : INetworkNode
		where TNetworkInterface : ISimulationNetworkPort<TNetworkInterface>
	{
		/// <summary>
		///		Gets a list of <see cref="TNetworkInterface" /> objects that
		///		may be used to connect neighbouring devices or networks together.
		/// </summary>
		IEnumerable<TNetworkInterface> Neighbours { get; }
		
		IHostNameResolver HostResolver { get; }
	}

	public interface ISimulationNetworkPort<TCompatibleInterfaceType> :
		ISimulationNetworkInterface
		where TCompatibleInterfaceType : INetworkInterface
	{
		void Connect(TCompatibleInterfaceType otherInterface);
		void Disconnect();
	}
	
	public interface ISimulationNetworkInterface : 
		INetworkInterface
	{
		void MakeUnaddressable();
		void MakeAddressable(Subnet subnet, uint address);
	}
}