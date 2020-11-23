using Unity.Mathematics;

namespace PropHunt.Constants
{
    /// <summary>
    /// Database for menu screen names
    /// </summary>
    public static class KCCConstants
    {
        /// <summary>
        /// Maximum angle between an object and a character 
        /// </summary>
        public static readonly float MaxAngleShoveRadians = math.radians(90.0f);

        /// <summary>
        /// Small distance for acccounting for non deterministic simulation and float errors
        /// </summary>
        public static readonly float Epsilon = 0.001f;
    }
}