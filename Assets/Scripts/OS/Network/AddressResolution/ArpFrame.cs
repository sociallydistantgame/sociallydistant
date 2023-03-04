#nullable enable

namespace OS.Network.AddressResolution
{
	public struct ArpFrame
	{
		public long SourceMacAddress;
		public long DestinationMacAddress;
		public uint SourceNetworkAddress;
		public uint DestinationNetworkAddress;
	}
}