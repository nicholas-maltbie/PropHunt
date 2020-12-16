
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Rotating platform component data for current state
    /// </summary>
    [GenerateAuthoringComponent]
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct RotatingObject : IComponentData
    {
        /// <summary>
        /// Speed at which this platform rotates between its targets in degrees per second
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float3 angularVelocity;
    }
}