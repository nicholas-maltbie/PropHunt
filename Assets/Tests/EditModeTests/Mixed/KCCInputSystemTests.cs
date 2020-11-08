using NUnit.Framework;
using PropHunt.Mixed.Systems;
using PropHunt.Mixed.Components;
using PropHunt.Tests.Utils;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Moq;
using PropHunt.InputManagement;
using PropHunt.Mixed.Utilities;
using Unity.NetCode;
using PropHunt.Mixed.Commands;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class KCCInputSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for parsing player input to KCC movement
        /// </summary>
        private KinematicCharacterControllerInput kccInputSystem;

        /// <summary>
        /// Mock of prediction state controller
        /// </summary>
        private Mock<IPredictionState> predictionStateMock;

        /// <summary>
        /// Mock of unity service for managing delta time in a testable manner
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        /// <summary>
        /// System for predicting ghosts in mocked ecs test world
        /// </summary>
        private GhostPredictionSystemGroup ghostPredGroup;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup kcc input system
            this.kccInputSystem = World.CreateSystem<KinematicCharacterControllerInput>();

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();
            this.predictionStateMock = new Mock<IPredictionState>();

            // Connect mocked variables ot system
            this.kccInputSystem.predictionManager = this.predictionStateMock.Object;
            this.kccInputSystem.unityServie = this.unityServiceMock.Object;

            // Setup the necessary GhostPredictionSystemGroup
            this.ghostPredGroup = base.World.CreateSystem<GhostPredictionSystemGroup>();
        }

        /// <summary>
        /// Script to create a test player for the KCCInput system
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer()
        {
            Entity player = base.m_Manager.CreateEntity();
            base.m_Manager.AddBuffer<PlayerInput>(player);
            base.m_Manager.AddComponent<KCCVelocity>(player);
            base.m_Manager.AddComponent<KCCJumping>(player);
            base.m_Manager.AddComponent<PredictedGhostComponent>(player);
            base.m_Manager.AddComponent<PlayerView>(player);
            base.m_Manager.AddComponentData<KCCMovementSettings>(player,
                new KCCMovementSettings
                {
                    sprintMultiplier = 2.0f,
                    moveSpeed = 1.0f
                }
            );

            return player;
        }

        /// <summary>
        /// Assert player does not update when the is no prediction permitted
        /// </summary>
        [Test]
        public void NoInputNonPredictCharacterCharacter()
        {
            Entity player = this.CreateTestPlayer();
            // Setup mocked behaviour to not permit predicting for this player
            this.predictionStateMock.Setup(e => e.ShouldPredict(0, It.IsAny<PredictedGhostComponent>())).Returns(false);

            this.kccInputSystem.Update();

            // Assert that the player velocity is zero
            Assert.IsTrue(TestUtils.WithinErrorRange(base.m_Manager.GetComponentData<KCCVelocity>(player).playerVelocity, float3.zero));
        }
        
        /// <summary>
        /// Assert proper update of player given various input states
        /// </summary>
        [Test]
        public void UpdatePlayerVelocityAndJumpState()
        {
            Entity player = this.CreateTestPlayer();
            // First 'data' tick will be tick 1
            uint currentTick = 1;
            // Setup mocked behaviour to permit predicting for this player
            this.predictionStateMock.Setup(e => e.ShouldPredict(currentTick, It.IsAny<PredictedGhostComponent>())).Returns(true);
            this.predictionStateMock.Setup(e => e.GetPredictingTick(It.IsAny<World>())).Returns(currentTick);
            this.unityServiceMock.Setup(e => e.GetDeltaTime()).Returns(1.0f);

            float horizMove = 0.0f;
            float vertMove = 1.0f;
            float targetYaw = 0.0f;
            float targetPitch = 30.0f;
            byte jump = 1;
            byte interact = 1;
            byte sprint = 0;

            DynamicBuffer<PlayerInput> inputBuffer = base.m_Manager.GetBuffer<PlayerInput>(player);
            inputBuffer.Add(new PlayerInput
            {
                tick = currentTick,
                horizMove = horizMove,
                vertMove = vertMove,
                targetYaw = targetYaw,
                targetPitch = targetPitch,
                jump = jump,
                interact = interact,
                sprint = sprint,
            });

            this.kccInputSystem.Update();

            // Assert state is updated correctly
            Assert.IsTrue(TestUtils.WithinErrorRange(base.m_Manager.GetComponentData<KCCVelocity>(player).playerVelocity, new float3(0, 0, 1)));
            Assert.IsTrue(base.m_Manager.GetComponentData<KCCJumping>(player).attemptingJump == true);
        }
    }
}