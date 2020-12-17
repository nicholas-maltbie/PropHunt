using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Component to track an object's displacement and rotation during a frame
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct MovementTracking : IComponentData
    {
        /// <summary>
        /// Should momentum be transferred to players
        /// </summary>
        [GhostField]
        public bool avoidTransferMomentum;

        /// <summary>
        /// Finds the change in attitude (expressed as a quaternion) between
        /// the current and previous attitude. QFinal * Inv(QInitial)
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = false)]
        public quaternion ChangeAttitude;

        /// <summary>
        /// Displacement between current and previous frames
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = false)]
        public float3 Displacement;

        /// <summary>
        /// Gets the absolute displacement of a point relative to the
        /// tracking object based on the current change in attitude and position
        /// </summary>
        /// <param name="tracking"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float3 GetDisplacementAtPoint(MovementTracking track, float3 relativePosition)
        {
            // Rotate point around center by change in attitude
            float3 rotatedFinalPosition = math.mul(track.ChangeAttitude, relativePosition);
            // Get the delta due to rotation
            float3 deltaRotation = rotatedFinalPosition - relativePosition;
            // Shift point by total displacement
            return deltaRotation + track.Displacement;
        }
    }
}