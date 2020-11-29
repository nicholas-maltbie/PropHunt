using NUnit.Framework;
using PropHunt.Authoring;
using PropHunt.Server.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using UnityEngine;

namespace PropHunt.EditMode.Tests.Authoring
{
    [TestFixture]
    public class RandomizedStateRandomWrapperTests : ECSTestsFixture
    {
        private GameObject randomObject;

        private GameObjectConversionSettings settings;

        private BlobAssetStore assetStore;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.randomObject = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.Euler(Vector3.zero));
            this.assetStore = new BlobAssetStore();
            this.settings = GameObjectConversionSettings.FromWorld(base.World, assetStore);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            // Cleanup platform object
            GameObject.DestroyImmediate(this.randomObject);
            assetStore.Dispose();
        }

        [Test]
        public void CreateRandomizedStateSpawner()
        {
            this.randomObject.AddComponent<RandomizedStateRandomWrapper>();
            Entity converted = GameObjectConversionUtility.ConvertGameObjectHierarchy(this.randomObject, this.settings);
            Assert.IsTrue(base.m_Manager.HasComponent<RandomWrapper>(converted));
        }
    }
}