using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Player view component data, holds the attitude of the player's
    /// view to match a camera to.
    /// </summary>
    [GenerateAuthoringComponent]
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct PlayerView : IComponentData
    {
        /// <summary>
        /// Offset from the actual object position
        /// </summary>
        [GhostField(Quantization = 1000, Interpolate = false)]
        public float3 offset;

        /// <summary>
        /// Player view speed in degrees per second
        /// </summary>
        [GhostField(Quantization = 1000, Interpolate = false)]
        public float viewRotationRate;

        /// <summary>
        /// Maximum player pitch value
        /// </summary>
        public float maxPitch;

        /// <summary>
        /// Minimum player pitch value
        /// </summary>
        public float minPitch;

        /// <summary>
        /// Current pitch of the player's view in degrees
        /// </summary>
        public float pitch;

        /// <summary>
        /// Current yaw of the player's view in degrees
        /// </summary>
        public float yaw;

        /// <summary>
        /// Get the current rotation of this player view as a quaternion
        /// </summary>
        public quaternion TargetRotation => quaternion.Euler(math.radians(this.pitch), math.radians(this.yaw), 0);

        /// <summary>
        /// Get a forward vector of the direction the player is looking
        /// </summary>
        public float3 Forward => math.mul(this.TargetRotation, new float3(0, 0, 1));
    }

}