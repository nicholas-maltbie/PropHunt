
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System to update a moving platform's velocity to follow the current system
    /// </summary>
    [UpdateBefore(typeof(MovementTrackingSystem))]
    public class MovingPlatformSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.ForEach((
                ref PhysicsVelocity pv,
                ref MovingPlatform movingPlatform,
                in Translation translation,
                in DynamicBuffer<MovingPlatformTarget> platformTargets) =>
                {
                    if (movingPlatform.elapsedWaiting < movingPlatform.delayBetweenPlatforms)
                    {
                        movingPlatform.elapsedWaiting += deltaTime;
                        pv.Linear = 0;
                        return;
                    }
                    DynamicBuffer<float3> targets = platformTargets.Reinterpret<float3>();
                    float3 currentTarget = targets[movingPlatform.current];
                    float dist = math.distance(translation.Value, currentTarget);
                    float movement = movingPlatform.speed * deltaTime;
                    if (dist <= movement)
                    {
                        movingPlatform.elapsedWaiting = 0;
                        // go to next target
                        int nextPlatform = movingPlatform.current + movingPlatform.direction;
                        // Adjust by current rule if out of bounds
                        if (nextPlatform < 0 || nextPlatform > movingPlatform.direction)
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
                    // Set physics velocity based on speed and direction
                    pv.Linear = dir * movingPlatform.speed;
                }
            ).ScheduleParallel();
        }
    }
}