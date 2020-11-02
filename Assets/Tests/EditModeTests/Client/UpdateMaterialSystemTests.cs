using Moq;
using NUnit.Framework;
using PropHunt.Client.Systems;
using PropHunt.Mixed;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Rendering;
using UnityEngine;

namespace PropHunt.EditMode.Tests.Client
{
    /// <summary>
    /// Tests for Update Material System tests.
    /// </summary>
    [TestFixture]
    class UpdateMaterialSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System to update material of objects
        /// </summary>
        private UpdateMaterialSystem updateMaterialSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            updateMaterialSystem = base.World.CreateSystem<UpdateMaterialSystem>();
        }

        /// <summary>
        /// Update the materials of objects
        /// </summary>
        [Test]
        public void TestUpdateMaterialOperation()
        {
            Mock<ISharedMaterialLookup> sharedMaterialMock = new Mock<ISharedMaterialLookup>();
            SharedMaterials.Instance = sharedMaterialMock.Object;

            Material mat = new Material(Shader.Find("Shader Graphs/LitDOTS"));
            sharedMaterialMock.Setup(m => m.GetMaterialById(It.IsAny<int>())).Returns(mat);

            Entity entity = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<UpdateMaterialComponentData>(entity);
            base.m_Manager.AddComponent<MaterialIdComponentData>(entity);
            base.m_Manager.AddComponent<UpdateMaterialComponentData>(entity);
            base.m_Manager.AddSharedComponentData<RenderMesh>(entity, new RenderMesh());

            this.updateMaterialSystem.Update();
            EndSimulationEntityCommandBufferSystem buffer = base.World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            buffer.Update();

            Material.DestroyImmediate(mat);

            Assert.IsFalse(base.m_Manager.HasComponent<UpdateMaterialComponentData>(entity));
        }
    }
}