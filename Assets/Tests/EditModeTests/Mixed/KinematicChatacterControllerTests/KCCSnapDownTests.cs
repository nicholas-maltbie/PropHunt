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
using PropHunt.Mixed.Utils;
using Unity.NetCode;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class KCCSnapDownSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for parsing player input to KCC movement
        /// </summary>
        private KCCSnapDown kccSnapDown;

        /// <summary>
        /// Mock of unity service for managing delta time in a testable manner
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        /// <summary>
        /// Mock of prediction state controller
        /// </summary>
        private Mock<IPredictionState> predictionStateMock;

        /// <summary>
        /// Current build physics world for test
        /// </summary>
        private BuildPhysicsWorld buildPhysicsWorld;

        /// <summary>
        /// Speed to snap player to ground in units per second
        /// </summary>
        private float snapDownSpeed = 5.0f;

        /// <summary>
        /// Distance that the player can snap down to the ground
        /// </summary>
        private float snapDownOffset = 1.0f;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup kcc input system
            this.kccSnapDown = World.CreateSystem<KCCSnapDown>();
            this.buildPhysicsWorld = base.World.CreateSystem<BuildPhysicsWorld>();

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1f);
            this.predictionStateMock = new Mock<IPredictionState>();
            this.predictionStateMock.Setup(e => e.GetPredictingTick(It.IsAny<Unity.Entities.World>())).Returns(1u);

            // Setup network stream in game component
            base.m_Manager.CreateEntity(typeof(NetworkStreamInGame));

            // Connect mocked variables to system
            this.kccSnapDown.unityService = this.unityServiceMock.Object;
            this.kccSnapDown.predictionManager = this.predictionStateMock.Object;
        }

        /// <summary>
        /// Script to create a test player for the KCCInput system
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer(float3 position, float3 size)
        {
            Entity player = PhysicsTestUtils.CreateBox(base.m_Manager, size, position, float3.zero, 0, quaternion.Euler(float3.zero), false);
            base.m_Manager.AddComponent<PredictedGhostComponent>(player);
            base.m_Manager.AddComponent<KCCGrounded>(player);
            base.m_Manager.AddComponent<KCCVelocity>(player);
            base.m_Manager.AddComponentData<KCCGravity>(player, new KCCGravity
            {
                gravityAcceleration = new float3(0, -9.8f, 0)
            });
            base.m_Manager.AddComponentData<KCCMovementSettings>(player, new KCCMovementSettings
            {
                // Distance to snap player down to ground
                snapDownOffset = this.snapDownOffset,
                // Speed at which player is snapped to ground
                snapDownSpeed = this.snapDownSpeed,
            });

            return player;
        }

        /// <summary>
        /// Test to ensure no action when should not predict
        /// </summary>
        [Test]
        public void TestNoPredict()
        {
            Entity player = CreateTestPlayer(new float3(0, 1, 0), 1.0f);
            base.m_Manager.SetComponentData<PredictedGhostComponent>(player, new PredictedGhostComponent { PredictionStartTick = 1u });
            this.buildPhysicsWorld.Update();
            this.kccSnapDown.Update();
        }

        /// <summary>
        /// Test the case of snapping down to the floor a short distance
        /// </summary>
        [Test]
        public void TestSnapShortDistance()
        {
            // Make a player character for this test
            Entity player = CreateTestPlayer(float3.zero, new float3(1, 1, 1));
            // Put a floor below the player
            Entity floor = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), float3.zero, float3.zero, 0, quaternion.Euler(float3.zero), false);
            // Move floor slightly below player so they can snap down to floor
            // gap of 0.1 units between floor and player
            base.m_Manager.SetComponentData<Translation>(player, new Translation { Value = new float3(0, 1f, 0) });
            base.m_Manager.SetComponentData<Translation>(floor, new Translation { Value = new float3(0, -0.1f, 0) });

            // Skip when there is* movement along gravity.Up
            float3 gravityUp = base.m_Manager.GetComponentData<KCCGravity>(player).Up;
            base.m_Manager.SetComponentData<KCCVelocity>(player, new KCCVelocity
            {
                playerVelocity = float3.zero,
                worldVelocity = float3.zero,
            });
            base.m_Manager.SetComponentData<KCCGrounded>(player, new KCCGrounded
            {
                // On the ground
                onGround = true,
                // Distance to ground is less than falling distance
                distanceToGround = 0.0f,
                groundFallingDistance = 1.0f,
            });

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccSnapDown.Update();

            // Assert that the player snapped down by 0.1 units
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            Assert.IsTrue(TestUtils.WithinErrorRange(endingPosition, startingPosition + new float3(0, -0.1f, 0), error: 0.1f));
        }

        /// <summary>
        /// Test the case of snapping down to the floor a long distance.
        /// Should only move an amount less than or equal to the speed * delta time.
        /// </summary>
        [Test]
        public void TestSnapLongDistance()
        {
            float sampleDeltaTime = 0.1f;
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(sampleDeltaTime);
            // Make a player character for this test
            Entity player = CreateTestPlayer(float3.zero, new float3(1, 1, 1));
            // Put a floor below the player
            Entity floor = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), float3.zero, float3.zero, 0, quaternion.Euler(float3.zero), false);
            // Move floor slightly below player so they can snap down to floor
            // gap of 0.75 units between floor and player
            base.m_Manager.SetComponentData<Translation>(player, new Translation { Value = new float3(0, 1f, 0) });
            base.m_Manager.SetComponentData<Translation>(floor, new Translation { Value = new float3(0, -0.75f, 0) });

            // Skip when there is* movement along gravity.Up
            float3 gravityUp = base.m_Manager.GetComponentData<KCCGravity>(player).Up;
            base.m_Manager.SetComponentData<KCCVelocity>(player, new KCCVelocity
            {
                playerVelocity = float3.zero,
                worldVelocity = float3.zero,
            });
            base.m_Manager.SetComponentData<KCCGrounded>(player, new KCCGrounded
            {
                // On the ground
                onGround = true,
                // Distance to ground is less than falling distance
                distanceToGround = 0.0f,
                groundFallingDistance = 1.0f,
            });

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccSnapDown.Update();

            // Assert that the player snapped down by 0.1 units
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            float3 delta = endingPosition - startingPosition;
            float maxDistanceSnap = this.snapDownSpeed * sampleDeltaTime;
            UnityEngine.Debug.Log($"Measured delta due to snap: {delta}");
            Assert.IsTrue(math.all(math.abs(delta) <= math.abs(new float3(0, maxDistanceSnap, 0))));
        }

        /// <summary>
        /// Test the case of snapping down when too far from the ground
        /// </summary>
        [Test]
        public void TestSnapTooFarFromGround()
        {
            // Make a player character for this test
            Entity player = CreateTestPlayer(float3.zero, new float3(1, 1, 1));
            // Put a floor below the player
            Entity floor = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), float3.zero, float3.zero, 0, quaternion.Euler(float3.zero), false);
            // Move floor slightly below player so they can snap down to floor
            // gap of 0.75 units between floor and player
            base.m_Manager.SetComponentData<Translation>(player, new Translation { Value = new float3(0, 1f, 0) });
            base.m_Manager.SetComponentData<Translation>(floor, new Translation { Value = new float3(0, -3f, 0) });

            // Skip when there is* movement along gravity.Up
            float3 gravityUp = base.m_Manager.GetComponentData<KCCGravity>(player).Up;
            base.m_Manager.SetComponentData<KCCVelocity>(player, new KCCVelocity
            {
                playerVelocity = float3.zero,
                worldVelocity = float3.zero,
            });
            base.m_Manager.SetComponentData<KCCGrounded>(player, new KCCGrounded
            {
                // On the ground
                onGround = true,
                // Distance to ground is less than falling distance
                distanceToGround = 0.0f,
                groundFallingDistance = 1.0f,
            });

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.buildPhysicsWorld.Update();
            this.kccSnapDown.Update();

            // Assert that the player did not snap down onto the floor
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            Assert.IsTrue(TestUtils.WithinErrorRange(startingPosition, endingPosition, error: 0.1f));
        }

        /// <summary>
        /// Test the cases when the snap down should be skipped
        /// </summary>
        [Test]
        public void TestSkipSnap()
        {
            // Make a player character for this test
            Entity player = CreateTestPlayer(new float3(0, 0, 0), 0.5f);
            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            // Put a floor below the player
            Entity floor = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), float3.zero, float3.zero, 0, quaternion.Euler(float3.zero), false);
            // Move floor slightly below player so they can snap down to floor
            // gap of 0.1 units between floor and player
            base.m_Manager.SetComponentData<Translation>(player, new Translation { Value = float3.zero });
            base.m_Manager.SetComponentData<Translation>(floor, new Translation { Value = new float3(0, -0.6f, 0) });

            // Skip when there is* movement along gravity.Up
            float3 gravityUp = base.m_Manager.GetComponentData<KCCGravity>(player).Up;
            base.m_Manager.SetComponentData<KCCVelocity>(player, new KCCVelocity
            {
                playerVelocity = gravityUp,
                worldVelocity = gravityUp
            });

            this.buildPhysicsWorld.Update();
            this.kccSnapDown.Update();

            // Assert that the player did not snap down onto the floor
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            Assert.IsTrue(TestUtils.WithinErrorRange(startingPosition, endingPosition));

            // Skip when there is !grounded.Standing onGround
            base.m_Manager.SetComponentData<KCCVelocity>(player, new KCCVelocity
            {
                playerVelocity = float3.zero,
                worldVelocity = float3.zero
            });
            // Make the StandingOnGround function return false
            base.m_Manager.SetComponentData<KCCGrounded>(player, new KCCGrounded
            {
                onGround = false
            });

            this.buildPhysicsWorld.Update();
            this.kccSnapDown.Update();

            // Assert that the player did not snap down onto the floor
            endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            Assert.IsTrue(TestUtils.WithinErrorRange(startingPosition, endingPosition));
        }
    }

}