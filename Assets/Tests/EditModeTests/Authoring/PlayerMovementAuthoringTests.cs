using NUnit.Framework;
using PropHunt.Authoring;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using UnityEngine;

namespace PropHunt.EditMode.Tests.Authoring
{
    /// <summary>
    /// Tests for the player movement authoring component
    /// </summary>
    [TestFixture]
    public class PlayerMovementAuthoringTests : ECSTestsFixture
    {
        private GameObject playerObject;

        private GameObjectConversionSettings settings;

        private BlobAssetStore assetStore;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.playerObject = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.Euler(Vector3.zero));
            this.assetStore = new BlobAssetStore();
            this.settings = GameObjectConversionSettings.FromWorld(base.World, assetStore);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            // Cleanup platform object
            GameObject.DestroyImmediate(this.playerObject);
            assetStore.Dispose();
        }

        [Test]
        public void VerifyPlayerSettings()
        {
            this.playerObject.AddComponent<PlayerMovementAuthoringComponent>();
            var playerMovement = this.playerObject.GetComponent<PlayerMovementAuthoringComponent>();
            // Setup an example player movement script
            playerMovement.moveSpeed = 5f;
            playerMovement.sprintMultiplier = 2f;
            playerMovement.gravityForce = new Vector3(0, -9.8f, 0);
            playerMovement.jumpForce = 5.0f;
            playerMovement.maxWalkAngle = 45;
            playerMovement.groundCheckDistance = 0.1f;
            playerMovement.fallAnglePower = 1.2f;
            playerMovement.moveAnglePower = 2.0f;
            playerMovement.pushPower = 20.0f;
            playerMovement.moveMaxBounces = 3;
            playerMovement.fallMaxBounces = 2;
            playerMovement.pushDecay = 0.0f;
            playerMovement.jumpGraceTime = 0.0f;
            playerMovement.jumpCooldown = 0.1f;
            playerMovement.groundFallingDistance = 0.01f;
            playerMovement.maxPush = 2;
            playerMovement.stepOffset = 0.1f;
            playerMovement.snapDownOffset = 0.25f;
            playerMovement.snapDownSpeed = 0.5f;

            // Convert to a game object
            Entity converted = GameObjectConversionUtility.ConvertGameObjectHierarchy(this.playerObject, this.settings);

            // Verify components are added
            Assert.IsTrue(base.m_Manager.HasComponent<KCCJumping>(converted));
            Assert.IsTrue(base.m_Manager.HasComponent<KCCGrounded>(converted));
            Assert.IsTrue(base.m_Manager.HasComponent<KCCVelocity>(converted));
            Assert.IsTrue(base.m_Manager.HasComponent<KCCGravity>(converted));
            // Get setting components
            KCCMovementSettings settings = base.m_Manager.GetComponentData<KCCMovementSettings>(converted);
            KCCJumping jumping = base.m_Manager.GetComponentData<KCCJumping>(converted);
            KCCGrounded grounded = base.m_Manager.GetComponentData<KCCGrounded>(converted);
            KCCVelocity velocity = base.m_Manager.GetComponentData<KCCVelocity>(converted);
            KCCGravity gravity = base.m_Manager.GetComponentData<KCCGravity>(converted);
            // Verify component attributes are correct
            // Components for movement settings
            Assert.IsTrue(settings.moveSpeed == playerMovement.moveSpeed);
            Assert.IsTrue(settings.sprintMultiplier == playerMovement.sprintMultiplier);
            Assert.IsTrue(settings.moveMaxBounces == playerMovement.moveMaxBounces);
            Assert.IsTrue(settings.moveAnglePower == playerMovement.moveAnglePower);
            Assert.IsTrue(settings.movePushPower == playerMovement.pushPower);
            Assert.IsTrue(settings.movePushDecay == playerMovement.pushDecay);
            Assert.IsTrue(settings.fallMaxBounces == playerMovement.fallMaxBounces);
            Assert.IsTrue(settings.fallAnglePower == playerMovement.fallAnglePower);
            Assert.IsTrue(settings.fallPushPower == playerMovement.pushPower);
            Assert.IsTrue(settings.fallPushDecay == playerMovement.pushDecay);
            Assert.IsTrue(settings.maxPush == playerMovement.maxPush);
            Assert.IsTrue(settings.stepOffset == playerMovement.stepOffset);
            Assert.IsTrue(settings.snapDownOffset == playerMovement.snapDownOffset);
            Assert.IsTrue(settings.snapDownSpeed == playerMovement.snapDownSpeed);
            // Components for jumping
            Assert.IsTrue(jumping.jumpForce == playerMovement.jumpForce);
            Assert.IsTrue(jumping.jumpGraceTime == playerMovement.jumpGraceTime);
            Assert.IsTrue(jumping.jumpCooldown == playerMovement.jumpCooldown);
            // Components for grounded
            Assert.IsTrue(grounded.maxWalkAngle == playerMovement.maxWalkAngle);
            Assert.IsTrue(grounded.groundCheckDistance == playerMovement.groundCheckDistance);
            Assert.IsTrue(grounded.groundFallingDistance == playerMovement.groundFallingDistance);
            // Components for velocity
            Assert.IsTrue(math.all(velocity.playerVelocity == float3.zero));
            Assert.IsTrue(math.all(velocity.worldVelocity == float3.zero));
            // Gravity components
            Assert.IsTrue(math.all(gravity.gravityAcceleration == new float3(playerMovement.gravityForce)));
        }
    }
}