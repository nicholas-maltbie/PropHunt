using PropHunt.Mixed.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// Update the state of interactable objects in this group
    /// </summary>
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateAfter(typeof(FocusDetectionSystem))]
    public class InteractableUpdateGroup : ComponentSystemGroup { }

    /// <summary>
    /// update group for systems that want to listen to the state of interactable objects
    /// </summary>
    [UpdateInGroup(typeof(GhostSimulationSystemGroup))]
    [UpdateAfter(typeof(GhostPredictionSystemGroup))]
    public class InteractableListenGroup : ComponentSystemGroup { }

    /// <summary>
    /// System for resetting the interactable state of each object.
    /// This will by default set an entity's interactable state to false for this frame unless otherwise set true. 
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(InteractableUpdateGroup), OrderFirst = true)]
    public class InteractableResetState : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((
                ref Interactable interactable
            ) =>
            {
                // Save state from previous frame
                interactable.previousInteracted = interactable.interacted;
                // Update current interacted state to false
                interactable.interacted = false;
            }).ScheduleParallel();
        }
    }
}