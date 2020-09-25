using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utilities;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System group for all Kinematic Character Controller Actions
    /// </summary>
    [UpdateAfter(typeof(MovementTrackingSystem))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
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
                    var hitCollector = FilteringClosestHitCollector<ColliderCastHit>.GetSelfFilteringCollector(entity.Index, 1.0f, physicsWorld.CollisionWorld);

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

                    grounded.previousAngle = grounded.angle;
                    grounded.previousOnGround = grounded.onGround;

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
                        grounded.elapsedFallTime = 0;
                    }
                    else
                    {
                        grounded.onGround = false;
                        grounded.distanceToGround = -1;
                        grounded.angle = -1;
                        grounded.groundedRBIndex = -1;
                        grounded.groundedPoint = float3.zero;
                        grounded.hitEntity = Entity.Null;
                        grounded.elapsedFallTime += deltaTime;
                    }
                }
            ).ScheduleParallel();
        }
    }

    /// <summary>
    /// Pushes kinematic character controllers out of objects they are stuck in
    /// </summary>
    [UpdateInGroup(typeof(KCCUpdateGroup))]
    [UpdateAfter(typeof(KCCGroundedSystem))]
    [UpdateBefore(typeof(KCCMovementSystem))]
    public class KCCPushOverlappingSystem : SystemBase
    {
        protected unsafe override void OnUpdate()
        {
            var physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;

            Entities.ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    ref Translation translation,
                    in PhysicsCollider collider,
                    in KCCGrounded grounded,
                    in KCCMovementSettings movementSettings
                ) =>
            {
                // Draw a ray from the center of the character collider to 
                //  the point of collision
                // If the ray intersects the other object before it reaches the edge of our own collider,
                //  then we know that we are overlapping with the object
                float3 hitPoint = grounded.groundedPoint;
                int hitObject = grounded.hitEntity.Index;
                int selfIndex = entity.Index;

                // Hit collector to only collide with our object and the object we overlap with
                var hitCollector = new FilteringClosestHitCollector<Unity.Physics.RaycastHit>(
                    selfIndex, hitObject, 1.0f, physicsWorld.CollisionWorld);

                // Compute the center of our collider
                float3 center = translation.Value + movementSettings.characterCenter;

                // Draw a ray from the center of the character to the hit object
                var input = new RaycastInput()
                {
                    Filter = collider.Value.Value.Filter,
                    Start = center,
                    End = hitPoint,
                };

                // Do the raycast computation
                bool collisionOcurred = physicsWorld.CollisionWorld.CastRay(input, ref hitCollector);

                // Push our character collider out of the object we are overlapping with
                if (collisionOcurred)
                {
                    // Hit something
                    var raycastHit = hitCollector.ClosestHit;
                    // Get the distance of overlap (1 - hit fraction) * distance
                    float3 direction = hitPoint - center;
                    float distance = math.length(direction);
                    float overlapDistance = (1 - raycastHit.Fraction) * distance;
                    // Get movement in direction touching object`
                    float3 push = math.normalizesafe(-direction) * (overlapDistance);
                    // Push character collider by this much
                    translation.Value = translation.Value + push;
                }
            }).Schedule();
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
            var physicsMassGetter = this.GetComponentDataFromEntity<PhysicsMass>(true);
            float deltaTime = Time.DeltaTime;

            Entities.WithReadOnly(physicsMassGetter).ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref Translation translation,
                in KCCVelocity velocity,
                in PhysicsCollider physicsCollider,
                in Rotation rotation,
                in KCCMovementSettings movementSettings) =>
            {
                // Adjust character translation due to player movement
                translation.Value = KCCUtils.ProjectValidMovement(
                    commandBuffer,
                    entityInQueryIndex,
                    physicsWorld.CollisionWorld,
                    translation.Value,
                    velocity.playerVelocity * deltaTime,
                    physicsCollider,
                    entity.Index,
                    rotation.Value,
                    physicsMassGetter,
                    maxBounces: movementSettings.moveMaxBounces,
                    pushPower: movementSettings.movePushPower,
                    pushDecay: movementSettings.movePushDecay,
                    anglePower: movementSettings.moveAnglePower
                );
                // Adjust character translation due to gravity/world forces
                translation.Value = KCCUtils.ProjectValidMovement(
                    commandBuffer,
                    entityInQueryIndex,
                    physicsWorld.CollisionWorld,
                    translation.Value,
                    velocity.worldVelocity * deltaTime,
                    physicsCollider,
                    entity.Index,
                    rotation.Value,
                    physicsMassGetter,
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
                    if (jumping.attemptingJump && grounded.elapsedFallTime <= jumping.jumpGraceTime && jumping.timeElapsedSinceJump >= jumping.jumpCooldown)
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
                ref Translation translation,
                in KCCGrounded grounded) =>
                {
                    // Displacement of floor
                    float3 displacement = float3.zero;

                    // Bit jittery but this could probably be fixed by smoothing the movement a bit
                    // to handle server lag and difference between positions
                    if (!grounded.Falling && this.HasComponent<MovementTracking>(grounded.hitEntity))
                    {
                        MovementTracking track = this.GetComponent<MovementTracking>(grounded.hitEntity);
                        displacement = MovementTracking.GetDisplacementAtPoint(track, grounded.groundedPoint);
                        translation.Value += displacement;
                        velocity.worldVelocity = float3.zero;
                    }
                    else if (!grounded.Falling)
                    {
                        velocity.worldVelocity = float3.zero;
                    }
                    // If was grounded previous frame and no longer falling, add previous floor velocity to
                    //  current world velocity as if you jump off with that momentum
                    else if (grounded.Falling && !grounded.PreviousFalling)
                    {
                        velocity.worldVelocity += velocity.floorVelocity;
                    }

                    // Set velocity of floor
                    velocity.floorVelocity = displacement / deltaTime;
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