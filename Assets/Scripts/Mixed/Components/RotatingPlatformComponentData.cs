
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Target for a moving platform
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct RotatingPlatformTarget : IBufferElementData
    {
        /// <summary>
        /// Position of target
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float3 target;
    }

    /// <summary>
    /// Rotating platform component data for current state
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct RotatingPlatform : IComponentData
    {
        /// <summary>
        /// Speed at which this platform rotates between its targets in degrees per second
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float speed;

        /// <summary>
        /// Current angle of the platform
        /// </summary>
        public float3 currentAngle;

        /// <summary>
        /// Method of looping when target list is exhausted
        /// /// </summary>
        [GhostField]
        public PlatformLooping loopMethod;

        /// <summary>
        /// Current platform this is moving toward
        /// </summary>
        [GhostField]
        public int current;

        /// <summary>
        /// Direction of next platform being selected. This
        /// is decided by loop method when the list ends
        /// </summary>
        [GhostField]
        public int direction;
    }
}