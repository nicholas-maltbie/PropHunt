using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// State of a door, opening, closing, opened, closed
    /// </summary>
    public enum DoorState
    {
        Opened,
        Closed,
        Opening,
        Closing,
    }

    /// <summary>
    /// Component for identifying the state of a door
    /// </summary>
    [GhostComponent]
    public struct Door : IComponentData
    {
        /// <summary>
        /// Current state of the door
        /// </summary>
        [GhostField]
        public DoorState state;

        /// <summary>
        /// Time it takes to transition between open and closed state
        /// </summary>
        [GhostField]
        public float transitionTime;

        /// <summary>
        /// Elapsed time since the previous action
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = false)]
        public float elapsedTransitionTime;

        /// <summary>
        /// Position of door when opening
        /// </summary>
        public float3 openedPosition;

        /// <summary>
        /// Position of door when closing
        /// </summary>
        public float3 closedPosition;

        /// <summary>
        /// Rotation of door when opening as an euler angle measured in Radians
        /// </summary>
        public float3 openedRotation;

        /// <summary>
        /// Rotation of door when closing as an euler angle measured in Radians
        /// </summary>
        public float3 closedRotation;

        /// <summary>
        /// Returns the current transition progress as a value between 0 and 1
        /// </summary>
        public float TransitionProgress => transitionTime > 0 ? elapsedTransitionTime / transitionTime : 1;
    }

}