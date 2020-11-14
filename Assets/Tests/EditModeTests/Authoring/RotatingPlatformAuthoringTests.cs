using NUnit.Framework;
using PropHunt.Authoring;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using UnityEngine;

namespace PropHunt.EditMode.Tests.Authoring
{
    /// <summary>
    /// Tests for the rotating platform authorining component operation
    /// </summary>
    [TestFixture]
    public class RotatingPlatformAuthoringTests : ECSTestsFixture
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
            Transform[] attitudes = new Transform[transformObjects];
            // Create some game objects for these transforms
            GameObject[] positionObjects = new GameObject[transformObjects];
            for (int i = 0; i < transformObjects; i++)
            {
                positionObjects[i] = GameObject.Instantiate(new GameObject(),
                    Vector3.zero,
                    Quaternion.Euler(new Vector3((int)UnityEngine.Random.Range(0, 100), (int)UnityEngine.Random.Range(0, 100), (int)UnityEngine.Random.Range(0, 100))));
                attitudes[i] = positionObjects[i].transform;
            }

            // Add moving platform authoring component
            this.platformObject.AddComponent<RotatingPlatformAuthoringComponent>();
            // Set the data for the moving platform authoring component
            var platformAuthoring = this.platformObject.GetComponent<RotatingPlatformAuthoringComponent>();
            platformAuthoring.loopMethod = PlatformLooping.CYCLE;
            platformAuthoring.speed = 1;
            platformAuthoring.translations = attitudes;

            // Convert the game object to an entity
            Entity converted = GameObjectConversionUtility.ConvertGameObjectHierarchy(this.platformObject, this.settings);

            RotatingPlatform entitySettings = base.m_Manager.GetComponentData<RotatingPlatform>(converted);
            // Assert that settings carry over
            Assert.IsTrue(entitySettings.speed == platformAuthoring.speed);
            Assert.IsTrue(entitySettings.loopMethod == platformAuthoring.loopMethod);
            // Assert that the created object has a buffer of positions
            Assert.IsTrue(base.m_Manager.HasComponent<RotatingPlatformTarget>(converted));
            DynamicBuffer<RotatingPlatformTarget> targets = base.m_Manager.GetBuffer<RotatingPlatformTarget>(converted);
            // Assert that the targets are generated correctly
            for (int i = 0; i < transformObjects; i++)
            {
                Assert.IsTrue(Quaternion.Angle(attitudes[i].rotation, Quaternion.Euler(targets[i].target)) < 0.001f);
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
            Transform[] attitudes = new Transform[transformObjects];

            // Add moving platform authoring component
            this.platformObject.AddComponent<RotatingPlatformAuthoringComponent>();
            // Set the data for the moving platform authoring component
            var platformAuthoring = this.platformObject.GetComponent<RotatingPlatformAuthoringComponent>();
            platformAuthoring.loopMethod = PlatformLooping.REVERSE;
            platformAuthoring.speed = 10;
            platformAuthoring.translations = attitudes;

            // Convert the game object to an entity
            Entity converted = GameObjectConversionUtility.ConvertGameObjectHierarchy(this.platformObject, this.settings);

            RotatingPlatform entitySettings = base.m_Manager.GetComponentData<RotatingPlatform>(converted);
            // Assert that settings carry over
            Assert.IsTrue(entitySettings.speed == platformAuthoring.speed);
            Assert.IsTrue(entitySettings.loopMethod == platformAuthoring.loopMethod);
            // Assert that the created object has a buffer of positions
            Assert.IsTrue(base.m_Manager.HasComponent<RotatingPlatformTarget>(converted));
            DynamicBuffer<RotatingPlatformTarget> targets = base.m_Manager.GetBuffer<RotatingPlatformTarget>(converted);
            Assert.IsTrue(targets.Length == 0);
        }
    }
}