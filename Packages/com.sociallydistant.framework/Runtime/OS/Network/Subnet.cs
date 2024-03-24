#nullable enable

using System;
using Core.Serialization;
using UnityEngine.Serialization;


namespace OS.Network
{
	[Serializable]
	public struct Subnet : ISerializable
	{
		[FormerlySerializedAs("NetworkAddress")]
		public uint networkAddress;
		[FormerlySerializedAs("Mask")]
		public uint mask;
		[FormerlySerializedAs("LowerRange")]
		public uint lowerRange;
		[FormerlySerializedAs("HigherRange")]
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