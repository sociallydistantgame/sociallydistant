#nullable enable
namespace SociallyDistant.Core.OS.Devices
{
	public interface IUserSpecifiedPort : INetworkService
	{
		ushort Port { get; set; }
	}
}