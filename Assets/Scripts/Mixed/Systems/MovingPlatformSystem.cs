
using PropHunt.InputManagement;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System to update a moving platform's velocity to follow the current system
    /// </summary>
    [UpdateInGroup(typeof(GhostSimulationSystemGroup))]
    [UpdateBefore(typeof(MovementTrackingSystem))]
    public class MovingPlatformSystem : SystemBase
    {
        /// <summary>
        /// Unity service for accessing unity data
        /// </summary>
        public IUnityService unityService = new UnityService();

        protected override void OnUpdate()
        {
            float deltaTime = this.unityService.GetDeltaTime(base.Time);
            Entities.ForEach((
                ref MovingPlatform movingPlatform,
                ref Translation translation,
                ref PhysicsVelocity velocity,
                in DynamicBuffer<MovingPlatformTarget> platformTargets) =>
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
                    // Set physics velocity based on speed and direction
                    velocity.Linear = dir * movingPlatform.speed;
                }
            ).ScheduleParallel();
        }
    }
}