
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PropHunt.Mixed.Components
{
    public struct MovementTracking : IComponentData
    {
        public float3 position;

        private float3 previousPosition;

        public quaternion attitude;

        private quaternion previousAttitude;

        private int updates;

        private bool Initialized => updates >= 2;

        public float3 Displacement => Initialized ? position - previousPosition : float3.zero;

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
