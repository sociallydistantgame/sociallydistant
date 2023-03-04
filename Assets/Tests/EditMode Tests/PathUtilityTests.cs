using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OS.Network;
using UnityEngine.TestTools;
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

        [Test]
        public void CidrNotationTest()
        {
            string cidrNotation = "10.0.0.0/24";

            uint expectedMask = 0xffffff00;
            uint expectedAddress = 0x0a000000;

            bool result = NetUtility.TryParseCidrNotation(cidrNotation, out Subnet subnet);
            Assert.IsTrue(result);
            
            Assert.AreEqual(expectedMask, subnet.Mask);
            Assert.AreEqual(expectedAddress, subnet.NetworkAddress);
            Assert.AreEqual(cidrNotation, subnet.CidrNotation);
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
    }
    
    public class PathUtilityTests
    {
        [Test]
        public void IdiotProofTests()
        {
            Assert.IsTrue(PathUtility.SeparatorChar == '/');
        }

        [Test]
        public void GetFileName()
        {
            var paths = new Dictionary<string, string>
            {
                { "/", string.Empty },
                { "/test", "test" },
                { "/home/user", "user" },
                { "~/Documents", "Documents" },
                { "filename.txt", "filename.txt" }
            };

            foreach (string path in paths.Keys)
            {
                string expectedFileName = paths[path];
                string actualFileName = PathUtility.GetFileName(path);
                
                Assert.AreEqual(expectedFileName, actualFileName);
            }
        }
    }
}
