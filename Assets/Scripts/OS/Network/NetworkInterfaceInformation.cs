namespace OS.Network
{
	public struct NetworkInterfaceInformation
	{
		public string Name;
		public string MacAddress;
		public string LocalAddress;
		public string SubnetMask;
		public string? DefaultGateway;
	}
}