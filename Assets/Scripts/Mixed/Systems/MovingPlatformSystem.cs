
using PropHunt.InputManagement;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utilities;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System to update a moving platform's velocity to follow the current system
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateAfter(typeof(KCCPreUpdateGroup))]
    [UpdateBefore(typeof(KCCUpdateGroup))]
    public class MovingPlatformSystem : PredictionStateSystem
    {
        protected override void OnUpdate()
        {
            float deltaTime = this.unityService.GetDeltaTime(base.Time);
            uint tick = this.predictionManager.GetPredictingTick(base.World);
            Entities.ForEach((
                ref MovingPlatform movingPlatform,
                ref Translation translation,
                ref MovementTracking tracking,
                in DynamicBuffer<MovingPlatformTarget> platformTargets,
                in PredictedGhostComponent predicted) =>
                {
                    DynamicBuffer<float3> targets = platformTargets.Reinterpret<float3>();
                    float3 currentTarget = targets[movingPlatform.current];
                    float dist = math.distance(translation.Value, currentTarget);
                    float movement = movingPlatform.speed * deltaTime;

                    if (dist <= movement)
                    {
                        // go to next target
                        int nextPlatform = movingPlatform.current + movingPlatform.direction;
                        // Adjust by current rule if out of bounds
                        if (nextPlatform < 0 || nextPlatform >= targets.Length)
                        {
                            if (movingPlatform.loopMethod == PlatformLooping.REVERSE)
                            {
                                // reverse direction
                                movingPlatform.direction *= -1;
                                // update next platform
                                nextPlatform = movingPlatform.current + movingPlatform.direction;
                            }
                            else if (movingPlatform.loopMethod == PlatformLooping.CYCLE)
                            {
                                // reset to first platform
                                nextPlatform = 0;
                            }
                        }

                        movingPlatform.current = nextPlatform;
                    }

                    // Move toward current platform by speed
                    float3 dir = math.normalizesafe(currentTarget - translation.Value);
                    // Move based on direction and speed
                    float3 displacement = dir * movingPlatform.speed * deltaTime;
                    if (GhostPredictionSystemGroup.ShouldPredict(tick, predicted))
                    {
                        translation.Value += displacement;
                    }
                    tracking.Displacement = displacement;
                }
            ).ScheduleParallel();
        }
    }
}