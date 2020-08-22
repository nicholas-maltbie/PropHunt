
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Buffer to store forces applied to object every frame.
    /// Saved as a BufferElement data due to the buffer like nature of
    /// an object's forces as it could be pushed by multiple entities
    /// </summary>
    [InternalBufferCapacity(8)]
    public struct PushForce : IBufferElementData
    {
        /// <summary>
        /// For being applied
        /// </summary>
        public float3 force;

        /// <summary>
        /// Location of force being applied
        /// </summary>
        public float3 point;
    }
}
