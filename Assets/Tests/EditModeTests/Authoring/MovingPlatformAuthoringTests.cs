using Moq;
using Moq.Protected;
using NUnit.Framework;
using PropHunt.Authoring;
using PropHunt.Client.Systems;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace PropHunt.EditMode.Tests.Authoring
{
    /// <summary>
    /// Tests for the moving platform authorining component operation
    /// </summary>
    [TestFixture]
    public class MovingPlatformAuthoringTests : ECSTestsFixture
    {
        private GameObject platformObject;

        private GameObjectConversionSettings settings;

        private BlobAssetStore assetStore;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.platformObject = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.Euler(Vector3.zero));
            this.assetStore = new BlobAssetStore();
            this.settings = GameObjectConversionSettings.FromWorld(base.World, assetStore);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            // Cleanup platform object
            GameObject.DestroyImmediate(this.platformObject);
            assetStore.Dispose();
        }

        [Test]
        public void VerifyCreatePlatformSettings()
        {
            // Number of transform objects
            int transformObjects = 3;
            // Setup some position objects
            Transform[] positions = new Transform[transformObjects];
            // Create some game objects for these transforms
            GameObject[] positionObjects = new GameObject[transformObjects];
            for (int i = 0; i < transformObjects; i++)
            {
                positionObjects[i] = GameObject.Instantiate(new GameObject(),
                    new Vector3((int)UnityEngine.Random.Range(0, 100), (int)UnityEngine.Random.Range(0, 100), (int)UnityEngine.Random.Range(0, 100)),
                    Quaternion.Euler(Vector3.zero));
                positions[i] = positionObjects[i].transform;
            }

            // Add moving platform authoring component
            this.platformObject.AddComponent<MovingPlatformAuthoringComponent>();
            // Set the data for the moving platform authoring component
            var platformAuthoring = this.platformObject.GetComponent<MovingPlatformAuthoringComponent>();
            platformAuthoring.loopMethod = PlatformLooping.CYCLE;
            platformAuthoring.speed = 1;
            platformAuthoring.positions = positions;

            // Convert the game object to an entity
            Entity converted = GameObjectConversionUtility.ConvertGameObjectHierarchy(this.platformObject, this.settings);

            MovingPlatform entitySettings = base.m_Manager.GetComponentData<MovingPlatform>(converted);
            // Assert that settings carry over
            Assert.IsTrue(entitySettings.speed == platformAuthoring.speed);
            Assert.IsTrue(entitySettings.loopMethod == platformAuthoring.loopMethod);
            // Assert that the created object has a buffer of positions
            Assert.IsTrue(base.m_Manager.HasComponent<MovingPlatformTarget>(converted));
            DynamicBuffer<MovingPlatformTarget> targets = base.m_Manager.GetBuffer<MovingPlatformTarget>(converted);
            // Assert that the targets are generated correctly
            for (int i = 0; i < transformObjects; i++)
            {
                Assert.IsTrue(math.all(new float3(positions[i].position) == targets[i].target));
            }

            // Cleanup the created objects
            foreach (GameObject positionObj in positionObjects)
            {
                GameObject.DestroyImmediate(positionObj);
            }
        }

        [Test]
        public void VerifyCreatePlatformNoTargets()
        {
            // Number of transform objects
            int transformObjects = 0;
            // Setup some position objects
            Transform[] positions = new Transform[transformObjects];

            // Add moving platform authoring component
            this.platformObject.AddComponent<MovingPlatformAuthoringComponent>();
            // Set the data for the moving platform authoring component
            var platformAuthoring = this.platformObject.GetComponent<MovingPlatformAuthoringComponent>();
            platformAuthoring.loopMethod = PlatformLooping.REVERSE;
            platformAuthoring.speed = 10;
            platformAuthoring.positions = positions;

            // Convert the game object to an entity
            Entity converted = GameObjectConversionUtility.ConvertGameObjectHierarchy(this.platformObject, this.settings);

            MovingPlatform entitySettings = base.m_Manager.GetComponentData<MovingPlatform>(converted);
            // Assert that settings carry over
            Assert.IsTrue(entitySettings.speed == platformAuthoring.speed);
            Assert.IsTrue(entitySettings.loopMethod == platformAuthoring.loopMethod);
            // Assert that the created object has a buffer of positions
            Assert.IsTrue(base.m_Manager.HasComponent<MovingPlatformTarget>(converted));
            DynamicBuffer<MovingPlatformTarget> targets = base.m_Manager.GetBuffer<MovingPlatformTarget>(converted);
            Assert.IsTrue(targets.Length == 0);
        }
    }
}