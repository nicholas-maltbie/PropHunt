using Unity.Entities;

namespace PropHunt.Server.Components
{
    /// <summary>
    /// Autonomous agent to control the kinematic character controller
    /// </summary>
    [GenerateAuthoringComponent]
    public struct AutonomousKCCAgent : IComponentData
    {
        /// <summary>
        /// Minimum time between direction changes
        /// </summary>
        public float minTimeDirectionChange;

        /// <summary>
        /// Maximum time between direction changes
        /// </summary>
        public float maxTimeDirectionChange;

        /// <summary>
        /// Speed of movement when walking around (in units per second)
        /// </summary>
        public float moveSpeed;

        /// <summary>
        /// Minimum time between jumping action
        /// </summary>
        public float minTimeJump;

        /// <summary>
        /// Maximum time between jumping actions
        /// </summary>
        public float maxTimeJump;

        /// <summary>
        /// Remaining time to next direction change
        /// </summary>
        public float remainingTimeDirectionChange;

        /// <summary>
        /// Remaining time to next last jump action
        /// </summary>
        public float remainingTimeJump;
    }
}