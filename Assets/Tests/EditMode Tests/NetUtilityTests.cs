using System.Collections.Generic;
using System.Security.Permissions;
using NUnit.Framework;
using OS.Network;
using Utility;

namespace Tests.EditMode_Tests
{
	public class NetUtilityTests
	{
		private readonly Dictionary<string, long> macAddresses = new Dictionary<string, long>
		{
			{ "00:00:00:00:00:00", 0x000000000000 },
			{ "ff:ff:ff:ff:ff:ff", 0xffffffffffff },
			{ "de:ad:be:ef:de:ad", 0xdeadbeefdead },
			{ "01:23:45:67:89:ab", 0x0123456789ab },
			{ "cd:ef:01:23:45:67", 0xcdef01234567 },
			{ "89:ab:cd:ef:01:23", 0x89abcdef0123 },
			{ "45:67:89:ab:cd:ef", 0x456789abcdef },
			{ "00:11:22:33:44:55", 0x001122334455 },
			{ "66:77:88:99:aa:bb", 0x66778899aabb },
			{ "cc:dd:ee:ff:00:11", 0xccddeeff0011 },
			{ "22:33:44:55:66:77", 0x223344556677 },
			{ "88:99:aa:bb:cc:dd", 0x8899aabbccdd },
			{ "ee:ff:00:11:22:33", 0xeeff00112233 },
			{ "44:55:66:77:88:99", 0x445566778899 },
			{ "aa:bb:cc:dd:ee:ff", 0xaabbccddeeff },
			{ "86:75:30:9a:bc:de", 0x8675309abcde },
			{ "69:42:06:66:de:af", 0x69420666deaf },
			{ "c0:ff:ee:1a:77:e0", 0xc0ffee1a77e0 }
		};

		private readonly Dictionary<string, ExpectedSubnet> subnetParsingTests = new Dictionary<string, ExpectedSubnet>
		{
			{
				"10.0.0.0/24", new ExpectedSubnet
				{
					SubnetMask = 0xffffff00,
					CidrBits = 24,
					GroupSize = 256,
					NetworkAddress = 0x0a000000,
					LowerRange = 0x0a000000,
					HigherRange = 0x0a0000ff
				}
			},
			{
				"224.248.0.0/16", new ExpectedSubnet
				{
					SubnetMask = 0xffff0000,
					CidrBits = 16,
					GroupSize = 65536,
					NetworkAddress = 0xe0f80000,
					LowerRange = 0xe0f80000,
					HigherRange = 0xe0f8ffff
				}
			}
		};

		[Test]
		public void CidrNotationTest()
		{
			foreach (string cidrNotation in subnetParsingTests.Keys)
			{
				ExpectedSubnet expectedSubnet = subnetParsingTests[cidrNotation];

				bool didParse = NetUtility.TryParseCidrNotation(cidrNotation, out Subnet subnet);
				
				Assert.IsTrue(didParse); // All of these tests must parse.
				
				// CIDR notation of the subnet must match.
				Assert.AreEqual(cidrNotation, subnet.CidrNotation);
				
				// Check the masks are equal and that they have the same number of bits
				// as expected by the CIDR notation.
				Assert.AreEqual(expectedSubnet.SubnetMask, subnet.mask);
				Assert.AreEqual(expectedSubnet.CidrBits, NetUtility.CountBits(subnet.mask));
				
				// Check the network address itself!
				Assert.AreEqual(expectedSubnet.NetworkAddress, subnet.networkAddress);
				
				// Lower ranges must be equal, and must be equal to the network address.
				Assert.AreEqual(expectedSubnet.LowerRange, subnet.lowerRange);
				Assert.AreEqual(subnet.networkAddress, subnet.lowerRange);
				
				// Group size must be equal
				Assert.AreEqual(expectedSubnet.GroupSize, subnet.GroupSize);

				// Higher ranges must be equal, and must be equal to lower range + group size - 1.
				Assert.AreEqual(expectedSubnet.HigherRange, subnet.higherRange);
				Assert.AreEqual(subnet.lowerRange + subnet.GroupSize - 1, subnet.higherRange);
				
				// First host must be the lower range + 1
				Assert.AreEqual(subnet.lowerRange + 1, subnet.FirstHost);
				
				// Last host must be higher range - 1
				Assert.AreEqual(subnet.higherRange - 1, subnet.LastHost);
				
				// Host count must be exactly the group size - 2
				Assert.AreEqual(subnet.GroupSize - 2, subnet.UsableAddressSize);
			}
			
		}
        
		[Test]
		public void MacAddressParseTest()
		{
			foreach (string macAddress in macAddresses.Keys)
			{
				long expected = macAddresses[macAddress];
				NetUtility.TryParseMacAddress(macAddress, out long result);
                
				Assert.AreEqual(expected, result);
			}
		}

		[Test]
		public void MacAddressToStringTest()
		{
			foreach (string expected in macAddresses.Keys)
			{
				long macAddress = macAddresses[expected];
				string result = NetUtility.GetMacAddressString(macAddress);
                
				Assert.AreEqual(expected, result);
			}
		}

		public struct ExpectedSubnet
		{
			public uint SubnetMask;
			public uint LowerRange;
			public uint HigherRange;
			public uint CidrBits;
			public uint NetworkAddress;
			public uint GroupSize;
		}
	}
}