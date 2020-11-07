using Unity.Mathematics;

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
        /// <returns>Ture if | value - target | <= error</returns>
        public static bool WithinErrorRange(float3 value, float3 target, float error = 0.001f)
        {
            return math.all(math.abs(value - target) <= error);
        }
    }
}
