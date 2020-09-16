using System.Collections;
using NUnit.Framework;
using PropHunt.Game;
using PropHunt.PlayMode.Tests.Utility;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static PropHunt.Game.ClientGameSystem;

namespace PropHunt.PlayMode.Tests.CharacterController
{
    [TestFixture]
    public class NetworkIntegrationTests
    {
        private World clientWorld;

        private World serverWorld;

        [SetUp]
        public void Setup()
        {
            // var clientGameSystem = this.clientWorld.GetExistingSystem<ClientGameSystem>();
            // Create entity to auto connect client to server
            // this.clientWorld.EntityManager.CreateEntity(typeof(InitClientGameComponent));

        }

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
            SceneManager.LoadScene("SampleScene");

            yield return new WaitForSceneLoaded("SampleScene");

            var defaultWorld = World.DefaultGameObjectInjectionWorld;
            // Initialize server world first
            this.serverWorld = ClientServerBootstrap.CreateServerWorld(defaultWorld, "ServerWorld");

            // Then initialize client world
            this.clientWorld = ClientServerBootstrap.CreateClientWorld(defaultWorld, "ClientWorld");
        }

        [UnityTest]
        public IEnumerator TestCode()
        {
            // Make a connect request

            yield return null;

            this.clientWorld.EntityManager.CreateEntity(typeof(InitClientGameComponent));

            yield return new WaitForSeconds(3);
        }
    }
}
