using NUnit.Framework;
using PropHunt.Mixed.Systems;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Moq;
using PropHunt.InputManagement;
using Unity.Transforms;
using Unity.Mathematics;
using PropHunt.Tests.Utils;
using PropHunt.Mixed.Utilities;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class KCCMoveWithGroundTests : ECSTestsFixture
    {
        /// <summary>
        /// Move with ground tests for KCC
        /// </summary>
        private KCCMoveWithGroundSystem kccMoveWithGround;

        /// <summary>
        /// Mock of unity service for managing delta time in a testable manner
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.kccMoveWithGround = base.World.CreateSystem<KCCMoveWithGroundSystem>();

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();
            this.kccMoveWithGround.unityService = unityServiceMock.Object;
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);
        }

        /// <summary>
        /// Script to create a test player for the KCCMoveWithGroundSystem
        /// </summary>
        /// <returns></returns>
        public Entity CreateTestPlayer(float3 position)
        {
            Entity player = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<KCCGravity>(player, new KCCGravity { gravityAcceleration = new float3(0, -9.8f, 0) });
            base.m_Manager.AddComponentData<KCCVelocity>(player, new KCCVelocity { playerVelocity = float3.zero, worldVelocity = float3.zero });
            base.m_Manager.AddComponentData<Translation>(player, new Translation { Value = position });
            base.m_Manager.AddComponent<FloorMovement>(player);
            base.m_Manager.AddComponentData<KCCGrounded>(player, new KCCGrounded
            {
                groundCheckDistance = 0.1f,
                groundFallingDistance = 0.05f,
                maxWalkAngle = 45.0f,
                hitEntity = Entity.Null
            });

            return player;
        }

        [Test]
        public void VerifyNoMoveWhenNotStandingOnPlatform()
        {
            Entity player = CreateTestPlayer(float3.zero);
            // Set the velocity to a temporary value
            float3 temporaryVelocity = new float3(1, 1, 1);
            KCCVelocity velocity = base.m_Manager.GetComponentData<KCCVelocity>(player);
            velocity.worldVelocity = temporaryVelocity;
            base.m_Manager.SetComponentData<KCCVelocity>(player, velocity);
            // Set current grounded state to not falling
            KCCGrounded groundedState = base.m_Manager.GetComponentData<KCCGrounded>(player);
            // take player off ground
            groundedState.onGround = false;
            groundedState.distanceToGround = -1;
            // Assert is falling
            Assert.IsTrue(groundedState.Falling);
            // Set to be falling previous frame as well
            groundedState.previousOnGround = false;
            groundedState.previousDistanceToGround = -1;
            // Assert is falling previous frame
            Assert.IsTrue(groundedState.PreviousFalling);

            base.m_Manager.SetComponentData<KCCGrounded>(player, groundedState);

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.kccMoveWithGround.Update();
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;

            // Assert that player world velocity is still temporary value
            Assert.IsTrue(math.all(base.m_Manager.GetComponentData<KCCVelocity>(player).worldVelocity == temporaryVelocity));
            // Assert floor movement state is correct as well
            FloorMovement floorState = base.m_Manager.GetComponentData<FloorMovement>(player);
            Assert.IsTrue(math.all(floorState.floorVelocity == float3.zero));
            Assert.IsTrue(math.all(floorState.frameDisplacement == float3.zero));
            // Assert that final position is same as initial position
            Assert.IsTrue(TestUtils.WithinErrorRange(startingPosition, endingPosition));
        }

        [Test]
        public void VerifyNoMoveWhenStandingOnNonMovementTracking()
        {
            Entity player = CreateTestPlayer(float3.zero);
            // put the player on some ground
            Entity ground = base.m_Manager.CreateEntity();
            // Set the velocity to a temporary value
            // Assert that player is not moving along vertical (gravity) axis
            float3 temporaryVelocity = new float3(1, 0, 1);
            KCCVelocity velocity = base.m_Manager.GetComponentData<KCCVelocity>(player);
            velocity.worldVelocity = temporaryVelocity;
            velocity.playerVelocity = float3.zero;
            base.m_Manager.SetComponentData<KCCVelocity>(player, velocity);
            // Assert that the player is not moving along vertical axs
            Assert.IsFalse(KCCUtils.HasMovementAlongAxis(velocity, base.m_Manager.GetComponentData<KCCGravity>(player).Up));
            // Set current grounded state to not falling
            KCCGrounded groundedState = base.m_Manager.GetComponentData<KCCGrounded>(player);
            // take player off ground
            groundedState.onGround = true;
            groundedState.distanceToGround = 0.0f;
            groundedState.angle = 0;
            groundedState.hitEntity = ground;
            // Assert is not falling
            Assert.IsTrue(!groundedState.Falling);
            // Set to be falling previous frame as well
            groundedState.previousOnGround = true;
            groundedState.previousDistanceToGround = 0.0f;
            groundedState.previousAngle = 0;
            // Assert is not falling previous frame
            Assert.IsTrue(!groundedState.PreviousFalling);
            base.m_Manager.SetComponentData<KCCGrounded>(player, groundedState);

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.kccMoveWithGround.Update();
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;

            // Assert that player world velocity is now zero
            Assert.IsTrue(math.all(base.m_Manager.GetComponentData<KCCVelocity>(player).worldVelocity == float3.zero));
            // Assert floor movement state is correct as well
            FloorMovement floorState = base.m_Manager.GetComponentData<FloorMovement>(player);
            Assert.IsTrue(math.all(floorState.floorVelocity == float3.zero));
            Assert.IsTrue(math.all(floorState.frameDisplacement == float3.zero));
            // Assert that final position is same as initial position
            Assert.IsTrue(TestUtils.WithinErrorRange(startingPosition, endingPosition));
        }

        [Test]
        public void VerifyNoMoveWhenStandingOnNonMovementTrackingMovingVertically()
        {
            Entity player = CreateTestPlayer(float3.zero);
            // put the player on some ground
            Entity ground = base.m_Manager.CreateEntity();
            // Set the velocity to a temporary value
            // Assert that player IS moving along vertical (gravity) axis
            float3 worldVelocity = new float3(0, 1, 0);
            float3 playerVelocity = new float3(1, 0, 0);
            KCCVelocity velocity = base.m_Manager.GetComponentData<KCCVelocity>(player);
            velocity.worldVelocity = worldVelocity;
            velocity.playerVelocity = playerVelocity;
            base.m_Manager.SetComponentData<KCCVelocity>(player, velocity);
            // Assert that the player is not moving along vertical axs
            Assert.IsTrue(KCCUtils.HasMovementAlongAxis(velocity, base.m_Manager.GetComponentData<KCCGravity>(player).Up));
            // Set current grounded state to not falling
            KCCGrounded groundedState = base.m_Manager.GetComponentData<KCCGrounded>(player);
            // take player off ground
            groundedState.onGround = true;
            groundedState.distanceToGround = 0.0f;
            groundedState.angle = 0;
            groundedState.hitEntity = ground;
            // Assert is not falling
            Assert.IsTrue(!groundedState.Falling);
            // Set to be falling previous frame as well
            groundedState.previousOnGround = true;
            groundedState.previousDistanceToGround = 0.0f;
            groundedState.previousAngle = 0;
            // Assert is not falling previous frame
            Assert.IsTrue(!groundedState.PreviousFalling);
            base.m_Manager.SetComponentData<KCCGrounded>(player, groundedState);

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.kccMoveWithGround.Update();
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;

            // Assert that world velocity is still the same
            Assert.IsTrue(math.all(base.m_Manager.GetComponentData<KCCVelocity>(player).worldVelocity == worldVelocity));
            // Assert floor movement state is correct as well
            FloorMovement floorState = base.m_Manager.GetComponentData<FloorMovement>(player);
            Assert.IsTrue(math.all(floorState.floorVelocity == float3.zero));
            Assert.IsTrue(math.all(floorState.frameDisplacement == float3.zero));
            // Assert that final position is same as initial position
            Assert.IsTrue(TestUtils.WithinErrorRange(startingPosition, endingPosition));
        }

        [Test]
        public void VerifyNoMoveWhenLeavingPlatformAndTransferingMotion()
        {
            Entity player = CreateTestPlayer(float3.zero);
            // Set the floor velocity of the player
            float3 newFloorVelocity = new float3(10, 0, -10);
            base.m_Manager.SetComponentData<FloorMovement>(player, new FloorMovement
            {
                floorVelocity = newFloorVelocity
            });
            // Set the velocity to a temporary value
            float3 worldVelocity = new float3(0, 1, 0);
            KCCVelocity velocity = base.m_Manager.GetComponentData<KCCVelocity>(player);
            velocity.worldVelocity = worldVelocity;
            base.m_Manager.SetComponentData<KCCVelocity>(player, velocity);
            // Set current grounded state to be falling
            KCCGrounded groundedState = base.m_Manager.GetComponentData<KCCGrounded>(player);
            // take player off ground
            groundedState.onGround = false;
            groundedState.distanceToGround = -1;
            groundedState.angle = 0;
            // Assert is not falling
            Assert.IsTrue(groundedState.Falling);
            // Set to be falling previous frame
            groundedState.previousOnGround = true;
            groundedState.previousDistanceToGround = 0.0f;
            groundedState.previousAngle = 0;
            // Assert is not falling previous frame
            Assert.IsTrue(!groundedState.PreviousFalling);
            base.m_Manager.SetComponentData<KCCGrounded>(player, groundedState);

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.kccMoveWithGround.Update();
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;

            // Assert that world velocity is now the floor velocity
            Assert.IsTrue(math.all(base.m_Manager.GetComponentData<KCCVelocity>(player).worldVelocity == worldVelocity + newFloorVelocity));
            // Assert floor movement state is correct as well
            FloorMovement floorState = base.m_Manager.GetComponentData<FloorMovement>(player);
            Assert.IsTrue(math.all(floorState.floorVelocity == float3.zero));
            Assert.IsTrue(math.all(floorState.frameDisplacement == float3.zero));
            // Assert that final position is same as initial position
            Assert.IsTrue(TestUtils.WithinErrorRange(startingPosition, endingPosition));
        }

        [Test]
        public void VerifyMoveAndTransferMomentumWhenStandingOnMovementTracking()
        {
            float deltaTime = 2.0f;
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(deltaTime);

            Entity player = CreateTestPlayer(float3.zero);
            // put the player on some ground
            Entity ground = base.m_Manager.CreateEntity();
            // Initialize movement tracking component
            MovementTracking movement = new MovementTracking { avoidTransferMomentum = false };
            float3 groundVelocity = new float3(1, 0, 0);
            MovementTracking.UpdateState(ref movement, float3.zero, quaternion.Euler(float3.zero));
            MovementTracking.UpdateState(ref movement, groundVelocity, quaternion.Euler(float3.zero));
            base.m_Manager.AddComponentData<MovementTracking>(ground, movement);
            // Set the velocity to a temporary value
            // Assert that player is not moving along vertical (gravity) axis
            float3 worldVelocity = new float3(0, 0, 0);
            float3 playerVelocity = new float3(1, 0, 0);
            KCCVelocity velocity = base.m_Manager.GetComponentData<KCCVelocity>(player);
            velocity.worldVelocity = worldVelocity;
            velocity.playerVelocity = playerVelocity;
            base.m_Manager.SetComponentData<KCCVelocity>(player, velocity);
            // Assert that the player is not moving along vertical axs
            Assert.IsFalse(KCCUtils.HasMovementAlongAxis(velocity, base.m_Manager.GetComponentData<KCCGravity>(player).Up));
            // Set current grounded state to not falling
            KCCGrounded groundedState = base.m_Manager.GetComponentData<KCCGrounded>(player);
            // take player off ground
            groundedState.onGround = true;
            groundedState.distanceToGround = 0.0f;
            groundedState.angle = 0;
            groundedState.hitEntity = ground;
            // Assert is not falling
            Assert.IsTrue(!groundedState.Falling);
            // Set to be falling previous frame as well
            groundedState.previousOnGround = true;
            groundedState.previousDistanceToGround = 0.0f;
            groundedState.previousAngle = 0;
            // Assert is not falling previous frame
            Assert.IsTrue(!groundedState.PreviousFalling);
            base.m_Manager.SetComponentData<KCCGrounded>(player, groundedState);

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.kccMoveWithGround.Update();
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;

            // Assert that world velocity is still the same
            Assert.IsTrue(math.all(base.m_Manager.GetComponentData<KCCVelocity>(player).worldVelocity == worldVelocity));
            // Assert floor movement state is correct as well
            FloorMovement floorState = base.m_Manager.GetComponentData<FloorMovement>(player);
            Assert.IsTrue(math.all(floorState.floorVelocity == groundVelocity / deltaTime));
            Assert.IsTrue(math.all(floorState.frameDisplacement == groundVelocity));
            // Assert that final position is same as initial position
            Assert.IsTrue(TestUtils.WithinErrorRange(startingPosition + floorState.frameDisplacement, endingPosition));
        }

        [Test]
        public void VerifyMoveAndNoTransferMomentumWhenStandingOnMovementTracking()
        {
            float deltaTime = 2.0f;
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(deltaTime);

            Entity player = CreateTestPlayer(float3.zero);
            // put the player on some ground
            Entity ground = base.m_Manager.CreateEntity();
            // Initialize movement tracking component
            MovementTracking movement = new MovementTracking { avoidTransferMomentum = true };
            float3 groundVelocity = new float3(1, 0, 0);
            MovementTracking.UpdateState(ref movement, float3.zero, quaternion.Euler(float3.zero));
            MovementTracking.UpdateState(ref movement, groundVelocity, quaternion.Euler(float3.zero));
            base.m_Manager.AddComponentData<MovementTracking>(ground, movement);
            // Set the velocity to a temporary value
            // Assert that player is not moving along vertical (gravity) axis
            float3 worldVelocity = new float3(0, 0, 0);
            float3 playerVelocity = new float3(1, 0, 0);
            KCCVelocity velocity = base.m_Manager.GetComponentData<KCCVelocity>(player);
            velocity.worldVelocity = worldVelocity;
            velocity.playerVelocity = playerVelocity;
            base.m_Manager.SetComponentData<KCCVelocity>(player, velocity);
            // Assert that the player is not moving along vertical axs
            Assert.IsFalse(KCCUtils.HasMovementAlongAxis(velocity, base.m_Manager.GetComponentData<KCCGravity>(player).Up));
            // Set current grounded state to not falling
            KCCGrounded groundedState = base.m_Manager.GetComponentData<KCCGrounded>(player);
            // take player off ground
            groundedState.onGround = true;
            groundedState.distanceToGround = 0.0f;
            groundedState.angle = 0;
            groundedState.hitEntity = ground;
            // Assert is not falling
            Assert.IsTrue(!groundedState.Falling);
            // Set to be falling previous frame as well
            groundedState.previousOnGround = true;
            groundedState.previousDistanceToGround = 0.0f;
            groundedState.previousAngle = 0;
            // Assert is not falling previous frame
            Assert.IsTrue(!groundedState.PreviousFalling);
            base.m_Manager.SetComponentData<KCCGrounded>(player, groundedState);

            float3 startingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;
            this.kccMoveWithGround.Update();
            float3 endingPosition = base.m_Manager.GetComponentData<Translation>(player).Value;

            // Assert that world velocity is still the same
            Assert.IsTrue(math.all(base.m_Manager.GetComponentData<KCCVelocity>(player).worldVelocity == worldVelocity));
            // Assert floor movement state is correct as well
            FloorMovement floorState = base.m_Manager.GetComponentData<FloorMovement>(player);
            Assert.IsTrue(math.all(floorState.floorVelocity == float3.zero));
            Assert.IsTrue(math.all(floorState.frameDisplacement == groundVelocity));
            // Assert that final position is same as initial position
            Assert.IsTrue(TestUtils.WithinErrorRange(startingPosition + floorState.frameDisplacement, endingPosition));
        }
    }
}