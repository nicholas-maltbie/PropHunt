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
using Assets.Tests.EditModeTests.Utils;

namespace PropHunt.EditMode.Tests.Mixed
{
    /// <summary>
    /// Tests for the KCC Components
    /// </summary>
    [TestFixture]
    public class KCCComponentsTests : ECSTestsFixture
    {
        [Test]
        public void TestKCCMovementSettingsSprint()
        {
            KCCMovementSettings movement = new KCCMovementSettings
            {
                moveSpeed = 5.0f,
                sprintMultiplier = 2.0f,
            };

            Assert.IsTrue(movement.SprintSpeed == 10.0f);
            movement.moveSpeed = 1.0f;
            Assert.IsTrue(movement.SprintSpeed == 2.0f);
            movement.sprintMultiplier = 3.0f;
            Assert.IsTrue(movement.SprintSpeed == 3.0f);
        }

        [Test]
        public void TestKCCGravityState()
        {
            KCCGravity gravity = new KCCGravity
            {
                gravityAcceleration = new float3(0, 10, 0)
            };

            Assert.IsTrue(TestUtils.WithinErrorRange(gravity.Down, new float3(0, 1, 0)));
            Assert.IsTrue(TestUtils.WithinErrorRange(gravity.Up, new float3(0, -1, 0)));
        }

        [Test]
        public void TestKCCGroundedState()
        {
            KCCGrounded grounded = new KCCGrounded
            {
                maxWalkAngle = 45.0f,
                groundCheckDistance = 1.0f,
                groundFallingDistance = 0.1f,

                angle = 30.0f,
                distanceToGround = 0.25f,
                onGround = true,

                previousDistanceToGround = 0.35f,
                previousOnGround = true,
                previousAngle = 30.0f
            };

            // State should be falling and not standing on ground
            Assert.IsFalse(grounded.StandingOnGround);
            Assert.IsTrue(grounded.Falling);
            Assert.IsFalse(grounded.PreviousStandingOnGround);
            Assert.IsTrue(grounded.PreviousFalling);

            // State should be not falling and standing on Ground
            grounded.distanceToGround = 0.0f;
            grounded.previousDistanceToGround = 0.0f;
            Assert.IsTrue(grounded.StandingOnGround);
            Assert.IsFalse(grounded.Falling);
            Assert.IsTrue(grounded.PreviousStandingOnGround);
            Assert.IsFalse(grounded.PreviousFalling);

            // State should be falling and not standing on ground
            grounded.angle = 60.0f;
            Assert.IsTrue(grounded.StandingOnGround);
            Assert.IsTrue(grounded.Falling);
            Assert.IsTrue(grounded.PreviousStandingOnGround);
            Assert.IsFalse(grounded.PreviousFalling);
            grounded.previousAngle = 60.0f;
            Assert.IsTrue(grounded.StandingOnGround);
            Assert.IsTrue(grounded.Falling);
            Assert.IsTrue(grounded.PreviousStandingOnGround);
            Assert.IsTrue(grounded.PreviousFalling);
        }
    }
}