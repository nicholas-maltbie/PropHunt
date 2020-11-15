using NUnit.Framework;
using PropHunt.Mixed.Systems;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Transforms;
using Unity.Mathematics;
using Moq;
using PropHunt.InputManagement;
using Unity.NetCode;
using PropHunt.Server.Systems;
using PropHunt.Mixed.Utilities;
using PropHunt.Mixed.Commands;
using Assets.Tests.EditModeTests.Utils;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class PlayerRotationSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for parsing player input to rotation.
        /// </summary>
        private PlayerRotationSystem playerRotationSystem;

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
            this.playerRotationSystem = World.CreateSystem<PlayerRotationSystem>();

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();
            this.predictionStateMock = new Mock<IPredictionState>();

            // Connect mocked variables ot system
            this.playerRotationSystem.predictionManager = this.predictionStateMock.Object;
            this.playerRotationSystem.unityService = this.unityServiceMock.Object;

            // Setup the necessary GhostPredictionSystemGroup
            this.ghostPredGroup = base.World.CreateSystem<GhostPredictionSystemGroup>();
        }

        /// <summary>
        /// Assert player does not update when the is no prediction permitted
        /// </summary>
        [Test]
        public void NoInputNonPredictCharacterCharacter()
        {
            Entity player = this.CreateTestPlayer();
            uint currentTick = 1;
            // Setup mocked behaviour to permit predicting for this player
            this.predictionStateMock.Setup(e => e.GetPredictingTick(It.IsAny<World>())).Returns(currentTick);
            var rotBefore = m_Manager.GetComponentData<Rotation>(player).Value;
            this.playerRotationSystem.Update();
            var rotAfter = m_Manager.GetComponentData<Rotation>(player).Value;


            // Assert that the player velocity is zero
            Assert.AreEqual(rotBefore, rotAfter);
        }

        [Test]
        public void VerifyPlayerRotatesAccordingToInput()
        {
            var player = this.CreateTestPlayer();
            // Verify that a buffer is removed from a pushed entity
            uint currentTick = 1;
            var targetYaw = 30.0f;
            // Setup mocked behaviour to permit predicting for this player
            this.predictionStateMock.Setup(e => e.GetPredictingTick(It.IsAny<World>())).Returns(currentTick);
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

            // Add input to player.
            InputUtils.AddInput(base.m_Manager, player, currentTick, vertMove: 0.0f, targetPitch: 30.0f, targetYaw: targetYaw, jump: 1);

            this.playerRotationSystem.Update();

            //rot.Value.value = quaternion.Euler(new float3(0, math.radians(view.yaw), 0)).value;
            var expectedRotation = quaternion.Euler(new float3(0, math.radians(targetYaw), 0));

            // Assert Player View updates accordingly
            Assert.AreEqual(base.m_Manager.GetComponentData<PlayerView>(player).yaw, targetYaw);

            // Assert the player rotated as it should have.
            Assert.AreEqual(base.m_Manager.GetComponentData<Rotation>(player).Value, expectedRotation);
        }

        /// <summary>
        /// Script to create a test player for the Rotation System.
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer()
        {
            Entity player = base.m_Manager.CreateEntity();
            base.m_Manager.AddBuffer<PlayerInput>(player);
            base.m_Manager.AddComponent<Rotation>(player);
            base.m_Manager.AddComponent<PlayerId>(player);
            base.m_Manager.AddComponentData(player, new PredictedGhostComponent{
                PredictionStartTick = 0, // This will make Should Predict return true.
            });
            base.m_Manager.AddComponent<PlayerView>(player);

            return player;
        }
    }
}