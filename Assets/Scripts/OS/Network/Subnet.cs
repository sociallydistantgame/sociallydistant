#nullable enable

using Core.Serialization;
using Utility;

namespace OS.Network
{
	public struct Subnet : ISerializable
	{
		public uint NetworkAddress;
		public uint Mask;

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