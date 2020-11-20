using NUnit.Framework;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utilities;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Physics.Systems;
using PropHunt.EditMode.Tests.Utils;
using Unity.Mathematics;
using Unity.Physics;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class KCCUtilsTests : ECSTestsFixture
    {
        /// <summary>
        /// Current build physics world for test
        /// </summary>
        private BuildPhysicsWorld buildPhysicsWorld;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.buildPhysicsWorld = base.World.CreateSystem<BuildPhysicsWorld>();
        }

        /// <summary>
        /// Script to create a test player for the KCCGravity system
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer(float3 position, float radius)
        {
            Entity player = PhysicsTestUtils.CreateSphere(base.m_Manager, radius, position, quaternion.Euler(float3.zero), false);

            return player;
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