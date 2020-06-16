
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

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
            ref MovableCubeComponent cube) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;
            CubeInput input;
            inputBuffer.GetDataAtTick(tick, out input);

            cube.pitch += deltaTime * -1 * input.pitch;
            cube.yaw += deltaTime * input.yaw;
            
            if (cube.pitch > math.PI / 2)
            {
                cube.pitch = math.PI / 2;
            }
            else if (cube.pitch < -math.PI / 2)
            {
                cube.pitch = -math.PI / 2;
            }

            rot.Value.value = quaternion.Euler(new float3(cube.pitch, cube.yaw, 0)).value;

            // Rotate movement vector around current attitude (only care about horizontal)
            float3 direction = math.normalizesafe(new float3(input.horizontal, 0, input.vertical));


            // Adjust position by movement
            trans.Value += math.mul(quaternion.Euler(0, cube.yaw, 0), direction) * 3 * deltaTime;
        });
    }
}

