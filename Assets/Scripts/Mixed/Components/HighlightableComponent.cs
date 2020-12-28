using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Component for things that are highlighted when you look at them.
    /// </summary>
    [GenerateAuthoringComponent]
    [GhostComponent]
    public struct HighlightableComponent : IComponentData
    {
        /// <summary>
        /// Color of the highlight emission
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        float4 emissionColor;

        /// <summary>
        /// Set whether object is currently being highlighted
        /// </summary>
        [GhostField]
        bool emissionIsActive;

        /// <summary>
        /// Set whether object has a 'heartbeat' effect when emission is active
        /// </summary>
        [GhostField]
        bool hasHeartbeat;

        /// <summary>
        /// Set fresnel value of hightlight
        /// </summary>
        [GhostField]
        bool fresnelValue;

        /// <summary>
        /// Set heartbeat speed of highlight
        /// </summary>
        [GhostField]
        bool heartbeatSpeed;
    }
}