using NUnit.Framework;
using PropHunt.Client.Systems;
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.NetCode;

namespace PropHunt.EditMode.Tests.Client
{
    [TestFixture]
    public class JoinGameSystemTests : ECSTestsFixture
    {
        private JoinGameClientSystem joinGameClientSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.joinGameClientSystem = base.World.CreateSystem<JoinGameClientSystem>();
        }

        [Test]
        public void NoActionWithoutEntity()
        {
            this.joinGameClientSystem.Update();

            // Verify that there is no JoinGameRequest object
            int numRequests = base.m_Manager.CreateEntityQuery(typeof(JoinGameRequest)).CalculateEntityCount();
            Assert.IsTrue(numRequests == 0);
        }

        [Test]
        public void VerifyActionWithEntity()
        {
            // Create NetworkIdComponent without StreamInGame
            Entity newtorkId = base.m_Manager.CreateEntity(typeof(NetworkIdComponent));

            this.joinGameClientSystem.Update();

            // Verify that there is a JoinGameRequest object
            base.World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>().Update();
            int numRequests = base.m_Manager.CreateEntityQuery(typeof(JoinGameRequest)).CalculateEntityCount();
            Assert.IsTrue(base.m_Manager.HasComponent<NetworkStreamInGame>(newtorkId));
            Assert.IsTrue(numRequests == 1);
        }

        [Test]
        public void VerifyMaterialUpdateComponent()
        {
            // Verify that entities with MaterialIdComponent data receive UpdateMaterialComponentData
            // Create some MaterialIdComponentDataEntities
            Entity materialId1 = base.m_Manager.CreateEntity(typeof(MaterialIdComponentData));
            Entity materialId2 = base.m_Manager.CreateEntity(typeof(MaterialIdComponentData));
            Entity materialId3 = base.m_Manager.CreateEntity(typeof(MaterialIdComponentData));

            this.joinGameClientSystem.Update();

            // Verify that entities have the UpdateMarerialComponentDataComponent
            Assert.IsTrue(base.m_Manager.HasComponent<UpdateMaterialComponentData>(materialId1));
            Assert.IsTrue(base.m_Manager.HasComponent<UpdateMaterialComponentData>(materialId2));
            Assert.IsTrue(base.m_Manager.HasComponent<UpdateMaterialComponentData>(materialId3));
        }
    }
}