using PropHunt.InputManagement;
using PropHunt.Mixed.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System to update a rotating platform's angular velocity to follow the current system
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateBefore(typeof(KCCUpdateGroup))]
    public class RotatingObjectSystem : SystemBase
    {
        /// <summary>
        /// Unity service for accessing unity data
        /// </summary>
        public IUnityService unityService = new UnityService();

        protected override void OnUpdate()
        {
            float deltaTime = this.unityService.GetDeltaTime(base.Time);
            Entities.ForEach((
                ref RotatingObject rotatingObject,
                ref Rotation rotation,
                ref MovementTracking movementTracking) =>
                {
                    quaternion deltaAngle = quaternion.Euler(math.radians(rotatingObject.angularVelocity * deltaTime));
                    // Add quaternions by multiplying them
                    rotation.Value = math.mul(rotation.Value, deltaAngle);
                    // Save this as the change in able
                    movementTracking.ChangeAttitude = deltaAngle;
                }
            ).ScheduleParallel();
        }
    }
}