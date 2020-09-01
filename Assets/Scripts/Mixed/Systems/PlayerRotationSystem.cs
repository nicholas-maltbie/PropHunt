using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{

    /// <summary>
    /// Player movement system that moves player controlled objects from
    /// a given player's input. Rotates the player view and object
    /// based on the character's current viewport.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    public class PlayerRotationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
            var tick = group.PredictingTick;
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((
                DynamicBuffer<PlayerInput> inputBuffer,
                ref PlayerView view,
                ref Rotation rot,
                in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                {
                    return;
                }

                PlayerInput input;
                inputBuffer.GetDataAtTick(tick, out input);
                
                view.pitch += deltaTime * -1 * input.pitchChange * view.viewRotationRate;
                view.yaw += deltaTime * input.yawChange * view.viewRotationRate;

                if (view.pitch > 90)
                {
                    view.pitch = 90;
                }
                else if (view.pitch < -90)
                {
                    view.pitch = -90;
                }

                rot.Value.value = quaternion.Euler(new float3(0, math.radians(view.yaw), 0)).value;
            }).ScheduleParallel();
        }

    }

}
