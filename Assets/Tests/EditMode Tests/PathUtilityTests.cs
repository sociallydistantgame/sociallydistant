using System.Collections;
using System.Collections.Generic;
using Core;
using NUnit.Framework;
using UnityEngine.TestTools;
using Utility;

namespace Tests.EditMode_Tests
{
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
