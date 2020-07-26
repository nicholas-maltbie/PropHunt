
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace PropHunt.Mixed.Systems
{

    /// <summary>
    /// Player movement system that moves player controlled objects from
    /// a given player's input. Rotates the player view and object
    /// based on the character's current viewport.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    public class PlayerRotationSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
            var tick = group.PredictingTick;
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((DynamicBuffer<PlayerInput> inputBuffer,
                ref PredictedGhostComponent prediction,
                ref PlayerView view, ref PlayerMovement settings,
                ref Translation trans, ref Rotation rot) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                {
                    return;
                }

                PlayerInput input;
                inputBuffer.GetDataAtTick(tick, out input);
                
                view.pitch += deltaTime * -1 * input.pitchChange * settings.viewRotationRate;
                view.yaw += deltaTime * input.yawChange * settings.viewRotationRate;

                if (view.pitch > math.PI / 2)
                {
                    view.pitch = math.PI / 2;
                }
                else if (view.pitch < -math.PI / 2)
                {
                    view.pitch = -math.PI / 2;
                }

                rot.Value.value = quaternion.Euler(new float3(0, view.yaw, 0)).value;
            });
        }

    }

}
