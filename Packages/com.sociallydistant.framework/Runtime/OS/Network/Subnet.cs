#nullable enable

using Core.Serialization;


namespace OS.Network
{
	public struct Subnet : ISerializable
	{
		public uint NetworkAddress;
		public uint Mask;
		public uint LowerRange;
		public uint HigherRange;

		public uint FirstHost => LowerRange + 1;
		public uint LastHost => HigherRange - 1;
		public uint GroupSize => ~Mask + 1;
		public uint UsableAddressSize => GroupSize - 2;
		public uint NetworkId => NetworkAddress & ~Mask;
		
		
		public string CidrNotation
			=> $"{NetUtility.GetNetworkAddressString(NetworkAddress & Mask)}/{NetUtility.CountBits(Mask)}";

		/// <inheritdoc />
		public void Write(IDataWriter writer)
		{
			writer.Write(NetworkAddress);
			writer.Write(Mask);
		}

		/// <inheritdoc />
		public void Read(IDataReader reader)
		{
			NetworkAddress = reader.Read_uint();
			Mask = reader.Read_uint();
		}
	}
}