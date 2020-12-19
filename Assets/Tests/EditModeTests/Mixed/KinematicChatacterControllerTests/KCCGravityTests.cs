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
    public class KCCGravitySystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for gravity
        /// </summary>
        private KCCGravitySystem kccGravitySystem;

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

            this.kccGravitySystem = base.World.CreateSystem<KCCGravitySystem>();

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();
            this.predictionStateMock = new Mock<IPredictionState>();
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);
            this.predictionStateMock.Setup(e => e.GetPredictingTick(It.IsAny<Unity.Entities.World>())).Returns(1u);

            // Setup the network stream in game component
            base.m_Manager.CreateEntity(typeof(NetworkStreamInGame));

            // Connect mocked variables to system
            this.kccGravitySystem.unityService = this.unityServiceMock.Object;
            this.kccGravitySystem.predictionManager = this.predictionStateMock.Object;
        }

        /// <summary>
        /// Script to create a test player for the KCCGravity system
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer(float3 position, float radius, bool isGrounded)
        {
            Entity player = PhysicsTestUtils.CreateSphere(base.m_Manager, radius, position, quaternion.Euler(float3.zero), false);
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
                    angle = 0f,
                    maxWalkAngle = 90f
                });
            }
            else
            {
                // In order to simulate a not grounded player, we simply set the bool onGround to be false.
                base.m_Manager.AddComponentData<KCCGrounded>(player, new KCCGrounded { groundCheckDistance = 0.5f, onGround = false, });
            }

            base.m_Manager.AddComponent<PredictedGhostComponent>(player);
            base.m_Manager.AddComponentData<KCCGravity>(player, new KCCGravity { gravityAcceleration = new float3(0, -9.8f, 0) });
            base.m_Manager.AddComponentData<KCCVelocity>(player, new KCCVelocity { playerVelocity = float3.zero, worldVelocity = float3.zero });
            return player;
        }

        /// <summary>
        /// Test to ensure no action when should not predict
        /// </summary>
        [Test]
        public void TestNoPredict()
        {
            Entity player = CreateTestPlayer(new float3(0, 0, 0), 1f, false);
            base.m_Manager.SetComponentData<PredictedGhostComponent>(player, new PredictedGhostComponent { PredictionStartTick = 1u });
            this.kccGravitySystem.Update();
        }

        [Test]
        public void VerifyGravityAppliesIfPlayerIsNotGrounded()
        {
            // Initializes a player with the required components for the gravity system.
            // It initializes world velocity as a float3.zero
            Entity player = CreateTestPlayer(new float3(0, 0, 0), 1f, false);
            // Setup default deltatime value.
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);

            // Update the System.
            this.kccGravitySystem.Update();
            var playerVelocity = m_Manager.GetComponentData<KCCVelocity>(player);
            var kccGravity = m_Manager.GetComponentData<KCCGravity>(player);

            // We expect the gravity to have applied.
            var expectedWorldVelocity = float3.zero + kccGravity.gravityAcceleration;
            Assert.AreEqual(expectedWorldVelocity, playerVelocity.worldVelocity);
        }

        [Test]
        public void VerifyGravityDoesNotApplyIfPlayerIsGrounded()
        {
            // Initializes a player with the required components for the gravity system.
            // It initializes world velocity as a float3.zero
            Entity player = CreateTestPlayer(new float3(0, 0, 0), 1f, true);
            // Setup default deltatime value.
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);

            // Update the System.
            this.kccGravitySystem.Update();
            var playerVelocity = m_Manager.GetComponentData<KCCVelocity>(player);

            // We expect the gravity to not be applied.
            var expectedWorldVelocity = float3.zero;
            Assert.AreEqual(expectedWorldVelocity, playerVelocity.worldVelocity);
        }
    }
}