

using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Player movement settings for when a player moveable object is
    /// being controlled.
    /// </summary>
    [GhostDefaultComponent(
        GhostDefaultComponentAttribute.Type.Server)]
    public struct PlayerMovement : IComponentData
    {
        /// <summary>
        /// Player movement speed in units per second.
        /// </summary>
        [GhostDefaultField(100, true)]
        public float moveSpeed;

        /// <summary>
        /// Multiplier for sprint speed
        /// </summary>
        [GhostDefaultField(100, true)]
        public float sprintMultiplier;

        /// <summary>
        /// Sprint speed is move speed by a multiplier
        /// </summary>
        public float SprintSpeed => moveSpeed * sprintMultiplier;

        /// <summary>
        /// Player view speed in radians per second
        /// </summary>
        [GhostDefaultField(100, true)]
        public float viewRotationRate;

    }
}