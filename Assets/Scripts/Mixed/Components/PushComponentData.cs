
using Unity.Entities;
using Unity.Mathematics;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Pushing being applied to an entity
    /// </summary>
    public struct PushForce : IComponentData
    {
        /// <summary>
        /// Force and direction of push event
        /// </summary>
        public float3 force;

        /// <summary>
        /// Point where the hit occurs
        /// </summary>
        public float3 point;
    }
}