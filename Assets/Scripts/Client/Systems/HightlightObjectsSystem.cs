using PropHunt.MaterialOverrides;
using PropHunt.Mixed.Components;
using PropHunt.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

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
            var childBufferGetter = this.GetBufferFromEntity<Child>(true);
            var parallelWriter = ecb.AsParallelWriter();

            Entities.WithChangeFilter<FocusTarget>()
                .WithReadOnly(emissionActiveGetter)
                .WithReadOnly(emissionColorGetter)
                .WithReadOnly(hasHeartbeatGetter)
                .WithReadOnly(heartbeatFrequencyGetter)
                .WithReadOnly(fresnelValueGetter)
                .WithReadOnly(childBufferGetter)
                .ForEach((
                Entity entity,
                int entityInQueryIndex,
                in HighlightableComponent highlightable,
                in FocusTarget focusTarget
            ) =>
            {
                if (focusTarget.isFocused)
                {
                    ModifyEntityUtilities.AddOrSetDataRecursive<EmissionActiveFloatOverride>(entityInQueryIndex, entity,
                        new EmissionActiveFloatOverride { Value = 1.0f },
                        emissionActiveGetter, childBufferGetter, parallelWriter);
                    ModifyEntityUtilities.AddOrSetDataRecursive<EmissionColorFloatOverride>(entityInQueryIndex, entity,
                        new EmissionColorFloatOverride { Value = highlightable.EmissionColor },
                        emissionColorGetter, childBufferGetter, parallelWriter);
                    ModifyEntityUtilities.AddOrSetDataRecursive<HasHeartbeatFloatOverride>(entityInQueryIndex, entity,
                        new HasHeartbeatFloatOverride { Value = highlightable.hasHeartbeat ? 1.0f : 0.0f },
                        hasHeartbeatGetter, childBufferGetter, parallelWriter);
                    ModifyEntityUtilities.AddOrSetDataRecursive<HeartbeatFrequencyFloatOverride>(entityInQueryIndex, entity,
                        new HeartbeatFrequencyFloatOverride { Value = highlightable.heartbeatFrequency },
                        heartbeatFrequencyGetter, childBufferGetter, parallelWriter);
                    ModifyEntityUtilities.AddOrSetDataRecursive<FresnelValueFloatOverride>(entityInQueryIndex, entity,
                        new FresnelValueFloatOverride { Value = highlightable.fresnelValue },
                        fresnelValueGetter, childBufferGetter, parallelWriter);
                }
                else
                {
                    ModifyEntityUtilities.AddOrSetDataRecursive<EmissionActiveFloatOverride>(entityInQueryIndex, entity,
                        new EmissionActiveFloatOverride { Value = 0.0f },
                        emissionActiveGetter, childBufferGetter, parallelWriter);
                }
            }).ScheduleParallel();
            this.Dependency.Complete();

            // Run the command buffer
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}