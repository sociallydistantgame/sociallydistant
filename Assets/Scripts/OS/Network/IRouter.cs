using System.Collections.Generic;

namespace OS.Network
{
	/// <summary>
	///		Represents a device with many network interfaces that decides
	///		how to route packets as they come in on each interface.
	/// </summary>
	public interface IRouter : INetworkNode
	{
		/// <summary>
		///		Gets a list of <see cref="NetworkInterface" /> objects that
		///		may be used to connect neighbouring devices or networks together.
		/// </summary>
		IEnumerable<NetworkInterface> Neighbours { get; }
	}
}