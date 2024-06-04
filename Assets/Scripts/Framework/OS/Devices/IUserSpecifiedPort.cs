#nullable enable
namespace OS.Devices
{
	public interface IUserSpecifiedPort : INetworkService
	{
		ushort Port { get; set; }
	}
}