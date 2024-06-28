namespace SociallyDistant.Core.OS.Network
{
	public interface INetworkInterfaceEnumerator
	{
		IEnumerable<NetworkInterfaceInformation> GetInterfaceInformation();
	}
}