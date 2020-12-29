using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Component for things that are highlighted when you look at them.
    /// </summary>
    [GhostComponent]
    [GenerateAuthoringComponent]
    public struct HighlightableComponent : IComponentData
    {
        /// <summary>
        /// Color of the highlight emission
        /// </summary>
        public Color emissionColor;

        /// <summary>
        /// Set whether object has a 'heartbeat' effect when emission is active
        /// </summary>
        public bool hasHeartbeat;

        /// <summary>
        /// Set fresnel value of hightlight
        /// </summary>
        public float fresnelValue;

        /// <summary>
        /// Set heartbeat speed of highlight
        /// </summary>
        public float heartbeatSpeed;

        /// <summary>
        /// Emission color as a float4 value
        /// </summary>
        public float4 EmissionColor => new float4(emissionColor.r, emissionColor.g, emissionColor.b, emissionColor.a);
    }
}