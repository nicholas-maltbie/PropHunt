using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Component to track an object's displacement and rotation during a frame
    /// </summary>
    [GenerateAuthoringComponent]
    [GhostComponent]
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
        private float3 position;

        /// <summary>
        /// Previously measured position of an object (previous frame)
        /// </summary>
        private float3 previousPosition;

        /// <summary>
        /// An object's current attitude (rotation)
        /// </summary>
        private quaternion attitude;

        /// <summary>
        /// Previously measured attitude of an object (previous frame)
        /// </summary>
        private quaternion previousAttitude;

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
        private quaternion ChangeAttitude => Initialized ? math.mul(attitude, math.inverse(previousAttitude)) : quaternion.identity;

        /// <summary>
        /// Displacement between current and previous frames
        /// </summary>
        private float3 Displacement => Initialized ? position - previousPosition : float3.zero;

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
            // Rotate point around center by change in attitude
            float3 rotatedFinalPosition = math.mul(track.ChangeAttitude, relativePosition);
            // Get the delta due to rotation
            float3 deltaRotation = rotatedFinalPosition - relativePosition;
            // Shift point by total displacement
            return deltaRotation + track.Displacement;
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
        }
    }
}