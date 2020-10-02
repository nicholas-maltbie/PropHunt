using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utilities;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System group for all Kinematic Character Controller Actions
    /// </summary>
    [UpdateAfter(typeof(MovementTrackingSystem))]
    [UpdateBefore(typeof(PushForceGroup))]
    public class KCCUpdateGroup : ComponentSystemGroup { }

    /// <summary>
    /// Updates the grounded data on a kinematic character controller
    /// </summary>
    [UpdateInGroup(typeof(KCCUpdateGroup))]
    public class KCCGroundedSystem : SystemBase
    {
        /// <summary>
        /// Maximum degrees between ground and player 
        /// </summary>
        public static readonly float MaxAngleFallDegrees = 90;

        protected unsafe override void OnUpdate()
        {
            PhysicsWorld physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
            float deltaTime = Time.DeltaTime;

            Entities.ForEach((
                Entity entity,
                ref KCCGrounded grounded,
                in KCCGravity gravity,
                in PhysicsCollider collider,
                in Translation translation,
                in Rotation rotation) =>
                {
                    SelfFilteringClosestHitCollector<ColliderCastHit> hitCollector =
                        new SelfFilteringClosestHitCollector<ColliderCastHit>(entity.Index, 1.0f, physicsWorld.CollisionWorld);

                    float3 from = translation.Value;
                    float3 to = from + gravity.Down * grounded.groundCheckDistance;

                    var input = new ColliderCastInput()
                    {
                        End = to,
                        Start = from,
                        Collider = collider.ColliderPtr,
                        Orientation = rotation.Value
                    };

                    bool collisionOcurred = physicsWorld.CollisionWorld.CastCollider(input, ref hitCollector);
                    Unity.Physics.ColliderCastHit hit = hitCollector.ClosestHit;

                    if (collisionOcurred)
                    {
                        float angleBetween = math.abs(math.acos(math.dot(math.normalizesafe(hit.SurfaceNormal), gravity.Up)));
                        float angleDegrees = math.degrees(angleBetween);
                        grounded.angle = math.max(0, math.min(angleDegrees, KCCGroundedSystem.MaxAngleFallDegrees));
                        grounded.onGround = true;
                        grounded.distanceToGround = hit.Fraction * grounded.groundCheckDistance;
                        grounded.groundedRBIndex = hit.RigidBodyIndex;
                        grounded.groundedPoint = hit.Position;
                        grounded.hitEntity = hit.Entity;
                    }
                    else
                    {
                        grounded.onGround = false;
                        grounded.distanceToGround = -1;
                        grounded.angle = -1;
                        grounded.groundedRBIndex = -1;
                        grounded.groundedPoint = float3.zero;
                        grounded.hitEntity = Entity.Null;
                    }

                    // Falling is generated from other values, can be falling
                    //  if hitting a steep slope on the ground.
                    if (grounded.Falling)
                    {
                        grounded.elapsedFallTime += deltaTime;
                    }
                    else
                    {
                        grounded.elapsedFallTime = 0;
                    }
                }
            ).ScheduleParallel();
        }
    }

    /// <summary>
    /// Applies character movement to a kinematic character controller
    /// </summary>
    [UpdateInGroup(typeof(KCCUpdateGroup))]
    public class KCCMovementSystem : SystemBase
    {
        /// <summary>
        /// Command buffer system for pushing objects
        /// </summary>
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            this.commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = this.commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
            float deltaTime = Time.DeltaTime;

            Entities.ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref Translation translation,
                in KCCVelocity velocity,
                in PhysicsCollider physicsCollider,
                in Rotation rotation,
                in KCCMovementSettings movementSettings) =>
            {
                // Adjust character translation due to player movement
                translation.Value = KinematicCharacterControllerUtilities.ProjectValidMovement(
                    commandBuffer,
                    entityInQueryIndex,
                    physicsWorld.CollisionWorld,
                    translation.Value,
                    velocity.playerVelocity * deltaTime,
                    physicsCollider,
                    entity.Index,
                    rotation.Value,
                    maxBounces: movementSettings.moveMaxBounces,
                    pushPower: movementSettings.movePushPower,
                    pushDecay: movementSettings.movePushDecay,
                    anglePower: movementSettings.moveAnglePower
                );
                // Adjust character translation due to gravity/world forces
                translation.Value = KinematicCharacterControllerUtilities.ProjectValidMovement(
                    commandBuffer,
                    entityInQueryIndex,
                    physicsWorld.CollisionWorld,
                    translation.Value,
                    velocity.worldVelocity * deltaTime,
                    physicsCollider,
                    entity.Index,
                    rotation.Value,
                    maxBounces: movementSettings.fallMaxBounces,
                    pushPower: movementSettings.fallPushPower,
                    pushDecay: movementSettings.fallPushDecay,
                    anglePower: movementSettings.fallAnglePower
                );
            }
            ).ScheduleParallel();

            this.Dependency.Complete();
            this.commandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }

    /// <summary>
    /// Jumping, happens after gravity and before moving character.
    /// Will effect the world velocity of the character (since jumping
    /// will decay due to gravity)
    /// </summary>
    [UpdateInGroup(typeof(KCCUpdateGroup))]
    [UpdateBefore(typeof(KCCMovementSystem))]
    [UpdateAfter(typeof(KCCGravitySystem))]
    public class KCCJumpSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.ForEach((
                ref KCCVelocity velocity,
                ref KCCJumping jumping,
                in KCCGrounded grounded,
                in KCCGravity gravity) =>
                {
                    // If the KCC is attempting to jump and is grounded, jump
                    if (jumping.attemptingJump && !grounded.Falling && grounded.elapsedFallTime <= jumping.jumpGraceTime && jumping.timeElapsedSinceJump >= jumping.jumpCooldown)
                    {
                        velocity.worldVelocity += gravity.Up * jumping.jumpForce;
                        jumping.timeElapsedSinceJump = 0.0f;
                    }
                    // Track jumping cooldown if not jumping
                    else if (jumping.timeElapsedSinceJump < jumping.jumpCooldown)
                    {
                        jumping.timeElapsedSinceJump += deltaTime;
                    }
                    // Otherwise do nothing
                }
            ).ScheduleParallel();
        }
    }

    /// <summary>
    /// System to move character with ground
    /// </summary>
    [UpdateInGroup(typeof(KCCUpdateGroup))]
    [UpdateAfter(typeof(KCCGroundedSystem))]
    [UpdateBefore(typeof(KCCGravitySystem))]
    public class KCCMoveWithGroundSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            // Only applies to grounded KCC characters with a KCC velocity.
            Entities.ForEach((
                ref KCCVelocity velocity,
                in KCCGrounded grounded) =>
                {
                    // Bit jittery but this could probably be fixed by smoothing the movement a bit
                    // to handle server lag and difference between positions
                    if (!grounded.Falling && this.HasComponent<MovementTracking>(grounded.hitEntity))
                    {
                        MovementTracking track = this.GetComponent<MovementTracking>(grounded.hitEntity);
                        velocity.worldVelocity = MovementTracking.GetDisplacementAtPoint(track, grounded.groundedPoint) / deltaTime;
                    }
                    else if (!grounded.Falling)
                    {
                        velocity.worldVelocity = float3.zero;
                    }
                }
            ).ScheduleParallel();
        }
    }

    /// <summary>
    /// Applies gravity to kinematic character controller. Does
    /// this after checking if character is grounded
    /// </summary>
    [UpdateInGroup(typeof(KCCUpdateGroup))]
    [UpdateBefore(typeof(KCCMovementSystem))]
    [UpdateAfter(typeof(KCCGroundedSystem))]
    public class KCCGravitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entities.ForEach((
                ref KCCVelocity velocity,
                in KCCGrounded grounded,
                in KCCGravity gravity) =>
                {
                    // If the player is not grounded, push down by
                    // gravity's acceleration
                    if (grounded.Falling)
                    {
                        // have world velocity decrease due to air resistance (future feature)

                        // fall due to gravity
                        velocity.worldVelocity += gravity.gravityAcceleration * deltaTime;
                    }
                    // else: Have hit the ground, don't accelerate due to gravity
                }
            ).ScheduleParallel();
        }
    }
}