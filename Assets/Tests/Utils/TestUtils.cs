using Unity.Mathematics;
using UnityEngine;

namespace PropHunt.Tests.Utils
{
    public static class TestUtils
    {
        /// <summary>
        /// Verify if a vector is within a target value by some error bounds
        /// </summary>
        /// <param name="value">Vector value</param>
        /// <param name="target">Target value for comparison</param>
        /// <param name="error">Error range</param>
        /// <returns>True if | value - target | <= error</returns>
        public static bool WithinErrorRange(float3 value, float3 target, float error = 0.001f)
        {
            return math.all(math.abs(value - target) <= error);
        }

        /// <summary>
        /// Checks if a vector a float3 are equal for tests
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if all values are equal, false otherwise</returns>
        public static bool VectorEquals(float3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }        
    }
}