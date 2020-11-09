using NUnit.Framework;
using PropHunt.Mixed.Systems;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Moq;
using PropHunt.InputManagement;
using PropHunt.Mixed.Commands;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Physics;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class KCCGroundedSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for parsing player input to KCC movement
        /// </summary>
        private KCCGroundedSystem kccGroundedSystem;

        /// <summary>
        /// Mock of unity service for managing delta time in a testable manner
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup kcc input system
            this.kccGroundedSystem = World.CreateSystem<KCCGroundedSystem>();

            Mock<BuildPhysicsWorld> physicsWorldMock = new Mock<BuildPhysicsWorld>();

            base.World.AddSystem<BuildPhysicsWorld>(physicsWorldMock.Object);

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();

            // Connect mocked variables ot system
            this.kccGroundedSystem.unityService = this.unityServiceMock.Object;
        }

        /// <summary>
        /// Script to create a test player for the KCCInput system
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer()
        {
            Entity player = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<KCCGrounded>(player);
            base.m_Manager.AddComponent<KCCGravity>(player);
            base.m_Manager.AddComponent<PhysicsCollider>(player);
            base.m_Manager.AddComponent<Translation>(player);
            base.m_Manager.AddComponent<Rotation>(player);

            return player;
        }

        [Test]
        public void TestGrounded()
        {
            Entity player = CreateTestPlayer();
            PhysicsWorld mockedPhysicsWorld = new PhysicsWorld();
            CollisionWorld mockedCollisionWorld = new CollisionWorld();
            mockedPhysicsWorld.CollisionWorld = mockedCollisionWorld;

            this.kccGroundedSystem.Update();
        }
    }
}