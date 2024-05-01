using System;

namespace OS.Network
{
	public struct NetworkInterfaceInformation
	{
		public string Name;
		public string MacAddress;
		public string LocalAddress;
		public string SubnetMask;
		public string? DefaultGateway;

		public Subnet ToSubnet()
		{
			if (!NetUtility.TryParseNetworkAddress(this.SubnetMask, out uint mask))
				throw new InvalidOperationException("Invalid subnet mask");
			
			if (!NetUtility.TryParseNetworkAddress(this.LocalAddress, out uint address))
				throw new InvalidOperationException("Invalid local address");

			return Subnet.FromAddressAndMask(address, mask);
		}
	}
}