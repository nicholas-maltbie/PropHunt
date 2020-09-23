using NUnit.Framework;
using PropHunt.PlayMode.Tests.Utility;
using System.Collections;
using Unity.Entities.Tests;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine;
using System.Text.RegularExpressions;
using static PropHunt.Game.ClientGameSystem;
using PropHunt.Client.Systems;
using PropHunt.Game;

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

            // Convert scene to an ecs world
            var serverSettings = GameObjectConversionSettings.FromWorld(this.serverWorld, new BlobAssetStore());
            this.serverWorld.EntityManager.CompleteAllJobs();

            // Ensure main camera still exists
            if (Camera.main == null)
            {
                var camera = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
                var cameraComponent = camera.AddComponent<Camera>();
                camera.tag = "MainCamera";
            }

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
