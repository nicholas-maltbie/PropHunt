
using Unity.Entities;
using Unity.NetCode;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using PropHunt.Mixed.Components;
using Unity.Collections;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Burst;

namespace PropHunt.Mixed.Systems
{

    /// <summary>
    /// Apply pushes to objects with push component
    /// </summary>
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    public class PushForceSystem : SystemBase
    {
        /// <summary>
        /// Command buffer for removing push forces after they are applied
        /// </summary>
        EndSimulationEntityCommandBufferSystem commandBufferSystem;

        /// <summary>
        /// Entity queries for selecting entities
        /// that fit the archetype for PushForce
        /// </summary>
        private EntityQuery m_Query;

        /// <summary>
        /// Struct for the job of pushing an chunk of objects
        /// and then cleaning up the PushForce events
        /// </summary>
        [BurstCompile]
        struct PushJob : IJobChunk
        {
            public bool isServer;

            public EntityCommandBuffer.Concurrent commandBuffer;

            public ArchetypeChunkComponentType<PhysicsVelocity> PhysicsVelocityType;

            [ReadOnly] public ArchetypeChunkEntityType EntityType;

            [ReadOnly] public ArchetypeChunkComponentType<PushForce> PushForceType;

            [ReadOnly] public ArchetypeChunkComponentType<PhysicsMass> PhysicsMassType;

            [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;

            [ReadOnly] public ArchetypeChunkComponentType<Rotation> RotationType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var chunkEntity = chunk.GetNativeArray(EntityType);
                var chunkPushForce = chunk.GetNativeArray(PushForceType);
                var instanceCount = chunk.Count;

                if (isServer && // Only push if server world
                    chunk.Has<PhysicsVelocity>(PhysicsVelocityType) &&
                    chunk.Has<PhysicsMass>(PhysicsMassType) &&
                    chunk.Has<Translation>(TranslationType) &&
                    chunk.Has<Rotation>(RotationType))
                {
                    var chunkPhysicsVelocity = chunk.GetNativeArray(PhysicsVelocityType);
                    var chunkPhysicsMass = chunk.GetNativeArray(PhysicsMassType);
                    var chunkTranslation = chunk.GetNativeArray(TranslationType);
                    var chunkRotation = chunk.GetNativeArray(RotationType);

                    for (int i = 0; i < instanceCount; i++)
                    {
                        var physicsVelocity = chunkPhysicsVelocity[i];
                        var physicsMass = chunkPhysicsMass[i];
                        var pushForce = chunkPushForce[i];
                        var translation = chunkTranslation[i];
                        var rotation = chunkRotation[i];

                        UnityEngine.Debug.Log($"Pushing entity {chunkEntity[i].Index}");
                        
                        physicsVelocity.ApplyImpulse(physicsMass, translation, rotation, pushForce.force, pushForce.point);

                        chunkPhysicsVelocity[i] = physicsVelocity;
                    }
                }

                for (int i = 0; i < instanceCount; i++)
                {
                    var entity = chunkEntity[i];
                    var pushForce = chunkPushForce[i];

                    commandBuffer.RemoveComponent(chunkIndex, entity, typeof(PushForce));
                }
            }
        }

        protected override void OnCreate()
        {
            this.commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            var queryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<PushForce>(),
                },
                Any = new ComponentType[]
                {
                    ComponentType.ReadWrite<PhysicsVelocity>(),
                    ComponentType.ReadOnly<PhysicsMass>(),
                    ComponentType.ReadOnly<Translation>(),
                    ComponentType.ReadOnly<Rotation>()
                }
            };

            this.m_Query = GetEntityQuery(queryDesc);
        }

        protected override void OnUpdate()
        {
            var commandBuffer = this.commandBufferSystem.CreateCommandBuffer().ToConcurrent();

            var job = new PushJob()
            {
                isServer = World.GetExistingSystem<ServerSimulationSystemGroup>() != null,
                commandBuffer = commandBuffer,
                EntityType = this.GetArchetypeChunkEntityType(),
                PhysicsVelocityType = this.GetArchetypeChunkComponentType<PhysicsVelocity>(),
                PushForceType = this.GetArchetypeChunkComponentType<PushForce>(),
                PhysicsMassType = this.GetArchetypeChunkComponentType<PhysicsMass>(),
                TranslationType = this.GetArchetypeChunkComponentType<Translation>(),
                RotationType = this.GetArchetypeChunkComponentType<Rotation>()
            };

            this.Dependency = job.ScheduleParallel(m_Query, this.Dependency);
            this.commandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}
