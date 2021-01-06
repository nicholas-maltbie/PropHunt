
using PropHunt.InputManagement;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// Update group for controlling doors in the game
    /// </summary>
    [UpdateInGroup(typeof(InteractableListenGroup))]
    public class DoorUpdateGroup : ComponentSystemGroup { }

    /// <summary>
    /// System to handle rotating or moving doors based on their current state
    /// </summary>
    [UpdateInGroup(typeof(DoorUpdateGroup))]
    public class DoorPositionUpdateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((
                Entity entity,
                ref Translation translation,
                ref Rotation rotation,
                in Door doorState
            ) =>
            {
                // Update door position based on current state and progress between two states.
                if (doorState.state == DoorState.Opened)
                {
                    translation.Value = doorState.openedPosition;
                    rotation.Value = quaternion.Euler(doorState.openedRotation);
                }
                else if (doorState.state == DoorState.Closed)
                {
                    translation.Value = doorState.closedPosition;
                    rotation.Value = quaternion.Euler(doorState.closedRotation);
                }
                else
                {
                    // Get the origin and target positions of the door
                    float3 originPosition = doorState.state == DoorState.Closing ? doorState.openedPosition : doorState.closedPosition;
                    float3 targetPosition = doorState.state == DoorState.Closing ? doorState.closedPosition : doorState.openedPosition;
                    float3 originRotation = doorState.state == DoorState.Closing ? doorState.openedRotation : doorState.closedRotation;
                    float3 targetRotation = doorState.state == DoorState.Closing ? doorState.closedRotation : doorState.openedRotation;
                    // Set the door state to be the proportional transition between these two states
                    translation.Value = originPosition + (targetPosition - originPosition) * doorState.TransitionProgress;
                    // q1 - q2 = q2 * inverse(q1)
                    rotation.Value = quaternion.Euler(originRotation +
                        RotatingPlatformSystem.ShortestAngleBetween(originRotation, targetRotation) * doorState.TransitionProgress);
                }
            }).ScheduleParallel();
        }
    }

    /// <summary>
    /// State machine to handle updating the door state based on interactions
    /// and from 
    /// </summary>
    [UpdateInGroup(typeof(DoorUpdateGroup), OrderFirst = true)]
    public class DoorStateMachineUpdateSystem : SystemBase
    {
        /// <summary>
        /// Unity service that can get delta time for unit testing
        /// </summary>
        public IUnityService unityService = new UnityService();

        protected override void OnUpdate()
        {
            float deltaTime = unityService.GetDeltaTime(base.Time);
            bool isServer = base.World.GetExistingSystem<ServerSimulationSystemGroup>() != null;

            if (!isServer)
            {
                return;
            }

            Entities.ForEach((
                Entity entity,
                ref Door door,
                in Interactable interactable
            ) =>
            {
                DoorState currentState = door.state;

                // Change state if the player starts to interacts with the door
                if (interactable.InteractStart)
                {
                    // If a player interacts, change state depending on the current state of the floor
                    if (currentState == DoorState.Opened)
                    {
                        currentState = DoorState.Closing;
                    }
                    else if (currentState == DoorState.Closed)
                    {
                        currentState = DoorState.Opening;
                    }
                    else if (currentState == DoorState.Opening)
                    {
                        currentState = DoorState.Closing;
                        // Invert the current transition progress as we are now moving backwards
                        door.elapsedTransitionTime = door.transitionTime - door.elapsedTransitionTime;
                    }
                    else if (currentState == DoorState.Closing)
                    {
                        currentState = DoorState.Opening;
                        // Invert the current transition progress as we are now moving backwards
                        door.elapsedTransitionTime = door.transitionTime - door.elapsedTransitionTime;
                    }
                }

                // If opening or closing, increment elapsed transition time
                if (currentState == DoorState.Opening || currentState == DoorState.Closing)
                {
                    door.elapsedTransitionTime += deltaTime;
                    // If current transition is completed
                    if (door.TransitionProgress >= 1)
                    {
                        currentState = currentState == DoorState.Opening ? DoorState.Opened : DoorState.Closed;
                        // Finish current transition
                        door.elapsedTransitionTime = 0;
                    }
                }

                // update state of the door
                door.state = currentState;
            }).ScheduleParallel();
        }
    }

}