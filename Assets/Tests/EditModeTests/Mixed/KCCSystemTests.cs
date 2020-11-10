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
using PropHunt.EditMode.Tests.Utils;
using Unity.Mathematics;

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

        /// <summary>
        /// Current build phyiscs world for test
        /// </summary>
        private BuildPhysicsWorld buildPhysicsWorld;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup kcc input system
            this.kccGroundedSystem = World.CreateSystem<KCCGroundedSystem>();
            this.buildPhysicsWorld = base.World.CreateSystem<BuildPhysicsWorld>();

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();

            // Connect mocked variables ot system
            this.kccGroundedSystem.unityService = this.unityServiceMock.Object;
        }

        /// <summary>
        /// Script to create a test player for the KCCInput system
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer(float3 position, float radius)
        {
            Entity player = PhysicsTestUtils.CreateSphere(base.m_Manager, radius, position, quaternion.Euler(float3.zero), false);
            base.m_Manager.AddComponent<KCCGrounded>(player);
            base.m_Manager.AddComponent<KCCGravity>(player);

            return player;
        }

        [Test]
        public void TestGroundedNoFloor()
        {
            Entity player = CreateTestPlayer(new float3(0, 1, 0), 1.0f);

            this.buildPhysicsWorld.Update();
            this.kccGroundedSystem.Update();
        }

        [Test]
        public void TestGroundedWithFloor()
        {
            Entity player = CreateTestPlayer(new float3(0, 1, 0), 1.0f);
            Entity floor = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), float3.zero, 0, quaternion.Euler(float3.zero), false);

            this.buildPhysicsWorld.Update();
            this.kccGroundedSystem.Update();
        }
    }
}