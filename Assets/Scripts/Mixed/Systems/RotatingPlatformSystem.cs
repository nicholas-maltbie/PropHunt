
using PropHunt.InputManagement;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System to update a rotating platform's angular velocity to follow the current system
    /// </summary>
    [UpdateInGroup(typeof(MovingPlatformGroup))]
    public class RotatingPlatformSystem : SystemBase
    {
        /// <summary>
        /// Unity service for accessing unity data
        /// </summary>
        public IUnityService unityService = new UnityService();

        public static float3 ShortestAngleBetween(float3 start, float3 end)
        {
            float3 delta = end - start;
            return new float3
            {
                x = delta.x > 180 ? delta.x - 360 : delta.x,
                y = delta.y > 180 ? delta.y - 360 : delta.y,
                z = delta.z > 180 ? delta.z - 360 : delta.z
            };
        }

        protected override void OnUpdate()
        {
            float deltaTime = this.unityService.GetDeltaTime(base.Time);
            Entities.ForEach((
                ref RotatingPlatform rotatingPlatform,
                ref Rotation rotation,
                in DynamicBuffer<RotatingPlatformTarget> rotationTargets) =>
                {
                    DynamicBuffer<float3> targets = rotationTargets.Reinterpret<float3>();
                    float3 currentTarget = targets[rotatingPlatform.current];
                    float degreeMovement = rotatingPlatform.speed * deltaTime;
                    float3 angleBetween = ShortestAngleBetween(rotatingPlatform.currentAngle, currentTarget);
                    float angularDist = math.length(angleBetween);
                    if (angularDist <= degreeMovement)
                    {
                        // go to next target
                        int nextPlatform = rotatingPlatform.current + rotatingPlatform.direction;
                        // Adjust by current rule if out of bounds
                        if (nextPlatform < 0 || nextPlatform >= targets.Length)
                        {
                            if (rotatingPlatform.loopMethod == PlatformLooping.REVERSE)
                            {
                                // reverse direction
                                rotatingPlatform.direction *= -1;
                                // update next platform
                                nextPlatform = rotatingPlatform.current + rotatingPlatform.direction;
                            }
                            else if (rotatingPlatform.loopMethod == PlatformLooping.CYCLE)
                            {
                                // reset to first platform
                                nextPlatform = 0;
                            }
                        }

                        rotatingPlatform.current = nextPlatform;
                    }

                    // Move toward current platform by speed
                    float3 dir = math.normalizesafe(angleBetween);

                    // Set physics velocity based on speed and direction
                    rotatingPlatform.currentAngle += dir * rotatingPlatform.speed * deltaTime;
                    rotation.Value = quaternion.Euler(math.radians(rotatingPlatform.currentAngle));
                }
            ).ScheduleParallel();
        }
    }
}