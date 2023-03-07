#nullable enable
namespace OS.Network
{
	/// <summary>
	///		Represents a device with a single network interface.
	/// </summary>
	public interface IDeviceWithNetworkInterface
	{
		NetworkInterface NetworkInterface { get; }
	}
}