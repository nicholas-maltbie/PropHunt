using PropHunt.MaterialOverrides;
using PropHunt.Mixed.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Client.Systems
{

    /// <summary>
    /// Highlight objects on the screen for clients
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class HighlightObjectsSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            var emissionActiveGetter = this.GetComponentDataFromEntity<EmissionActiveFloatOverride>(true);
            var emissionColorGetter = this.GetComponentDataFromEntity<EmissionColorFloatOverride>(true);
            var hasHeartbeatGetter = this.GetComponentDataFromEntity<HasHeartbeatFloatOverride>(true);
            var heartbeatFrequencyGetter = this.GetComponentDataFromEntity<HeartbeatFrequencyFloatOverride>(true);
            var fresnelValueGetter = this.GetComponentDataFromEntity<FresnelValueFloatOverride>(true);
            var parallelWriter = ecb.AsParallelWriter();

            Entities.WithChangeFilter<FocusTarget>()
                .WithReadOnly(emissionActiveGetter)
                .WithReadOnly(emissionColorGetter)
                .WithReadOnly(hasHeartbeatGetter)
                .WithReadOnly(heartbeatFrequencyGetter)
                .WithReadOnly(fresnelValueGetter)
                .ForEach((
                Entity entity,
                int entityInQueryIndex,
                HighlightableComponent highlightable,
                FocusTarget focusTarget
            ) =>
            {
                if (focusTarget.isFocused)
                {
                    if (!emissionActiveGetter.HasComponent(entity))
                    {
                        parallelWriter.AddComponent<EmissionActiveFloatOverride>(entityInQueryIndex, entity, new EmissionActiveFloatOverride { Value = 1.0f });
                    }
                    else
                    {
                        parallelWriter.SetComponent<EmissionActiveFloatOverride>(entityInQueryIndex, entity, new EmissionActiveFloatOverride { Value = 1.0f });
                    }
                    if (!emissionColorGetter.HasComponent(entity))
                    {
                        parallelWriter.AddComponent<EmissionColorFloatOverride>(entityInQueryIndex, entity, new EmissionColorFloatOverride { Value = highlightable.EmissionColor });
                    }
                    else
                    {
                        parallelWriter.SetComponent<EmissionColorFloatOverride>(entityInQueryIndex, entity, new EmissionColorFloatOverride { Value = highlightable.EmissionColor });
                    }
                    if (!hasHeartbeatGetter.HasComponent(entity))
                    {
                        parallelWriter.AddComponent<HasHeartbeatFloatOverride>(entityInQueryIndex, entity, new HasHeartbeatFloatOverride { Value = highlightable.hasHeartbeat ? 1.0f : 0.0f });
                    }
                    else
                    {
                        parallelWriter.SetComponent<HasHeartbeatFloatOverride>(entityInQueryIndex, entity, new HasHeartbeatFloatOverride { Value = highlightable.hasHeartbeat ? 1.0f : 0.0f });
                    }
                    if (!heartbeatFrequencyGetter.HasComponent(entity))
                    {
                        parallelWriter.AddComponent<HeartbeatFrequencyFloatOverride>(entityInQueryIndex, entity, new HeartbeatFrequencyFloatOverride { Value = highlightable.heartbeatSpeed });
                    }
                    else
                    {
                        parallelWriter.SetComponent<HeartbeatFrequencyFloatOverride>(entityInQueryIndex, entity, new HeartbeatFrequencyFloatOverride { Value = highlightable.heartbeatSpeed });
                    }
                    if (!fresnelValueGetter.HasComponent(entity))
                    {
                        parallelWriter.AddComponent<FresnelValueFloatOverride>(entityInQueryIndex, entity, new FresnelValueFloatOverride { Value = highlightable.fresnelValue });
                    }
                    else
                    {
                        parallelWriter.SetComponent<HeartbeatFrequencyFloatOverride>(entityInQueryIndex, entity, new HeartbeatFrequencyFloatOverride { Value = highlightable.heartbeatSpeed });
                    }
                }
                else
                {
                    if (!emissionActiveGetter.HasComponent(entity))
                    {
                        parallelWriter.AddComponent<EmissionActiveFloatOverride>(entityInQueryIndex, entity, new EmissionActiveFloatOverride { Value = 0.0f });
                    }
                    else
                    {
                        parallelWriter.SetComponent<EmissionActiveFloatOverride>(entityInQueryIndex, entity, new EmissionActiveFloatOverride { Value = 0.0f });
                    }
                }
            }).ScheduleParallel();
            this.Dependency.Complete();

            // Run the command buffer
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}