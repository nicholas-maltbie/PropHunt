using PropHunt.InputManagement;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utilities;
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
    [UpdateAfter(typeof(KCCPreUpdateGroup))]
    [UpdateBefore(typeof(KCCUpdateGroup))]
    public class RotatingObjectSystem : PredictionStateSystem
    {
        protected override void OnUpdate()
        {
            float deltaTime = this.unityService.GetDeltaTime(base.Time);
            uint tick = this.predictionManager.GetPredictingTick(base.World);
            Entities.ForEach((
                ref RotatingObject rotatingObject,
                ref Rotation rotation,
                ref MovementTracking movementTracking,
                in PredictedGhostComponent predicted) =>
                {
                    quaternion deltaAngle = quaternion.Euler(math.radians(rotatingObject.angularVelocity * deltaTime));
                    if (GhostPredictionSystemGroup.ShouldPredict(tick, predicted))
                    {
                        // Add quaternions by multiplying them
                        rotation.Value = math.mul(rotation.Value, deltaAngle);
                    }
                    // Save this as the change in able
                    movementTracking.ChangeAttitude = deltaAngle;
                }
            ).ScheduleParallel();
        }
    }
}