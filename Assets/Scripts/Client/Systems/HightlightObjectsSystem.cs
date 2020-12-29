using PropHunt.MaterialOverrides;
using PropHunt.Mixed.Components;
using PropHunt.Utils;
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
                in HighlightableComponent highlightable,
                in FocusTarget focusTarget
            ) =>
            {
                if (focusTarget.isFocused)
                {
                    ModifyEntityUtilities.AddOrSetData<EmissionActiveFloatOverride>(entityInQueryIndex, entity,
                        new EmissionActiveFloatOverride { Value = 1.0f }, emissionActiveGetter, parallelWriter);
                    ModifyEntityUtilities.AddOrSetData<EmissionColorFloatOverride>(entityInQueryIndex, entity,
                        new EmissionColorFloatOverride { Value = highlightable.EmissionColor }, emissionColorGetter, parallelWriter);
                    ModifyEntityUtilities.AddOrSetData<HasHeartbeatFloatOverride>(entityInQueryIndex, entity,
                        new HasHeartbeatFloatOverride { Value = highlightable.hasHeartbeat ? 1.0f : 0.0f }, hasHeartbeatGetter, parallelWriter);
                    ModifyEntityUtilities.AddOrSetData<HeartbeatFrequencyFloatOverride>(entityInQueryIndex, entity,
                        new HeartbeatFrequencyFloatOverride { Value = highlightable.heartbeatFrequency }, heartbeatFrequencyGetter, parallelWriter);
                    ModifyEntityUtilities.AddOrSetData<FresnelValueFloatOverride>(entityInQueryIndex, entity,
                        new FresnelValueFloatOverride { Value = highlightable.fresnelValue }, fresnelValueGetter, parallelWriter);
                }
                else
                {
                    ModifyEntityUtilities.AddOrSetData<EmissionActiveFloatOverride>(entityInQueryIndex, entity,
                        new EmissionActiveFloatOverride { Value = 0.0f }, emissionActiveGetter, parallelWriter);
                }
            }).ScheduleParallel();
            this.Dependency.Complete();

            // Run the command buffer
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}