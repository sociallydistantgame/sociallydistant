#nullable enable
using System;
using System.Text;
using OS.Network;

namespace Utility
{
	public static class NetUtility
	{
		public static bool TryParseNetworkAddress(string networkAddressString, out uint networkAddress)
		{
			networkAddress = default;

			if (string.IsNullOrWhiteSpace(networkAddressString))
				return false;

			if (networkAddressString.Length > 15)
				return false;

			string[] octetStrings = networkAddressString.Split('.');

			if (octetStrings.Length != 4)
				return false;

			var addr = 0u;
			for (var i = 0; i < octetStrings.Length; i++)
			{
				string octetString = octetStrings[i];
				if (!byte.TryParse(octetString, out byte octet))
					return false;

				addr = (addr << 8) + octet;
			}

			networkAddress = addr;
			return true;
		}

		public static string GetNetworkAddressString(uint networkAddress)
		{
			var sb = new StringBuilder(15, 15);

			for (var i = 0; i < 4; i++)
			{
				if (i > 0)
					sb.Append(".");

				var octet = (byte) (networkAddress >> (24 - (i * 8)));
				sb.Append(octet);
			}

			return sb.ToString();
		}

		public static bool TryParseMacAddress(string macAddressString, out long macAddress)
		{
			macAddress = default;

			if (string.IsNullOrWhiteSpace(macAddressString))
				return false;

			if (macAddressString.Length != 17)
				return false;

			long addr = 0;

			try
			{
				for (var i = 0; i < macAddressString.Length; i += 3)
				{
					if (i + 2 != macAddressString.Length && macAddressString[i + 2] != ':')
						return false;

					char aHex = macAddressString[i + 1];
					char bHex = macAddressString[i];

					var octet = (byte) ((16 * StringUtility.GetHexDigitValue(bHex)) + StringUtility.GetHexDigitValue(aHex));
					addr = (addr << 8) + octet;
				}
			}
			catch (FormatException)
			{
				return false;
			}

			macAddress = addr;
			return true;
		}

		public static bool TryParseCidrNotation(string cidrNotation, out Subnet subnet)
		{
			subnet = default;

			if (string.IsNullOrWhiteSpace(cidrNotation))
				return false;

			string[] addressAndMask = cidrNotation.Split('/');
			if (addressAndMask.Length != 2)
				return false;

			string addressString = addressAndMask[0];
			if (!TryParseNetworkAddress(addressString, out uint address))
				return false;

			string maskString = addressAndMask[1];
			if (!byte.TryParse(maskString, out byte maskBitCount))
				return false;

			if (maskBitCount >= 32) // what the fuck?
				return false;

			uint mask = 0;
			for (var i = 0; i < maskBitCount; i++)
			{
				mask |= (uint) (1 << (31 - i));
			}


			// Lower range is just the network address itself
			uint lowerRange = address;
			
			// Group size is how many addresses we can allocate.
			// We get this by doing a bitwise NOT of the mask.
			// Higher range is lower range + group size
			uint groupSize = ~mask;
			uint higherRange = lowerRange + groupSize;

			subnet = new Subnet
			{
				NetworkAddress = address & mask,
				Mask = mask,
				LowerRange = lowerRange,
				HigherRange = higherRange
			};
			
			return true;
		}
		
		public static string GetMacAddressString(long macAddress)
		{
			var sb = new StringBuilder(18, 18);

			for (var i = 0; i < 6; i++)
			{
				if (i > 0)
					sb.Append(":");

				byte octet = (byte) (macAddress >> (40 - (i * 8)));

				if (octet <= 0xf)
					sb.Append("0");

				sb.Append(octet.ToString("x"));
			}
			
			return sb.ToString();
		}

		public static int CountBits(uint value)
		{
			var count = 0;
			for (var i = 0; i < 32; i++)
			{
				if ((value & (1 << i)) != 0)
					count++;
			}

			return count;
		}
	}
}