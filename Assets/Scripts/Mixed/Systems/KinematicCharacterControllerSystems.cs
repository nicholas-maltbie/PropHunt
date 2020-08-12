
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utilities;
using Unity.Burst;
using Unity.Entities;
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
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    public class KCCGroundedSystem : ComponentSystem
    {
        /// <summary>
        /// Maximum degrees between ground and player 
        /// </summary>
        public static readonly float MaxAngleFallDegrees = 90;

        protected override unsafe void OnUpdate()
        {
            BuildPhysicsWorld physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
            CollisionWorld collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

            // Update grounded data for each KCC
            Entities.ForEach((
                Entity ent,
                ref PhysicsCollider collider,
                ref Translation trans,
                ref Rotation rot,
                ref KCCGrounded grounded,
                ref KCCGravity gravity) =>
            {
                SelfFilteringClosestHitCollector<ColliderCastHit> hitCollector =
                    new SelfFilteringClosestHitCollector<ColliderCastHit>(ent.Index, 1.0f, collisionWorld);

                float3 from = trans.Value;
                float3 to = from + gravity.Down * grounded.groundCheckDistance;

                var input = new ColliderCastInput()
                {
                    End = to,
                    Start = from,
                    Collider = collider.ColliderPtr,
                    Orientation = rot.Value
                };

                bool collisionOcurred = collisionWorld.CastCollider(input, ref hitCollector);
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
            });
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
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateBefore(typeof(KCCMovementSystem))]
    [UpdateAfter(typeof(KCCGravitySystem))]
    public class KCCJumpSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((
                ref KCCJumping jumping,
                ref KCCVelocity velocity,
                ref KCCGrounded grounded,
                ref KCCGravity gravity) =>
            {
                // If the KCC is attempting to jump and is grounded, jump
                if (jumping.attemptingJump && !grounded.Falling)
                {
                    velocity.worldVelocity = gravity.Up * jumping.jumpForce;
                }
                // Otherwise, do nothing
            });
        }
    }

    /// <summary>
    /// Applies gravity to kinematic character controller. Does
    /// this after checking if character is grounded
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateBefore(typeof(KCCMovementSystem))]
    [UpdateAfter(typeof(KCCGrounded))]
    public class KCCGravitySystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            Entities.ForEach((
                ref KCCVelocity velocity,
                ref KCCGrounded grounded,
                ref KCCGravity gravity) =>
            {
                // If the player is not grounded, push down by
                // gravity's acceleration
                if (grounded.Falling) {
                    velocity.worldVelocity += gravity.gravityAcceleration * deltaTime;
                }
                // Have hit the ground, stop moving
                else {
                    velocity.worldVelocity = float3.zero;
                }
            });
        }
    }


}
