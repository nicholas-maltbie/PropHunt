
using Unity.Entities;
using Unity.Mathematics;

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
        public float speed;

        /// <summary>
        /// Method of looping when target list is exhausted
        /// </summary>
        public PlatformLooping loopMethod;

        /// <summary>
        /// Current platform this is moving toward
        /// </summary>
        public int current;

        /// <summary>
        /// Direction of next platform being selected. This
        /// is decided by loop method when the list ends
        /// </summary>
        public int direction;
    }
}
