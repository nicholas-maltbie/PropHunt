
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
    /// a given player's input.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    public class PlayerMovementSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
            var tick = group.PredictingTick;
            var deltaTime = Time.DeltaTime;
            Entities.ForEach((DynamicBuffer<PlayerInput> inputBuffer, ref PredictedGhostComponent prediction, 
                ref Translation trans, ref Rotation rot, ref PlayerView view) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                {
                    return;
                }

                PlayerInput input;
                inputBuffer.GetDataAtTick(tick, out input);

                view.pitch += deltaTime * -1 * input.pitchChange;
                view.yaw += deltaTime * input.yawChange;
                
                if (view.pitch > math.PI / 2)
                {
                    view.pitch = math.PI / 2;
                }
                else if (view.pitch < -math.PI / 2)
                {
                    view.pitch = -math.PI / 2;
                }

                rot.Value.value = quaternion.Euler(new float3(view.pitch, view.yaw, 0)).value;

                // Rotate movement vector around current attitude (only care about horizontal)
                float3 inputVector = new float3(input.horizMove, 0, input.vertMove);
                // Don't allow the total movement to be more than the 1x max move speed
                float3 direction = inputVector / math.max(math.length(inputVector), 1);

                // Adjust position by movement
                trans.Value += math.mul(quaternion.RotateY(view.yaw), direction) * 5 * deltaTime;
            });
        }

    }

}
