using NUnit.Framework;
using PropHunt.Authoring;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using UnityEngine;

namespace PropHunt.EditMode.Tests.Authoring
{
    /// <summary>
    /// Tests for the door authoring component authorining component operation
    /// </summary>
    [TestFixture]
    public class DoorAuthoringComponentTests : ECSTestsFixture
    {
        private GameObject doorObject;

        private GameObjectConversionSettings settings;

        private BlobAssetStore assetStore;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.doorObject = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.Euler(Vector3.zero));
            this.assetStore = new BlobAssetStore();
            this.settings = GameObjectConversionSettings.FromWorld(base.World, assetStore);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            // Cleanup platform object
            GameObject.DestroyImmediate(this.doorObject);
            assetStore.Dispose();
        }

        [Test]
        public void VerifyCreatePlatformSettings()
        {
            // Create some game objects for these transforms
            GameObject openingGameObject = new GameObject();
            GameObject closingGameObject = new GameObject();

            // Set some filler data
            openingGameObject.transform.position = new Vector3(0, 10, 0);
            openingGameObject.transform.position = new Vector3(0, 5, 0);
            openingGameObject.transform.rotation = Quaternion.Euler(0, 30, 0);
            openingGameObject.transform.rotation = Quaternion.Euler(0, 90, 0);

            // Add moving platform authoring component
            DoorAuthoringComponent doorAuthoring = this.doorObject.AddComponent<DoorAuthoringComponent>();
            doorAuthoring.openedState = openingGameObject.transform;
            doorAuthoring.closedState = openingGameObject.transform;
            doorAuthoring.transitionTime = 3.0f;
            doorAuthoring.startingState = DoorState.Opened;

            // Convert the game object to an entity
            Entity converted = GameObjectConversionUtility.ConvertGameObjectHierarchy(this.doorObject, this.settings);
            Door doorComponent = base.m_Manager.GetComponentData<Door>(converted);
            Assert.IsTrue(math.all(doorComponent.closedPosition == new float3(closingGameObject.transform.position)));
            Assert.IsTrue(math.all(doorComponent.openedPosition == new float3(openingGameObject.transform.position)));
            Assert.IsTrue(math.all(doorComponent.closedRotation == new float3(closingGameObject.transform.rotation.eulerAngles)));
            Assert.IsTrue(math.all(doorComponent.openedRotation == new float3(openingGameObject.transform.rotation.eulerAngles)));
            Assert.IsTrue(doorAuthoring.transitionTime == 3.0f);

            // Cleanup objects
            GameObject.Destroy(openingGameObject);
            GameObject.Destroy(closingGameObject);
        }
    }
}