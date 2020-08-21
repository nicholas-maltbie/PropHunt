
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

        /// <summary>
        /// Entity queries for selecting entities
        /// that fit the archetype for KCCGrounded
        /// </summary>
        private EntityQuery m_Query;

        /// <summary>
        /// Job for updating grounded state in chunks
        /// </summary>
        [BurstCompile]
        struct KCCGroundedJob : IJobChunk
        {
            public ArchetypeChunkComponentType<KCCGrounded> KCCGroundedType;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ArchetypeChunkComponentType<PhysicsCollider> PhysicsColliderType;
            [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
            [ReadOnly] public ArchetypeChunkComponentType<Rotation> RotationType;
            [ReadOnly] public ArchetypeChunkComponentType<KCCGravity> KCCGravityType;
            public PhysicsWorld physicsWorld;

            public unsafe void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var chunkGrounded = chunk.GetNativeArray(KCCGroundedType);
                var chunkPhysicsCollider = chunk.GetNativeArray(PhysicsColliderType);
                var chunkTranslation = chunk.GetNativeArray(TranslationType);
                var chunkRotation = chunk.GetNativeArray(RotationType);
                var chunkKCCGravity = chunk.GetNativeArray(KCCGravityType);
                var chunkEntity = chunk.GetNativeArray(EntityType);
                var instanceCount = chunk.Count;

                for (int i = 0; i < instanceCount; i++)
                {
                    var entity = chunkEntity[i];
                    var grounded = chunkGrounded[i];
                    var collider = chunkPhysicsCollider[i];
                    var translation = chunkTranslation[i];
                    var rotation = chunkRotation[i];
                    var gravity = chunkKCCGravity[i];
                    
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

                    // reset assignment of written items
                    chunkGrounded[i] = grounded;
                }
            }
        }

        protected override void OnCreate()
        {
            var queryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadWrite<KCCGrounded>(),
                    ComponentType.ReadOnly<PhysicsCollider>(),
                    ComponentType.ReadOnly<Translation>(),
                    ComponentType.ReadOnly<Rotation>(),
                    ComponentType.ReadOnly<KCCGravity>()
                }
            };

            this.m_Query = GetEntityQuery(queryDesc);
        }

        protected unsafe override void OnUpdate()
        {
            BuildPhysicsWorld physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();

            var job = new KCCGroundedJob()
            {
                EntityType = this.GetArchetypeChunkEntityType(),
                KCCGroundedType = GetArchetypeChunkComponentType<KCCGrounded>(false),
                PhysicsColliderType = GetArchetypeChunkComponentType<PhysicsCollider>(true),
                TranslationType = GetArchetypeChunkComponentType<Translation>(true),
                RotationType = GetArchetypeChunkComponentType<Rotation>(true),
                KCCGravityType = GetArchetypeChunkComponentType<KCCGravity>(true),
                physicsWorld = physicsWorld.PhysicsWorld
            };

            this.Dependency = job.ScheduleParallel(m_Query, this.Dependency);
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
    public class KCCJumpSystem : SystemBase
    {

        /// <summary>
        /// Entity queries for selecting entities
        /// that fit the archetype for KCCJumpSystem
        /// </summary>
        private EntityQuery m_Query;

        /// <summary>
        /// Job for updating jump state in chunks
        /// </summary>
        [BurstCompile]
        private struct KCCJumpJob : IJobChunk
        {
            public ArchetypeChunkComponentType<KCCVelocity> KCCVelocityType;
            [ReadOnly] public ArchetypeChunkComponentType<KCCJumping> KCCJumpingType;
            [ReadOnly] public ArchetypeChunkComponentType<KCCGrounded> KCCGroundedType;
            [ReadOnly] public ArchetypeChunkComponentType<KCCGravity> KCCGravityType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var chunkVelocity = chunk.GetNativeArray(KCCVelocityType);
                var chunkJumping = chunk.GetNativeArray(KCCJumpingType);
                var chunkGrounded = chunk.GetNativeArray(KCCGroundedType);
                var chunkKCCGravity = chunk.GetNativeArray(KCCGravityType);
                var instanceCount = chunk.Count;

                for (int i = 0; i < instanceCount; i++)
                {
                    var velocity = chunkVelocity[i];
                    var jumping = chunkJumping[i];
                    var grounded = chunkGrounded[i];
                    var gravity = chunkKCCGravity[i];
                    
                    // If the KCC is attempting to jump and is grounded, jump
                    if (jumping.attemptingJump && !grounded.Falling)
                    {
                        velocity.worldVelocity = gravity.Up * jumping.jumpForce;
                    }
                    // Otherwise, do nothing

                    chunkVelocity[i] = velocity;
                }
            }
        }

        protected override void OnCreate()
        {
            var queryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadWrite<KCCVelocity>(),
                    ComponentType.ReadOnly<KCCJumping>(),
                    ComponentType.ReadOnly<KCCGrounded>(),
                    ComponentType.ReadOnly<KCCGravity>()
                }
            };

            this.m_Query = GetEntityQuery(queryDesc);
        }

        protected override void OnUpdate()
        {
            var job = new KCCJumpJob()
            {
                KCCVelocityType = GetArchetypeChunkComponentType<KCCVelocity>(false),
                KCCJumpingType = GetArchetypeChunkComponentType<KCCJumping>(true),
                KCCGroundedType = GetArchetypeChunkComponentType<KCCGrounded>(true),
                KCCGravityType = GetArchetypeChunkComponentType<KCCGravity>(true),
            };

            this.Dependency = job.ScheduleParallel(m_Query, this.Dependency);
        }
    }

    /// <summary>
    /// Applies gravity to kinematic character controller. Does
    /// this after checking if character is grounded
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateBefore(typeof(KCCMovementSystem))]
    [UpdateAfter(typeof(KCCGroundedSystem))]
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
