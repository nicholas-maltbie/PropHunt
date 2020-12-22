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
using PropHunt.Mixed.Utilities;
using Unity.NetCode;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class KCCJumpTests : ECSTestsFixture
    {
        /// <summary>
        /// System for gravity
        /// </summary>
        private KCCGravitySystem gravitySystem;

        /// <summary>
        /// System for jumping
        /// </summary>
        private KCCJumpSystem kccJumpSystem;

        /// <summary>
        /// Mock of prediction state controller
        /// </summary>
        private Mock<IPredictionState> predictionStateMock;

        /// <summary>
        /// Mock of unity service for managing delta time in a testable manner
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.gravitySystem = base.World.CreateSystem<KCCGravitySystem>();
            this.kccJumpSystem = base.World.CreateSystem<KCCJumpSystem>();

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();
            this.predictionStateMock = new Mock<IPredictionState>();
            this.predictionStateMock.Setup(e => e.GetPredictingTick(It.IsAny<Unity.Entities.World>())).Returns(1u);

            // Setup network stream in game component
            base.m_Manager.CreateEntity(typeof(NetworkStreamInGame));

            // Connect mocked variables to system
            this.kccJumpSystem.unityService = this.unityServiceMock.Object;
            this.kccJumpSystem.predictionManager = this.predictionStateMock.Object;
        }

        /// <summary>
        /// Script to create a test player for the KCCGravity system
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer(
            float3 position,
            float radius,
            bool isGrounded,
            bool isAttemptingJump,
            float groundedElapsedFallTime = 0f,
            float jumpGraceTime = 0f,
            float jumpCoolDown = 0f,
            float jumpForce = 0f,
            float timeElapsedSinceLastJump = 0f)
        {
            Entity player = PhysicsTestUtils.CreateSphere(base.m_Manager, radius, position, quaternion.Euler(float3.zero), false);
            base.m_Manager.AddComponentData<PredictedGhostComponent>(player, new PredictedGhostComponent { AppliedTick = 0u });
            if (isGrounded)
            {
                // In order to simulate a Grounded player, we need to cover the following conditions:
                // 1) The bool onGround is true.
                // 2) The distance to the ground, needs to be less than the ground falling distance.
                // 3) The player's angle is not greater than the max walk angle.
                base.m_Manager.AddComponentData<KCCGrounded>(player,
                new KCCGrounded
                {
                    groundCheckDistance = 0.5f,
                    onGround = true,
                    distanceToGround = 0.1f,
                    groundFallingDistance = 0.2f,
                    elapsedFallTime = groundedElapsedFallTime,
                    angle = 0f,
                    maxWalkAngle = 90f
                });
            }
            else
            {
                // In order to simulate a not grounded player, we simply set the bool onGround to be false.
                base.m_Manager.AddComponentData<KCCGrounded>(player, new KCCGrounded { groundCheckDistance = 0.5f, onGround = false, elapsedFallTime = groundedElapsedFallTime });
            }

            base.m_Manager.AddComponentData<KCCGravity>(player, new KCCGravity { gravityAcceleration = new float3(0, -9.8f, 0) });
            base.m_Manager.AddComponentData<KCCVelocity>(player, new KCCVelocity { playerVelocity = float3.zero, worldVelocity = float3.zero });
            base.m_Manager.AddComponentData<KCCJumping>(player,
                new KCCJumping
                {
                    attemptingJump = isAttemptingJump,
                    jumpGraceTime = jumpGraceTime,
                    jumpCooldown = jumpCoolDown,
                    jumpForce = jumpForce,
                    timeElapsedSinceJump = timeElapsedSinceLastJump,
                });

            return player;
        }

        /// <summary>
        /// Test to ensure no action when should not predict
        /// </summary>
        [Test]
        public void TestNoPredict()
        {
            Entity player = CreateTestPlayer(new float3(0, 0, 0), 1f, false, true, jumpForce: 10f, jumpCoolDown: 1f);
            base.m_Manager.SetComponentData<PredictedGhostComponent>(player, new PredictedGhostComponent { PredictionStartTick = 1u });
            this.kccJumpSystem.Update();
        }

        [Test]
        public void VerifyPlayerCantJumpIfNotGrounded()
        {
            // Initializes a player with the required components for the gravity system.
            // It initializes world velocity as a float3.zero
            Entity player = CreateTestPlayer(new float3(0, 0, 0), 1f, false, true, jumpForce: 10f, jumpCoolDown: 1f);
            // Setup default deltatime value.
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);

            // Update the System.
            this.kccJumpSystem.Update();

            var playerJumpData = m_Manager.GetComponentData<KCCJumping>(player);
            var kccGravity = m_Manager.GetComponentData<KCCGravity>(player);
            var playerVelocity = m_Manager.GetComponentData<KCCVelocity>(player);

            // We expect the jump velocity did not apply.
            var expectedWorldVelocity = float3.zero;
            Assert.AreEqual(expectedWorldVelocity, playerVelocity.worldVelocity);

            // We also expect the time elapsed since last jumped will be 1, because that's the delta time we sent.
            var expectedTimeElapsedSinceLastJump = 1f;
            Assert.AreEqual(expectedTimeElapsedSinceLastJump, playerJumpData.timeElapsedSinceJump);
        }

        [Test]
        public void VerifyPlayerCantJumpIfCoolDownGreaterThanTimeElapsedSinceLastJump()
        {
            // Initializes a player with the required components for the gravity system.
            // It initializes world velocity as a float3.zero
            Entity player = CreateTestPlayer(new float3(0, 0, 0), 1f, true, true, jumpForce: 10f, jumpCoolDown: 1f, timeElapsedSinceLastJump: 0.5f);
            // Setup default deltatime value.
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(0.5f);

            // Update the System.
            this.kccJumpSystem.Update();

            var playerJumpData = m_Manager.GetComponentData<KCCJumping>(player);
            var kccGravity = m_Manager.GetComponentData<KCCGravity>(player);
            var playerVelocity = m_Manager.GetComponentData<KCCVelocity>(player);

            // We expect the jump velocity did not apply.
            var expectedWorldVelocity = float3.zero;
            Assert.AreEqual(expectedWorldVelocity, playerVelocity.worldVelocity);

            // We also expect the time elapsed since last jumped will be 1, because that's 0.5f it already had plus 0.5f we sent.
            var expectedTimeElapsedSinceLastJump = 1f;
            Assert.AreEqual(expectedTimeElapsedSinceLastJump, playerJumpData.timeElapsedSinceJump);
        }

        [Test]
        public void VerifyPlayerWontJumpIfNoJumpIsAttempted()
        {
            // Initializes a player with the required components for the gravity system.
            // It initializes world velocity as a float3.zero
            Entity player = CreateTestPlayer(new float3(0, 0, 0), 1f, true, false, jumpForce: 10f, jumpCoolDown: 1f);
            // Setup default deltatime value.
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);

            // Update the System.
            this.kccJumpSystem.Update();

            var playerJumpData = m_Manager.GetComponentData<KCCJumping>(player);
            var kccGravity = m_Manager.GetComponentData<KCCGravity>(player);
            var playerVelocity = m_Manager.GetComponentData<KCCVelocity>(player);

            // We expect the jump velocity did not apply.
            var expectedWorldVelocity = float3.zero;
            Assert.AreEqual(expectedWorldVelocity, playerVelocity.worldVelocity);
        }

        [Test]
        public void VerifyPlayerCanJumpIfGroundedAndHasntJumpedBefore()
        {
            // Initializes a player with the required components for the gravity system.
            // It initializes world velocity as a float3.zero
            Entity player = CreateTestPlayer(
                position: new float3(0, 0, 0),
                radius: 1f,
                isGrounded: true,
                isAttemptingJump: true,
                jumpForce: 10f,
                jumpCoolDown: 1f,
                timeElapsedSinceLastJump: 1f);

            // Setup default deltatime value.
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);

            // Update the System.
            this.kccJumpSystem.Update();

            var playerJumpData = m_Manager.GetComponentData<KCCJumping>(player);
            var kccGravity = m_Manager.GetComponentData<KCCGravity>(player);
            var playerVelocity = m_Manager.GetComponentData<KCCVelocity>(player);

            // We expect the jump velocity did not apply.
            var expectedWorldVelocity = kccGravity.Up * playerJumpData.jumpForce;
            Assert.AreEqual(expectedWorldVelocity, playerVelocity.worldVelocity);
        }

        [Test]
        public void VerifyPlayerCanJumpIfFallingWithinGracePeriod()
        {
            // Initializes a player with the required components for the gravity system.
            // It initializes world velocity as a float3.zero
            Entity player = CreateTestPlayer(
                position: new float3(0, 0, 0),
                radius: 1f,
                isGrounded: true,
                isAttemptingJump: true,
                jumpForce: 10f,
                jumpCoolDown: 1f,
                groundedElapsedFallTime: 0.1f,
                jumpGraceTime: 0.2f,
                timeElapsedSinceLastJump: 1f);
            // Setup default deltatime value.
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);

            // Update the System.
            this.kccJumpSystem.Update();

            var playerJumpData = m_Manager.GetComponentData<KCCJumping>(player);
            var kccGravity = m_Manager.GetComponentData<KCCGravity>(player);
            var playerVelocity = m_Manager.GetComponentData<KCCVelocity>(player);

            // We expect the jump velocity did not apply.
            var expectedWorldVelocity = kccGravity.Up * playerJumpData.jumpForce;
            Assert.AreEqual(expectedWorldVelocity, playerVelocity.worldVelocity);
        }

        [Test]
        public void VerifyPlayerCantJumpIfFallingNotWithinGracePeriod()
        {
            // Initializes a player with the required components for the gravity system.
            // It initializes world velocity as a float3.zero
            Entity player = CreateTestPlayer(
                position: new float3(0, 0, 0),
                radius: 1f,
                isGrounded: true,
                isAttemptingJump: true,
                jumpForce: 10f,
                jumpCoolDown: 1f,
                groundedElapsedFallTime: 0.2f,
                jumpGraceTime: 0.1f,
                timeElapsedSinceLastJump: 1f);

            // Setup default deltatime value.
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);

            // Update the System.
            this.kccJumpSystem.Update();

            var playerJumpData = m_Manager.GetComponentData<KCCJumping>(player);
            var kccGravity = m_Manager.GetComponentData<KCCGravity>(player);
            var playerVelocity = m_Manager.GetComponentData<KCCVelocity>(player);

            // We expect the jump velocity did not apply.
            var expectedWorldVelocity = float3.zero;
            Assert.AreEqual(expectedWorldVelocity, playerVelocity.worldVelocity);
        }
    }
}