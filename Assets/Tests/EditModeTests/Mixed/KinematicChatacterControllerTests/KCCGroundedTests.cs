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
        /// Current build physics world for test
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
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

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
            base.m_Manager.AddComponentData<KCCGrounded>(player, new KCCGrounded { groundCheckDistance = 0.5f, maxWalkAngle = 30.0f });
            base.m_Manager.AddComponentData<KCCGravity>(player, new KCCGravity { gravityAcceleration = new float3(0, -9.8f, 0) });

            return player;
        }

        /// <summary>
        /// Verify constants for KCC Grounded
        /// </summary>
        [Test]
        public void TestConstantsKCCGrounded()
        {
            float value = KCCGroundedSystem.MaxAngleFallDegrees;
            Assert.IsTrue(value > 0);
        }

        /// <summary>
        /// Test to ensure player is falling with no ground
        /// </summary>
        [Test]
        public void TestGroundedNoFloor()
        {
            Entity player = CreateTestPlayer(new float3(0, 1, 0), 1.0f);

            this.buildPhysicsWorld.Update();
            this.kccGroundedSystem.Update();

            KCCGrounded grounded = base.m_Manager.GetComponentData<KCCGrounded>(player);
            Assert.IsFalse(grounded.onGround);
            Assert.IsTrue(grounded.distanceToGround == -1);
            Assert.IsTrue(grounded.angle == -1);
            Assert.IsTrue(grounded.groundedRBIndex == -1);
            Assert.IsTrue(math.all(grounded.groundedPoint == float3.zero));
            Assert.IsTrue(grounded.hitEntity == Entity.Null);
            Assert.IsTrue(math.all(grounded.surfaceNormal == float3.zero));
            Assert.IsTrue(grounded.elapsedFallTime == 1.0f);
        }

        /// <summary>
        /// Test to ensure player is not falling when not on ground
        /// </summary>
        [Test]
        public void TestGroundedWithFloor()
        {
            // Test with a flat floor
            Entity player = CreateTestPlayer(new float3(0, 0, 0), 0.5f);
            Entity floor = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), float3.zero, 0, quaternion.Euler(float3.zero), false);
            base.m_Manager.SetComponentData<Translation>(floor, new Translation { Value = new float3(0, -0.5f, 0) });

            this.buildPhysicsWorld.Update();
            this.kccGroundedSystem.Update();

            KCCGrounded grounded = base.m_Manager.GetComponentData<KCCGrounded>(player);
            UnityEngine.Debug.Log(grounded.angle);
            Assert.IsTrue(grounded.onGround);
            Assert.IsTrue(grounded.distanceToGround >= 0);
            Assert.IsTrue(grounded.angle != -1);
            Assert.IsTrue(grounded.hitEntity == floor);
            Assert.IsTrue(grounded.elapsedFallTime == 0);

            float flatAngle = grounded.angle;

            // Test with a sloped floor
            base.m_Manager.SetComponentData<Rotation>(floor, new Rotation { Value = quaternion.Euler(60, 0, 0) });
            base.m_Manager.SetComponentData<Translation>(floor, new Translation { Value = new float3(0, -1.25f, 0.8f) });

            this.buildPhysicsWorld.Update();
            this.kccGroundedSystem.Update();

            grounded = base.m_Manager.GetComponentData<KCCGrounded>(player);
            UnityEngine.Debug.Log(grounded.angle);
            Assert.IsTrue(grounded.onGround);
            Assert.IsTrue(grounded.distanceToGround >= 0);
            Assert.IsTrue(grounded.Falling);
            Assert.IsTrue(grounded.angle != -1);
            Assert.IsTrue(grounded.angle > flatAngle);
            Assert.IsTrue(grounded.hitEntity == floor);
            Assert.IsTrue(grounded.elapsedFallTime == 1.0f);
        }
    }

    [TestFixture]
    public class KCCGravitySystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for gravity
        /// </summary>
        private KCCGravitySystem gravitySystem;

        /// <summary>
        /// Mock of unity service for managing delta time in a testable manner
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.gravitySystem = base.World.CreateSystem<KCCGravitySystem>();

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();
            this.gravitySystem.unityService = unityServiceMock.Object;
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

            base.m_Manager.AddComponentData<KCCGravity>(player, new KCCGravity { gravityAcceleration = new float3(0, -9.8f, 0) });
            base.m_Manager.AddComponentData<KCCVelocity>(player, new KCCVelocity { playerVelocity = float3.zero, worldVelocity = float3.zero });
            return player;
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
            this.gravitySystem.Update();
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
            this.gravitySystem.Update();
            var playerVelocity = m_Manager.GetComponentData<KCCVelocity>(player);

            // We expect the gravity to not be applied.
            var expectedWorldVelocity = float3.zero;
            Assert.AreEqual(expectedWorldVelocity, playerVelocity.worldVelocity);
        }
    }
}