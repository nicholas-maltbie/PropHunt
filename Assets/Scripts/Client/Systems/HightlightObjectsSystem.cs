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
            var parallelWriter = ecb.AsParallelWriter();

            Entities.WithChangeFilter<FocusTarget>()
                .ForEach((
                Entity entity,
                int entityInQueryIndex,
                HighlightableComponent highlightable,
                FocusTarget focusTarget
            ) =>
            {
                if (focusTarget.isFocused)
                {
                    parallelWriter.AddComponent<EmissionActiveFloatOverride>(entityInQueryIndex, entity, new EmissionActiveFloatOverride { Value = 1.0f });
                    parallelWriter.AddComponent<EmissionColorFloatOverride>(entityInQueryIndex, entity, new EmissionColorFloatOverride { Value = highlightable.EmissionColor });
                    parallelWriter.AddComponent<HasHeartbeatFloatOverride>(entityInQueryIndex, entity, new HasHeartbeatFloatOverride { Value = highlightable.hasHeartbeat ? 1.0f : 0.0f });
                    parallelWriter.AddComponent<HeartbeatFrequencyFloatOverride>(entityInQueryIndex, entity, new HeartbeatFrequencyFloatOverride { Value = highlightable.heartbeatSpeed });
                    parallelWriter.AddComponent<FresnelValueFloatOverride>(entityInQueryIndex, entity, new FresnelValueFloatOverride{ Value = highlightable.fresnelValue });
                }
                else
                {
                    parallelWriter.AddComponent<EmissionActiveFloatOverride>(entityInQueryIndex, entity, new EmissionActiveFloatOverride { Value = 0.0f });
                }
            }).ScheduleParallel();
            this.Dependency.Complete();

            // Run the command buffer
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}