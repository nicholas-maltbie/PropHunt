using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using Unity.Entities;
using Unity.NetCode.Tests;

namespace PropHunt.PlayMode.Tests.CharacterController
{
    [TestFixture]
    public class NetworkIntegrationTests
    {
        private Unity.NetCode.Tests.NetCodeTestWorld netCodeTestWorld;

        [SetUp]
        public void Setup()
        {
            
        }

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
            this.netCodeTestWorld = new Unity.NetCode.Tests.NetCodeTestWorld();

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestCode()
        {
            yield return null;
        }
    }
}
