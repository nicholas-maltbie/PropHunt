using Moq;
using NUnit.Framework;
using PropHunt.EditMode.Tests.Utils;
using PropHunt.InputManagement;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Systems;
using PropHunt.Mixed.Utils;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics.Systems;

namespace PropHunt.EditMode.Tests.Mixed
{
    /// <summary>
    /// Tests for the KCC Components
    /// </summary>
    [TestFixture]
    public class FocusDetectionSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Focus detection system for detecting what a player is looking at
        /// </summary>
        private FocusDetectionSystem focusDetectionSystem;

        private BuildPhysicsWorld buildPhysicsWorld;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup kcc input system
            this.focusDetectionSystem = World.CreateSystem<FocusDetectionSystem>();
            this.buildPhysicsWorld = base.World.CreateSystem<BuildPhysicsWorld>();

            // Setup mocks for system
            var unityServiceMock = new Mock<IUnityService>();
            var predictionStateMock = new Mock<IPredictionState>();
            unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

            // Setup the network stream in game component
            base.m_Manager.CreateEntity(typeof(NetworkStreamInGame));

            // Connect mocked variables to system
            this.focusDetectionSystem.unityService = unityServiceMock.Object;
            this.focusDetectionSystem.predictionManager = predictionStateMock.Object;
        }

        private Entity CreateTestPlayer(float3 position, float pitch, float yaw, bool shouldPredict = true)
        {
            Entity player = PhysicsTestUtils.CreateSphere(base.m_Manager, 1.0f, position, quaternion.Euler(float3.zero), false);
            base.m_Manager.AddComponentData<PlayerView>(player, new PlayerView { pitch = pitch, yaw = yaw });
            base.m_Manager.AddComponentData<PredictedGhostComponent>(player, new PredictedGhostComponent { PredictionStartTick = shouldPredict ? 0u : 1u });
            base.m_Manager.AddComponentData<FocusDetection>(player, new FocusDetection
            {
                focusDistance = 1.0f,
                focusOffset = 0.0f,
                focusRadius = 0.05f,
            });

            return player;
        }

        /// <summary>
        /// Test to verify nothing happens when there should be no prediction
        /// </summary>
        [Test]
        public void TestNoPrediction()
        {
            // Create the test player
            Entity player = CreateTestPlayer(float3.zero, 0, 0, false);
            // Object for the player to look at
            Entity focusObject = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), new float3(0, 0, 1), float3.zero, 0, quaternion.Euler(float3.zero), false);

            this.buildPhysicsWorld.Update();
            this.focusDetectionSystem.Update();

            FocusDetection focusComponent = base.m_Manager.GetComponentData<FocusDetection>(player);
            Assert.IsTrue(focusComponent.lookObject == Entity.Null);
            Assert.IsTrue(focusComponent.lookDistance == 0);
        }

        /// <summary>
        /// Test to verify player focus change when the player is looking at something
        /// </summary>
        [Test]
        public void TestViewObject()
        {
            // Create the test player
            Entity player = CreateTestPlayer(float3.zero, 0, 0, true);
            // Object for the player to look at
            Entity focusObject = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), new float3(0, 0, 1), float3.zero, 0, quaternion.Euler(float3.zero), false);

            this.buildPhysicsWorld.Update();
            this.focusDetectionSystem.Update();

            FocusDetection focusComponent = base.m_Manager.GetComponentData<FocusDetection>(player);
            Assert.IsTrue(focusComponent.lookObject.Index == focusObject.Index);
            Assert.IsTrue(focusComponent.lookDistance > 0);
        }

        /// <summary>
        /// Test to verify player focus when the player is looking at nothing
        /// </summary>
        [Test]
        public void TestViewingNothing()
        {
            // Create the test player
            Entity player = CreateTestPlayer(float3.zero, 0, 0, true);

            this.buildPhysicsWorld.Update();
            this.focusDetectionSystem.Update();

            FocusDetection focusComponent = base.m_Manager.GetComponentData<FocusDetection>(player);
            UnityEngine.Debug.Log(focusComponent.lookDistance);
            Assert.IsTrue(focusComponent.lookObject == Entity.Null);
            Assert.IsTrue(focusComponent.lookDistance == -1);
        }
    }
}