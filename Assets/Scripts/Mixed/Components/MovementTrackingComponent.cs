using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Component to track an object's displacement and rotation during a frame
    /// </summary>
    [GenerateAuthoringComponent]
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct MovementTracking : IComponentData
    {
        /// <summary>
        /// Should momentum be transferred to players
        /// </summary>
        [GhostField]
        public bool avoidTransferMomentum;

        /// <summary>
        /// Current measured position of an object
        /// </summary>
        public float3 position;

        /// <summary>
        /// Previously measured position of an object (previous frame)
        /// </summary>
        public float3 previousPosition;

        /// <summary>
        /// An object's current attitude (rotation)
        /// </summary>
        public quaternion attitude;

        /// <summary>
        /// Previously measured attitude of an object (previous frame)
        /// </summary>
        public quaternion previousAttitude;

        /// <summary>
        /// Number of times this tracking component has been updated
        /// </summary>
        private int updates;

        /// <summary>
        /// Has this been properly initialized? (needs at least two samples to be initialized)
        /// </summary>
        private bool Initialized => updates >= 2;

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

        public static float3 GetDisplacementGivenChange(float3 displacement, quaternion changeAttitude, float3 relativePosition)
        {
            // Rotate point around center by change in attitude
            float3 rotatedFinalPosition = math.mul(changeAttitude, relativePosition);
            // Get the delta due to rotation
            float3 deltaRotation = rotatedFinalPosition - relativePosition;
            // Shift point by total displacement
            return deltaRotation + displacement;
        }

        /// <summary>
        /// Gets the absolute displacement of a point relative to the
        /// tracking object based on the current change in attitude and position
        /// </summary>
        /// <param name="tracking"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float3 GetDisplacementAtPoint(MovementTracking track, float3 point)
        {
            // Get relative position to previous start
            float3 relativePosition = point - track.previousPosition;
            return GetDisplacementGivenChange(track.Displacement, track.ChangeAttitude, relativePosition);
        }

        /// <summary>
        /// Updates the position and attitude of a movement tracking object based on the frame
        /// </summary>
        /// <param name="track">Movement tracking object</param>
        /// <param name="newPosition">New position of the object</param>
        /// <param name="newAttitude">Previous position of the object</param>
        public static void UpdateState(ref MovementTracking track, float3 newPosition, quaternion newAttitude)
        {
            // Update state
            track.previousPosition = track.position;
            track.previousAttitude = track.attitude;
            track.attitude = newAttitude;
            track.position = newPosition;

            // Ensure initialized if not already
            track.updates += 1;

            track.ChangeAttitude = track.Initialized ? math.mul(track.attitude, math.inverse(track.previousAttitude)) : quaternion.Euler(float3.zero);
            track.Displacement = track.Initialized ? track.position - track.previousPosition : float3.zero;
        }
    }
}