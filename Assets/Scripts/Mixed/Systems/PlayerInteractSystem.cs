using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System for resetting the interactable state of each object.
    /// This will by default set an entity's interactable state to false for this frame unless otherwise set true. 
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(InteractableUpdateGroup))]
    public class PlayerInteractSystem : PredictedStateSystem
    {
        protected override void OnUpdate()
        {
            var interactableGetter = base.GetComponentDataFromEntity<Interactable>(true);
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            var parallelWriter = ecb.AsParallelWriter();
            var tick = this.predictionManager.GetPredictingTick(base.World);

            Entities.WithReadOnly(interactableGetter)
                .ForEach((
                Entity entity,
                int entityInQueryIndex,
                DynamicBuffer<PlayerInput> inputBuffer,
                in FocusDetection focus,
                in PredictedGhostComponent predicted
            ) =>
            {
                // Only update the interact state if this should be predicted by the player
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, predicted))
                {
                    return;
                }

                // Get the current player input at this tick
                inputBuffer.GetDataAtTick(tick, out PlayerInput input);

                // If the player is interacting and looking at an object, set the interaction state of that object
                //  to be 'true'
                if (focus.lookObject != Entity.Null && input.IsInteracting && interactableGetter.HasComponent(focus.lookObject))
                {
                    Interactable newInteractedState = interactableGetter[focus.lookObject];
                    newInteractedState.interacted = true;
                    parallelWriter.SetComponent<Interactable>(entityInQueryIndex, focus.lookObject, newInteractedState);
                }
            }).ScheduleParallel();
            this.Dependency.Complete();

            // Set the focus state of objects
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}