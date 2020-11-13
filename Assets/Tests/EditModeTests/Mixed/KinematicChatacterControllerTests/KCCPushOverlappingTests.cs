using NUnit.Framework;
using PropHunt.Mixed.Systems;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Moq;
using PropHunt.InputManagement;
using Unity.Physics.Systems;
using Unity.Transforms;
using PropHunt.EditMode.Tests.Utils;
using Unity.Mathematics;
using PropHunt.Tests.Utils;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class KCCPushOverlappingSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for parsing player input to KCC movement
        /// </summary>
        private KCCPushOverlappingSystem kccPushOverlapping;

        /// <summary>
        /// Mock of unity service for managing delta time in a testable manner
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        /// <summary>
        /// Current build physics world for test
        /// </summary>
        private BuildPhysicsWorld buildPhysicsWorld;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup kcc input system
            this.kccPushOverlapping = World.CreateSystem<KCCPushOverlappingSystem>();
            this.buildPhysicsWorld = base.World.CreateSystem<BuildPhysicsWorld>();

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

            // Connect mocked variables ot system
            this.kccPushOverlapping.unityService = this.unityServiceMock.Object;
        }

        /// <summary>
        /// Script to create a test player for the KCCInput system
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer(float3 position, float radius)
        {
            Entity player = PhysicsTestUtils.CreateSphere(base.m_Manager, radius, position, quaternion.Euler(float3.zero), false);
            base.m_Manager.AddComponentData<KCCMovementSettings>(player, new KCCMovementSettings
            {
                // Allow player to be pushed at a rate of 2 unit per second
                maxPush = 2.0f,
            });
            base.m_Manager.AddComponentData<KCCGravity>(player, new KCCGravity { gravityAcceleration = new float3(0, -9.8f, 0) });

            return player;
        }

        /// <summary>
        /// Test to ensure no push when there is no overlap
        /// </summary>
        [Test]
        public void TestNoOverlap()
        {
            Entity player = CreateTestPlayer(new float3(0, 1, 0), 1.0f);

            float3 starting = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccPushOverlapping.Update();
            float3 ending = base.m_Manager.GetComponentData<Translation>(player).Value;

            // ensure no push when there is no overlap
            Assert.IsTrue(TestUtils.WithinErrorRange(starting, ending));
        }

        /// <summary>
        /// Test to ensure no push when there is no overlap but multiple objects
        /// </summary>
        [Test]
        public void TestNoOverlapObjects()
        {
            Entity player = CreateTestPlayer(new float3(0, 0.5f, 0), 0.5f);
            Entity overlap = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), float3.zero, 0, quaternion.Euler(float3.zero), false);
            // No overlap between objects
            base.m_Manager.SetComponentData<Translation>(overlap, new Translation { Value = new float3(0, -1f, 0) });


            float3 starting = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccPushOverlapping.Update();
            float3 ending = base.m_Manager.GetComponentData<Translation>(player).Value;

            // ensure no push when there is no overlap
            Assert.IsTrue(TestUtils.WithinErrorRange(starting, ending));
        }

        /// <summary>
        /// Test to ensure push when there is a big overlap
        /// </summary>
        [Test]
        public void TestBigOverlap()
        {
            Entity player = CreateTestPlayer(new float3(0, 0, 0), 0.5f);
            Entity overlap = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), float3.zero, 0, quaternion.Euler(float3.zero), false);
            // Overlapping by 0.25f units along the y axis with a normal that should be facing upwards
            base.m_Manager.SetComponentData<Translation>(player, new Translation { Value = new float3(0, 0.5f, 0) });
            base.m_Manager.SetComponentData<Translation>(overlap, new Translation { Value = new float3(0, -0.75f, 0) });


            float3 starting = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccPushOverlapping.Update();
            float3 ending = base.m_Manager.GetComponentData<Translation>(player).Value;

            // ensure push delta = (0, 0.25, 0)
            float3 delta = ending - starting;
            UnityEngine.Debug.Log(delta);
            UnityEngine.Debug.Log(starting);
            UnityEngine.Debug.Log(ending);
            Assert.IsTrue(TestUtils.WithinErrorRange(delta, new float3(0, 0.25f, 0), error: 0.01f));
        }

        /// <summary>
        /// Test to ensure push will not exceed cap speed
        /// </summary>
        [Test]
        public void TestCappedSpeed()
        {
            Entity player = CreateTestPlayer(new float3(0, 0, 0), 1.0f);
            base.m_Manager.SetComponentData<KCCMovementSettings>(player, new KCCMovementSettings
            {
                // Allow player to be pushed at a rate of 1 unit per second
                maxPush = 0.1f,
            });
            Entity overlap = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), float3.zero, 0, quaternion.Euler(float3.zero), false);
            // Overlapping by 0.25f units along the y axis with a normal that should be facing upwards
            base.m_Manager.SetComponentData<Translation>(player, new Translation { Value = new float3(0, 0.5f, 0) });
            base.m_Manager.SetComponentData<Translation>(overlap, new Translation { Value = new float3(0, -0.75f, 0) });

            float3 starting = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccPushOverlapping.Update();
            float3 ending = base.m_Manager.GetComponentData<Translation>(player).Value;

            // ensure push delta = (0, 0.1, 0) due to capped speed
            float3 delta = ending - starting;
            Assert.IsTrue(TestUtils.WithinErrorRange(delta, new float3(0, 0.1f, 0), error: 0.01f));

            // Repeat a second time
            starting = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccPushOverlapping.Update();
            ending = base.m_Manager.GetComponentData<Translation>(player).Value;

            // ensure push delta = (0, 0.1, 0) due to capped speed
            delta = ending - starting;
            Assert.IsTrue(TestUtils.WithinErrorRange(delta, new float3(0, 0.1f, 0), error: 0.01f));

            // final push of 0.05 units
            starting = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccPushOverlapping.Update();
            ending = base.m_Manager.GetComponentData<Translation>(player).Value;

            // ensure push delta = (0, 0.05, 0) due to remaining distance
            delta = ending - starting;
            Assert.IsTrue(TestUtils.WithinErrorRange(delta, new float3(0, 0.05f, 0), error: 0.01f));
        }
    }
}