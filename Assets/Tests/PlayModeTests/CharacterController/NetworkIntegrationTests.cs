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

namespace PropHunt.PlayMode.Tests.CharacterController
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
            // Scene activeScene = SceneManager.GetActiveScene();
            // Unity.Entities.GameObjectConversionUtility.ConvertScene(activeScene, serverSettings);
            this.serverWorld.EntityManager.CompleteAllJobs();
            // Debug.Log($"Num entities in server world: {this.serverWorld.EntityManager.GetAllEntities().Length}");
            // Debug.Log(this.serverWorld.EntityManager.UniversalQuery.GetSingleton<GhostPrefabCollectionComponent>());

            // Ensure main camera still exists
            if (Camera.main == null)
            {
                var camera = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
                var cameraComponent = camera.AddComponent<Camera>();
                camera.tag = "MainCamera";
            }

            // Then initialize client world
            this.clientWorld = ClientServerBootstrap.CreateClientWorld(base.World, "ClientWorld");

            LogAssert.ignoreFailingMessages = true;
        }

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
            SceneManager.LoadScene("SampleScene");

            yield return new WaitForSceneLoaded("SampleScene");
        }

        [UnityTest]
        public IEnumerator TestCode()
        {
            // Make a connect request
            LogAssert.Expect(LogType.Exception, "GetSingleton<Unity.NetCode.NetworkSnapshotAckComponent>() requires that exactly one Unity.NetCode.NetworkSnapshotAckComponent exist that match this query, but there are 0.");
            LogAssert.Expect(LogType.Error, new Regex(@"Large serverTick prediction error. Server tick rollback to \d* delta: -\d*"));

            yield return null;

            this.clientWorld.EntityManager.CreateEntity(typeof(InitClientGameComponent));

            yield return new WaitForSeconds(3);
        }
    }
}
