
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Buffer to store forces applied to object every frame
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