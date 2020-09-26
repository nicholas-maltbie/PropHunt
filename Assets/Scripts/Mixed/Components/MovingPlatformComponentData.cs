
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Looping method between platforms
    /// </summary>
    public enum PlatformLooping
    {
        // Cycle back to fist platform when done
        CYCLE,
        // Go back in reverse order
        REVERSE
    }

    /// <summary>
    /// Target for a moving platform
    /// </summary>
    public struct MovingPlatformTarget : IBufferElementData
    {
        /// <summary>
        /// Position of target
        /// </summary>
        public float3 target;
    }

    /// <summary>
    /// Moving platform component data for current state
    /// </summary>
    public struct MovingPlatform : IComponentData
    {
        /// <summary>
        /// Speed at which this platform moves between targets
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float speed;

        /// <summary>
        /// Method of looping when target list is exhausted
        /// </summary>
        [GhostField]
        public PlatformLooping loopMethod;

        /// <summary>
        /// Current platform this is moving toward
        /// </summary>
        [GhostField]
        public int current;

        /// <summary>
        /// Delay between moving to different platforms
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float delayBetweenPlatforms;

        /// <summary>
        /// Elapsed time waiting between platforms
        /// </summary>
        public float elapsedWaiting;

        /// <summary>
        /// Direction of next platform being selected. This
        /// is decided by loop method when the list ends
        /// </summary>
        [GhostField]
        public int direction;
    }
}