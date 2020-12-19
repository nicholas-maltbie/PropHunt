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
    public class KCCMovementTests : ECSTestsFixture
    {
        /// <summary>
        /// System for parsing player movement
        /// </summary>
        private KCCMovementSystem kccMovementSystem;

        /// <summary>
        /// Mock of prediction state controller
        /// </summary>
        private Mock<IPredictionState> predictionStateMock;

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
            this.kccMovementSystem = World.CreateSystem<KCCMovementSystem>();
            this.buildPhysicsWorld = base.World.CreateSystem<BuildPhysicsWorld>();

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1f);
            this.predictionStateMock = new Mock<IPredictionState>();
            this.predictionStateMock.Setup(e => e.GetPredictingTick(It.IsAny<Unity.Entities.World>())).Returns(1u);

            // Setup network stream in game component
            base.m_Manager.CreateEntity(typeof(NetworkStreamInGame));

            // Connect mocked variables to system
            this.kccMovementSystem.unityService = this.unityServiceMock.Object;
            this.kccMovementSystem.predictionManager = this.predictionStateMock.Object;
        }

        /// <summary>
        /// Script to create a test player for the KCCInput system
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer(float3 position, float radius)
        {
            Entity player = PhysicsTestUtils.CreateSphere(base.m_Manager, radius, position, quaternion.Euler(float3.zero), false);

            base.m_Manager.AddComponent<KCCVelocity>(player);
            base.m_Manager.AddComponent<PredictedGhostComponent>(player);
            base.m_Manager.AddComponentData<KCCMovementSettings>(player, new KCCMovementSettings
            {
                moveMaxBounces = 3,
                movePushPower = 10,
                movePushDecay = 1,
                moveAnglePower = 1,
                fallMaxBounces = 3,
                fallPushPower = 10,
                fallPushDecay = 1,
                fallAnglePower = 1,
            });
            base.m_Manager.AddComponentData<KCCGrounded>(player, new KCCGrounded
            {
                groundCheckDistance = 0.1f,
                maxWalkAngle = 30.0f,
                onGround = true,
                distanceToGround = 0.0f,
                angle = 0,
                surfaceNormal = new float3(0, 1, 0),
            });
            base.m_Manager.AddComponentData<KCCGravity>(player, new KCCGravity
            {
                gravityAcceleration = new float3(0, -9.8f, 0)
            });

            return player;
        }

        /// <summary>
        /// Assert that the movement is correct on a flat plane
        /// </summary>
        [Test]
        public void VerifyMovementOnFlatPlane()
        {
            Entity player = CreateTestPlayer(float3.zero, 0.5f);
            float3 moveVelocity = new float3(1, 0, 0);
            float3 worldVelocity = new float3(0, -1, 0);
            base.m_Manager.SetComponentData<KCCVelocity>(player, new KCCVelocity
            {
                playerVelocity = moveVelocity,
                worldVelocity = worldVelocity,
            });

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccMovementSystem.Update();
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;

            float3 delta = endingPosition - startingPosition;
            // Assert that the player has moved move velocity + world velocity
            Assert.IsTrue(TestUtils.WithinErrorRange(delta, moveVelocity + worldVelocity));
        }

        /// <summary>
        /// Assert that the movement is sloped up on a sloped plane
        /// </summary>
        [Test]
        public void VerifyMovementOnSlopedPlane()
        {
            Entity player = CreateTestPlayer(float3.zero, 0.5f);
            float3 moveVelocity = new float3(1, 0, 0);
            float3 worldVelocity = new float3(0, -1, 0);
            base.m_Manager.SetComponentData<KCCVelocity>(player, new KCCVelocity
            {
                playerVelocity = moveVelocity,
                worldVelocity = worldVelocity,
            });
            KCCGrounded grounded = base.m_Manager.GetComponentData<KCCGrounded>(player);
            float3 surfaceNormal = math.normalize(new float3(0, 1, 1));
            grounded.surfaceNormal = surfaceNormal;
            base.m_Manager.SetComponentData<KCCGrounded>(player, grounded);

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccMovementSystem.Update();
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;

            float3 delta = endingPosition - startingPosition;
            // Assert that the player has moved move velocity + world velocity
            Assert.IsTrue(TestUtils.WithinErrorRange(delta, KCCUtils.ProjectVectorOntoPlane(moveVelocity, surfaceNormal) + worldVelocity));
        }

        /// <summary>
        /// Assert that the movement is sloped up on a sloped plane
        /// </summary>
        [Test]
        public void VerifyMovementFalling()
        {
            Entity player = CreateTestPlayer(float3.zero, 0.5f);
            float3 moveVelocity = new float3(1, 0, 0);
            float3 worldVelocity = new float3(0, -1, 0);
            base.m_Manager.SetComponentData<KCCVelocity>(player, new KCCVelocity
            {
                playerVelocity = moveVelocity,
                worldVelocity = worldVelocity,
            });
            KCCGrounded grounded = base.m_Manager.GetComponentData<KCCGrounded>(player);
            float3 surfaceNormal = math.normalize(new float3(0, 1, 1));
            grounded.surfaceNormal = surfaceNormal;
            grounded.onGround = false;
            base.m_Manager.SetComponentData<KCCGrounded>(player, grounded);

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccMovementSystem.Update();
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;

            float3 delta = endingPosition - startingPosition;
            // Assert that the player has moved move velocity + world velocity
            Assert.IsTrue(TestUtils.WithinErrorRange(delta, moveVelocity + worldVelocity));
        }
    }
}