using NUnit.Framework;
using PropHunt.PlayMode.Tests.Utility;
using System.Collections;
using Unity.Entities.Tests;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using PropHunt.Client.Systems;
using PropHunt.Client.Components;
using PropHunt.Constants;
using Unity.Collections;

namespace PropHunt.PlayMode.Tests.NetworkTests
{
    [TestFixture]
    public class NetworkIntegrationTests : ECSTestsFixture
    {
        private World clientWorld;

        private World serverWorld;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Initialize server world first
            this.serverWorld = ClientServerBootstrap.CreateServerWorld(base.World, "ServerWorld");
            this.serverWorld.EntityManager.CompleteAllJobs();

            // Then initialize client world
            this.clientWorld = ClientServerBootstrap.CreateClientWorld(base.World, "ClientWorld");
            this.clientWorld.EntityManager.CompleteAllJobs();
        }

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
            SceneManager.LoadScene("SampleScene");

            yield return new WaitForSceneLoaded("SampleScene", newTimeout: 30);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ConnectionTest()
        {
            ConnectionSystem connectionManager = this.clientWorld.GetExistingSystem<ConnectionSystem>();
            base.m_Manager.CreateEntity(typeof(NetworkStreamConnection));

            yield return null;
            // Make a connect request
            ConnectionSystem.Instance.RequestConnect(new Game.NetworkControlSettings{
                NetworkAddress = new FixedString64(ProphuntClientServerControlSystem.DefaultNetworkAddress),
                NetworkPort = ProphuntClientServerControlSystem.DefaultNetworkPort
            });
            yield return null;
            yield return new WaitForConnected(connectionManager);
            Assert.IsTrue(connectionManager.GetSingleton<ConnectionComponent>().isConnected);

            yield return null;
            // Make a disconnect request
            ConnectionSystem.Instance.RequestDisconnect();
            yield return null;
            yield return new WaitForConnected(connectionManager, state: false);
            Assert.IsFalse(connectionManager.GetSingleton<ConnectionComponent>().isConnected);
        }
    }
}