
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class MoveCubeSystem : ComponentSystem
{
    
    protected override void OnUpdate()
    {
        GhostPredictionSystemGroup group = World.GetExistingSystem<GhostPredictionSystemGroup>();
        uint tick = group.PredictingTick;
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((DynamicBuffer<CubeInput> inputBuffer,
            ref Translation trans, ref Rotation rot, ref PredictedGhostComponent prediction,
            ref MovableCubeComponent cube, ref PlayerView view) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;
            CubeInput input;
            inputBuffer.GetDataAtTick(tick, out input);

            view.pitch += deltaTime * -1 * input.pitch;
            view.yaw += deltaTime * input.yaw;
            
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
            float3 inputVector = new float3(input.horizontal, 0, input.vertical);
            // Don't allow the total movement to be more than the 1x max move speed
            float3 direction = inputVector / math.max(math.length(inputVector), 1);

            // Adjust position by movement
            trans.Value += math.mul(quaternion.RotateY(view.yaw), direction) * 5 * deltaTime;
        });
    }
}

