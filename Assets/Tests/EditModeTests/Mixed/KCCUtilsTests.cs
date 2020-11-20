using NUnit.Framework;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utilities;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Physics.Systems;
using PropHunt.EditMode.Tests.Utils;
using Unity.Mathematics;
using Unity.Physics;
using PropHunt.Tests.Utils;
using Unity.Transforms;
using PropHunt.Mixed.Systems;
using Moq;
using PropHunt.InputManagement;
using PropHunt.Constants;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class KCCUtilsTests : ECSTestsFixture
    {
        /// <summary>
        /// Current build physics world for test
        /// </summary>
        private BuildPhysicsWorld buildPhysicsWorld;

        /// <summary>
        /// Entity command buffer
        /// </summary>
        private EndSimulationEntityCommandBufferSystem ecbSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.buildPhysicsWorld = base.World.CreateSystem<BuildPhysicsWorld>();
            this.ecbSystem = base.World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        /// <summary>
        /// Script to create a test player for the KCCGravity system
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer(float3 position, float length)
        {
            Entity player = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(length, length, length), position, float3.zero, 0, quaternion.Euler(float3.zero), false);

            base.m_Manager.AddComponent<KCCVelocity>(player);
            base.m_Manager.AddComponent<KCCGravity>(player);
            base.m_Manager.AddComponent<KCCMovementSettings>(player);
            base.m_Manager.AddComponent<KCCGrounded>(player);

            return player;
        }

        [Test]
        public void TestKCCPushKinematicObject()
        {
            Entity player = CreateTestPlayer(float3.zero, 1.0f);
            Entity wall = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(2, 2, 2), float3.zero, float3.zero, 0, quaternion.Euler(0, 45, 0), true);

            base.m_Manager.SetComponentData<Translation>(wall, new Translation { Value = new float3(3, -1, 1) });
            base.m_Manager.SetComponentData<PhysicsMass>(wall, new PhysicsMass { InverseInertia = float3.zero });

            this.buildPhysicsWorld.Update();

            float3 start = float3.zero;
            float3 movement = new float3(5, 0, 0);

            ComponentDataFromEntity<PhysicsMass> pmGetter = this.buildPhysicsWorld.GetComponentDataFromEntity<PhysicsMass>(true);

            // ECB can only be used in job, will make a KCC Movement system for this
            KCCMovementSystem movementSystem = base.World.CreateSystem<KCCMovementSystem>();
            Mock<IUnityService> unityServiceMock = new Mock<IUnityService>();
            unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);
            movementSystem.unityService = unityServiceMock.Object;
            base.m_Manager.SetComponentData<Translation>(player, new Translation { Value = start });
            base.m_Manager.SetComponentData<KCCVelocity>(player, new KCCVelocity { playerVelocity = movement });
            movementSystem.Update();

            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;

            // Ensure it moves somewhat along the movement axis
            Assert.IsTrue(math.dot(movement, endingPosition - start) > 0);

            // Apply the ecb system
            base.World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().Update();
            base.m_Manager.CompleteAllJobs();

            // Assert that the box has some push force applied to it
            Assert.IsFalse(base.m_Manager.HasComponent<PushForce>(wall));
        }

        [Test]
        public void TestKCCPushDynamicObject()
        {
            Entity player = CreateTestPlayer(float3.zero, 1.0f);
            Entity wall = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(2, 2, 2), float3.zero, float3.zero, 0, quaternion.Euler(0, 45, 0), true);

            base.m_Manager.SetComponentData<Translation>(wall, new Translation { Value = new float3(3, -1, 1) });

            this.buildPhysicsWorld.Update();

            float3 start = float3.zero;
            float3 movement = new float3(5, 0, 0);

            ComponentDataFromEntity<PhysicsMass> pmGetter = this.buildPhysicsWorld.GetComponentDataFromEntity<PhysicsMass>(true);

            // ECB can only be used in job, will make a KCC Movement system for this
            KCCMovementSystem movementSystem = base.World.CreateSystem<KCCMovementSystem>();
            Mock<IUnityService> unityServiceMock = new Mock<IUnityService>();
            unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);
            movementSystem.unityService = unityServiceMock.Object;
            base.m_Manager.SetComponentData<Translation>(player, new Translation { Value = start });
            base.m_Manager.SetComponentData<KCCVelocity>(player, new KCCVelocity { playerVelocity = movement });
            movementSystem.Update();

            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;

            // Ensure it moves somewhat along the movement axis
            Assert.IsTrue(math.dot(movement, endingPosition - start) > 0);

            // Apply the ecb system
            base.World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().Update();
            base.m_Manager.CompleteAllJobs();

            // Assert that the box has some push force applied to it
            Assert.IsTrue(base.m_Manager.HasComponent<PushForce>(wall));
            Assert.IsTrue(base.m_Manager.GetBuffer<PushForce>(wall).Length == 1);
        }

        [Test]
        public void TestKCCSnapUpStairsObject()
        {
            Entity player = CreateTestPlayer(float3.zero, 1.0f);
            Entity wall = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(1, 1, 1), float3.zero, new float3(0, 0.5f, 0), 0, quaternion.Euler(0, 0, 0), false);

            base.m_Manager.SetComponentData<Translation>(wall, new Translation { Value = new float3(3, -0.45f, 0) });
            base.m_Manager.SetComponentData<Translation>(player, new Translation { Value = float3.zero });

            this.buildPhysicsWorld.Update();

            float3 start = float3.zero;
            float3 movement = new float3(5, 0, 0);

            ComponentDataFromEntity<PhysicsMass> pmGetter = this.buildPhysicsWorld.GetComponentDataFromEntity<PhysicsMass>(true);
            var cb = this.ecbSystem.CreateCommandBuffer().AsParallelWriter();

            float3 endingPosition = KCCUtils.ProjectValidMovement(
                cb, 0,
                this.buildPhysicsWorld.PhysicsWorld.CollisionWorld,
                start, movement,
                base.m_Manager.GetComponentData<PhysicsCollider>(player), player.Index,
                quaternion.Euler(float3.zero), pmGetter,
                verticalSnapUp: 1f,
                gravityDirection: new float3(0, -1, 0),
                anglePower: 2,
                maxBounces: 3,
                pushPower: 0.25f,
                pushDecay: 0,
                epsilon: KCCConstants.Epsilon
            );

            // Ensure it moves somewhat along the movement axis
            float delta = endingPosition.y - start.y;
            UnityEngine.Debug.Log(endingPosition + " " + start);
            // Assert that delta along y axis is within 0.01 units of 0.05 target jump
            Assert.IsTrue(math.abs(delta - 0.05f) < 0.01f);
        }

        [Test]
        public void TestKCCMovementBounceOffObjects()
        {
            Entity player = CreateTestPlayer(float3.zero, 1.0f);
            Entity wall = PhysicsTestUtils.CreateBox(base.m_Manager, new float3(2, 2, 2), float3.zero, float3.zero, 0, quaternion.Euler(0, 45, 0), false);

            base.m_Manager.SetComponentData<Translation>(wall, new Translation { Value = new float3(3, -1, 1) });

            this.buildPhysicsWorld.Update();

            float3 start = float3.zero;
            float3 movement = new float3(5, 0, 0);

            ComponentDataFromEntity<PhysicsMass> pmGetter = this.buildPhysicsWorld.GetComponentDataFromEntity<PhysicsMass>(true);
            var cb = this.ecbSystem.CreateCommandBuffer().AsParallelWriter();

            float3 endingPosition = KCCUtils.ProjectValidMovement(
                cb, 0,
                this.buildPhysicsWorld.PhysicsWorld.CollisionWorld,
                start, movement,
                base.m_Manager.GetComponentData<PhysicsCollider>(player), player.Index,
                quaternion.Euler(float3.zero), pmGetter,
                verticalSnapUp: 0.1f,
                gravityDirection: new float3(0, -1, 0),
                anglePower: 2,
                maxBounces: 3,
                pushPower: 0.25f,
                pushDecay: 0,
                epsilon: KCCConstants.Epsilon
            );

            // Ensure it moves somewhat along the movement axis
            Assert.IsTrue(math.dot(movement, endingPosition - start) > 0);
        }

        [Test]
        public void TestKCCMovementNoBounces()
        {
            Entity player = CreateTestPlayer(float3.zero, 1.0f);

            this.buildPhysicsWorld.Update();

            float3 start = float3.zero;
            float3 movement = new float3(1, 0, 0);

            ComponentDataFromEntity<PhysicsMass> pmGetter = this.buildPhysicsWorld.GetComponentDataFromEntity<PhysicsMass>(true);
            var cb = this.ecbSystem.CreateCommandBuffer().AsParallelWriter();

            float3 endingPosition = KCCUtils.ProjectValidMovement(
                cb, 0,
                this.buildPhysicsWorld.PhysicsWorld.CollisionWorld,
                start, movement,
                base.m_Manager.GetComponentData<PhysicsCollider>(player), player.Index,
                quaternion.Euler(float3.zero), pmGetter,
                verticalSnapUp: 0.1f,
                gravityDirection: new float3(0, -1, 0),
                anglePower: 2,
                maxBounces: 3,
                pushPower: 0.25f,
                pushDecay: 0,
                epsilon: KCCConstants.Epsilon
            );

            Assert.IsTrue(TestUtils.WithinErrorRange(start + movement, endingPosition));
        }

        [Test]
        public void VerifyIsKinematicObjectsFromPhysicsMass()
        {
            PhysicsMass pm = new PhysicsMass
            {
                InverseInertia = float3.zero
            };

            Assert.IsTrue(KCCUtils.IsKinematic(pm));

            pm.InverseInertia = new float3(1, 1, 1);
            Assert.IsFalse(KCCUtils.IsKinematic(pm));
        }

        [Test]
        public void VerifyHasMovementAlongAxis()
        {
            // return math.dot(velocity.worldVelocity, axis) > 0 || math.dot(velocity.playerVelocity, axis) > 0;
            KCCVelocity velocity = new KCCVelocity { worldVelocity = float3.zero, playerVelocity = float3.zero };
            float3 axis = new float3(0, -1, 0);

            Assert.IsFalse(KCCUtils.HasMovementAlongAxis(velocity, axis));

            velocity.worldVelocity = new float3(-1, -1, 0);
            Assert.IsTrue(KCCUtils.HasMovementAlongAxis(velocity, axis));

            velocity.playerVelocity = new float3(-1, -1, 0);
            Assert.IsTrue(KCCUtils.HasMovementAlongAxis(velocity, axis));

            velocity.worldVelocity = float3.zero;
            Assert.IsTrue(KCCUtils.HasMovementAlongAxis(velocity, axis));

            velocity.playerVelocity = float3.zero;
            Assert.IsFalse(KCCUtils.HasMovementAlongAxis(velocity, axis));

            velocity.playerVelocity = new float3(-1, -1, 0);
            Assert.IsFalse(KCCUtils.HasMovementAlongAxis(velocity, new float3(0, 0, 1)));
        }

        [Test]
        public void VerifyCanJumpBehaviour()
        {
            // return !grounded.Falling && grounded.elapsedFallTime <= jumping.jumpGraceTime && jumping.timeElapsedSinceJump >= jumping.jumpCooldown;
            KCCJumping jumping = new KCCJumping
            {
                jumpGraceTime = 1.0f,
                timeElapsedSinceJump = 10.0f,
                jumpCooldown = 1.0f,
            };
            KCCGrounded grounded = new KCCGrounded
            {
                elapsedFallTime = 0,
                onGround = true,
                angle = 30,
                maxWalkAngle = 45,
                groundFallingDistance = 0.1f,
            };

            Assert.IsTrue(KCCUtils.CanJump(jumping, grounded));

            grounded.onGround = false;
            Assert.IsFalse(KCCUtils.CanJump(jumping, grounded));

            grounded.onGround = true;
            grounded.elapsedFallTime = 100;
            Assert.IsFalse(KCCUtils.CanJump(jumping, grounded));

            grounded.elapsedFallTime = 0;
            jumping.timeElapsedSinceJump = 0;
            Assert.IsFalse(KCCUtils.CanJump(jumping, grounded));
        }
    }
}