
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utilities;
using Unity.Burst;
using Unity.Collections;
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
    /// Updates the grounded data on a kinematic character controller
    /// </summary>
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    public class KCCGroundedSystem : SystemBase
    {
        /// <summary>
        /// Maximum degrees between ground and player 
        /// </summary>
        public static readonly float MaxAngleFallDegrees = 90;

        protected unsafe override void OnUpdate()
        {
            PhysicsWorld physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;

            Entities.WithBurst().ForEach((
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

                    if(collisionOcurred) {
                        float angleBetween = math.abs(math.acos(math.dot(math.normalizesafe(hit.SurfaceNormal), gravity.Up)));
                        float angleDegrees = math.degrees(angleBetween);
                        grounded.angle = math.max(0, math.min(angleDegrees, KCCGroundedSystem.MaxAngleFallDegrees));
                        grounded.onGround = true;
                        grounded.distanceToGround = hit.Fraction * grounded.groundCheckDistance;
                    }
                    else {
                        grounded.onGround = false;
                        grounded.distanceToGround = -1;
                        grounded.angle = -1;
                    }
                }
            ).ScheduleParallel();
        }
    }

    /// <summary>
    /// Applies character movement to a kinematic character controller
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    public class KCCMovementSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            BuildPhysicsWorld physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
            CollisionWorld collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

            // Update grounded data for each KCC
            Entities.ForEach((
                Entity ent,
                ref PhysicsCollider collider,
                ref Translation trans,
                ref Rotation rot,
                ref KCCVelocity velocity,
                ref KCCMovementSettings movementSettings) =>
            {
                // Adjust character translation due to player movement
                trans.Value = KinematicCharacterControllerUtilities.ProjectValidMovement(
                    EntityManager,
                    collisionWorld,
                    trans.Value,
                    velocity.playerVelocity * deltaTime,
                    collider,
                    ent.Index,
                    rot.Value,
                    maxBounces : movementSettings.moveMaxBounces,
                    pushPower  : movementSettings.movePushPower,
                    pushDecay  : movementSettings.movePushDecay,
                    anglePower : movementSettings.moveAnglePower
                );
                // Adjust character translation due to gravity/world forces
                trans.Value = KinematicCharacterControllerUtilities.ProjectValidMovement(
                    EntityManager,
                    collisionWorld,
                    trans.Value,
                    velocity.worldVelocity * deltaTime,
                    collider,
                    ent.Index,
                    rot.Value,
                    maxBounces : movementSettings.fallMaxBounces,
                    pushPower  : movementSettings.fallPushPower,
                    pushDecay  : movementSettings.fallPushDecay,
                    anglePower : movementSettings.fallAnglePower
                );
            });
        }
    }

    /// <summary>
    /// Jumping, happens after gravity and before moving character.
    /// Will effect the world velocity of the character (since jumping
    /// will decay due to gravity)
    /// </summary>
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateBefore(typeof(KCCMovementSystem))]
    [UpdateAfter(typeof(KCCGravitySystem))]
    public class KCCJumpSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithBurst().ForEach((
                ref KCCVelocity velocity,
                in KCCJumping jumping,
                in KCCGrounded grounded,
                in KCCGravity gravity) =>
                {
                    // If the KCC is attempting to jump and is grounded, jump
                    if (jumping.attemptingJump && !grounded.Falling)
                    {
                        velocity.worldVelocity = gravity.Up * jumping.jumpForce;
                    }
                    // Otherwise, do nothing
                }
            ).ScheduleParallel();
        }
    }

    /// <summary>
    /// Applies gravity to kinematic character controller. Does
    /// this after checking if character is grounded
    /// </summary>
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateBefore(typeof(KCCMovementSystem))]
    [UpdateAfter(typeof(KCCGroundedSystem))]
    public class KCCGravitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entities.WithBurst().ForEach((
                ref KCCVelocity velocity,
                in KCCGrounded grounded,
                in KCCGravity gravity) => 
                {
                    // If the player is not grounded, push down by
                    // gravity's acceleration
                    if (grounded.Falling)
                    {
                        velocity.worldVelocity += gravity.gravityAcceleration * deltaTime;
                    }
                    // Have hit the ground, stop moving
                    else
                    {
                        velocity.worldVelocity = float3.zero;
                    }
                }
            ).ScheduleParallel();
        }
    }
}
