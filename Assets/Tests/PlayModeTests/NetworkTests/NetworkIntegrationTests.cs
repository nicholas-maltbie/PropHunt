using NUnit.Framework;
using PropHunt.PlayMode.Tests.Utility;
using System.Collections;
using Unity.Entities.Tests;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine;
using PropHunt.Client.Systems;

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

            yield return new WaitForSceneLoaded("SampleScene");
        }

        [UnityTest]
        public IEnumerator ConnectionTest()
        {
            ConnectionSystem connectionManager = this.clientWorld.GetExistingSystem<ConnectionSystem>();

            yield return null;
            // Make a connect request
            ConnectionSystem.ConnectToServer();
            yield return new WaitForConnected(connectionManager);
            Assert.IsTrue(ConnectionSystem.IsConnected);

            // Let the simulation run for a few seconds
            yield return new WaitForSeconds(3);

            // Make a disconnect request
            ConnectionSystem.DisconnectFromServer();
            yield return new WaitForConnected(connectionManager, state : false);
            Assert.IsFalse(ConnectionSystem.IsConnected);


        }
    }
}
