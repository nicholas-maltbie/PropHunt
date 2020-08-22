
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
    public class KCCMovementSystem : SystemBase
    {
        /// <summary>
        /// Command buffer system for pushing objects
        /// </summary>
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        /// <summary>
        /// Entity queries for selecting entities
        /// that fit the archetype for KCCMovement
        /// </summary>
        private EntityQuery m_Query;
 
        protected override void OnCreate()
        {
            var queryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<PhysicsCollider>(),
                    ComponentType.ReadWrite<Translation>(),
                    ComponentType.ReadOnly<Rotation>(),
                    ComponentType.ReadOnly<KCCVelocity>(),
                    ComponentType.ReadOnly<KCCMovementSettings>()
                }
            };

            this.m_Query = GetEntityQuery(queryDesc);

            this.commandBufferSystem =  World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        /// <summary>
        /// Job to handle player movement
        /// </summary>
        [BurstCompile]
        private struct KCCMovementJob : IJobChunk
        {
            public float deltaTime;

            public PhysicsWorld physicsWorld;

            public EntityCommandBuffer.Concurrent commandBuffer;

            [ReadOnly] public ArchetypeChunkEntityType EntityType;

            [ReadOnly] public ArchetypeChunkComponentType<PhysicsCollider> PhysicsColliderType;
            
            public ArchetypeChunkComponentType<Translation> TranslationType;

            [ReadOnly] public ArchetypeChunkComponentType<Rotation> RotationType;

            [ReadOnly] public ArchetypeChunkComponentType<KCCVelocity> KCCVelocityType;

            [ReadOnly] public ArchetypeChunkComponentType<KCCMovementSettings> KCCMovementSettingsType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var chunkEntity = chunk.GetNativeArray(this.EntityType);
                var chunkPhysicsCollider = chunk.GetNativeArray(this.PhysicsColliderType);
                var chunkTranslation = chunk.GetNativeArray(this.TranslationType);
                var chunkRotation = chunk.GetNativeArray(this.RotationType);
                var chunkKCCVelocity = chunk.GetNativeArray(this.KCCVelocityType);
                var chunkKCCMovementSettings = chunk.GetNativeArray(this.KCCMovementSettingsType);

                var instanceCount = chunk.Count;
                for (int i = 0; i < instanceCount; i++)
                {
                    var entity = chunkEntity[i];
                    var physicsCollider = chunkPhysicsCollider[i];
                    var translation = chunkTranslation[i];
                    var rotation = chunkRotation[i];
                    var velocity = chunkKCCVelocity[i];
                    var movementSettings = chunkKCCMovementSettings[i];

                    // Adjust character translation due to player movement
                    translation.Value = KinematicCharacterControllerUtilities.ProjectValidMovement(
                        commandBuffer,
                        chunkIndex,
                        physicsWorld.CollisionWorld,
                        translation.Value,
                        velocity.playerVelocity * deltaTime,
                        physicsCollider,
                        entity.Index,
                        rotation.Value,
                        maxBounces : movementSettings.moveMaxBounces,
                        pushPower  : movementSettings.movePushPower,
                        pushDecay  : movementSettings.movePushDecay,
                        anglePower : movementSettings.moveAnglePower
                    );
                    // Adjust character translation due to gravity/world forces
                    translation.Value = KinematicCharacterControllerUtilities.ProjectValidMovement(
                        commandBuffer,
                        chunkIndex,
                        physicsWorld.CollisionWorld,
                        translation.Value,
                        velocity.worldVelocity * deltaTime,
                        physicsCollider,
                        entity.Index,
                        rotation.Value,
                        maxBounces : movementSettings.fallMaxBounces,
                        pushPower  : movementSettings.fallPushPower,
                        pushDecay  : movementSettings.fallPushDecay,
                        anglePower : movementSettings.fallAnglePower
                    );

                    chunkTranslation[i] = translation;
                }
            }
        }

        protected override void  OnUpdate()
        {
            var job = new KCCMovementJob()
            {
                deltaTime = Time.DeltaTime,
                physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld,
                commandBuffer = this.commandBufferSystem.CreateCommandBuffer().ToConcurrent(),
                EntityType = this.GetArchetypeChunkEntityType(),
                PhysicsColliderType = this.GetArchetypeChunkComponentType<PhysicsCollider>(true),
                TranslationType = this.GetArchetypeChunkComponentType<Translation>(false),
                RotationType = this.GetArchetypeChunkComponentType<Rotation>(true),
                KCCVelocityType = this.GetArchetypeChunkComponentType<KCCVelocity>(true),
                KCCMovementSettingsType = this.GetArchetypeChunkComponentType<KCCMovementSettings>(true)
            };

            this.Dependency = job.ScheduleParallel(this.m_Query, this.Dependency);
            this.commandBufferSystem.AddJobHandleForProducer(this.Dependency);
            this.Dependency.Complete();
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
