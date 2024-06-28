#nullable enable

using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.OS.Network
{
	[Serializable]
	public struct Subnet : ISerializable
	{
		public uint networkAddress;
		public uint mask;
		public uint lowerRange;
		public uint higherRange;

		public uint FirstHost => lowerRange + 1;
		public uint LastHost => higherRange - 1;
		public uint GroupSize => ~mask + 1;
		public uint UsableAddressSize => GroupSize - 2;
		public uint NetworkId => networkAddress & ~mask;
		
		
		public string CidrNotation
			=> $"{NetUtility.GetNetworkAddressString(networkAddress & mask)}/{NetUtility.CountBits(mask)}";

		/// <inheritdoc />
		public void Write(IDataWriter writer)
		{
			writer.Write(networkAddress);
			writer.Write(mask);
		}

		/// <inheritdoc />
		public void Read(IDataReader reader)
		{
			networkAddress = reader.Read_uint();
			mask = reader.Read_uint();
		}

		public static Subnet FromAddressAndMask(uint address, uint mask)
		{
			var subnet = new Subnet
			{
				mask = mask,
				networkAddress = address & mask
			};
			
			return subnet;
		}
	}
}