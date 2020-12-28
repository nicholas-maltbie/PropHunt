using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Structure to hold settings and detected object when focusing (interacting) with
    /// other entities in the world.
    /// This must be paired with a PlayerView component to detect objects and where a player
    /// is currently looking.
    /// </summary>
    [GenerateAuthoringComponent]
    [GhostComponent]
    public struct FocusDetection : IComponentData
    {
        /// <summary>
        /// Distance to check for objects
        /// </summary>
        public float focusDistance;

        /// <summary>
        /// Radius of the sphere used to detect objects
        /// </summary>
        public float focusRadius;

        /// <summary>
        /// Offset of focus from the camera location
        /// </summary>
        public float focusOffset;

        /// <summary>
        /// Distance to the object the player is looking at
        /// </summary>
        public float lookDistance;

        /// <summary>
        /// Object that is focused on this frame
        /// </summary>
        public Entity lookObject;

        /// <summary>
        /// Previous object the player was looking at
        /// </summary>
        public Entity previousLookObject;
    }
}