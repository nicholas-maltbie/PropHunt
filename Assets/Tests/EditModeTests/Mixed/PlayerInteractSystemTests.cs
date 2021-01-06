using Assets.Tests.EditModeTests.Utils;
using Moq;
using NUnit.Framework;
using PropHunt.EditMode.Tests.Utils;
using PropHunt.InputManagement;
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Systems;
using PropHunt.Mixed.Utils;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.EditMode.Tests.Mixed
{
    /// <summary>
    /// Tests for the player interact system system
    /// </summary>
    [TestFixture]
    public class PlayerInteractSystemTests : ECSTestsFixture
    {
        public PlayerInteractSystem playerinteractSystem;

        private Mock<IPredictionState> predictionStateMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup kcc input system
            this.playerinteractSystem = World.CreateSystem<PlayerInteractSystem>();

            // Setup mocks for system
            var unityServiceMock = new Mock<IUnityService>();
            this.predictionStateMock = new Mock<IPredictionState>();
            unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

            // Setup the network stream in game component
            base.m_Manager.CreateEntity(typeof(NetworkStreamInGame));

            // Connect mocked variables to system
            this.playerinteractSystem.unityService = unityServiceMock.Object;
            this.playerinteractSystem.predictionManager = this.predictionStateMock.Object;
        }

        private Entity CreateTestPlayer(float3 position, bool shouldPredict = true)
        {
            Entity player = PhysicsTestUtils.CreateSphere(base.m_Manager, 1.0f, position, quaternion.Euler(float3.zero), false);
            base.m_Manager.AddComponentData<PredictedGhostComponent>(player, new PredictedGhostComponent { PredictionStartTick = shouldPredict ? 0u : 1u });
            base.m_Manager.AddComponentData<FocusDetection>(player, new FocusDetection
            {
                focusDistance = 1.0f,
                focusOffset = 0.0f,
                focusRadius = 0.05f,
            });
            base.m_Manager.AddBuffer<PlayerInput>(player);

            return player;
        }

        [Test]
        public void VerifyInteractWithoutPredict()
        {
            Entity player = CreateTestPlayer(float3.zero, false);
            uint currentTick = 1;
            // Setup mocked behaviour to permit predicting for this player
            this.predictionStateMock.Setup(e => e.GetPredictingTick(It.IsAny<World>())).Returns(currentTick);
            InputUtils.AddInput(base.m_Manager, player, currentTick: currentTick);

            this.playerinteractSystem.Update();
        }

        [Test]
        public void VerifyInteractWithNothing()
        {
            Entity player = CreateTestPlayer(float3.zero, true);
            uint currentTick = 1;
            // Setup mocked behaviour to permit predicting for this player
            this.predictionStateMock.Setup(e => e.GetPredictingTick(It.IsAny<World>())).Returns(currentTick);
            InputUtils.AddInput(base.m_Manager, player, interact : 1, currentTick: currentTick);

            this.playerinteractSystem.Update();
        }

        [Test]
        public void VerifyInteractWithObject()
        {
            Entity player = CreateTestPlayer(float3.zero, true);
            Entity focus = base.m_Manager.CreateEntity(typeof(Interactable));
            base.m_Manager.SetComponentData<FocusDetection>(player, new FocusDetection { lookObject = focus });
            uint currentTick = 1;
            // Setup mocked behaviour to permit predicting for this player
            this.predictionStateMock.Setup(e => e.GetPredictingTick(It.IsAny<World>())).Returns(currentTick);
            InputUtils.AddInput(base.m_Manager, player, interact : 1, currentTick: currentTick);

            this.playerinteractSystem.Update();
            
            // Assert that the object is interacted
            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(focus).interacted == true);
        }
    }
}