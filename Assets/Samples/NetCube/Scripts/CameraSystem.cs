using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
[UpdateAfter(typeof(MoveCubeSystem))]
public class CameraSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        RequireSingletonForUpdate<NetworkIdComponent>();
    }

    protected override void OnUpdate()
    {
        int localPlayerId = GetSingleton<NetworkIdComponent>().Value;

        float3 position = Camera.main.transform.position;
        quaternion rotation = Camera.main.transform.rotation;
        Entities.
            ForEach(
                (ref Translation transform, ref Rotation rot, ref MovableCubeComponent cube) =>
                {
                    if (cube.PlayerId == localPlayerId) {
                        position.x = transform.Value.x;
                        position.y = transform.Value.y;
                        position.z = transform.Value.z;
                        rotation = rot.Value;
                    }
                }
            );
 
        if (Input.GetButtonDown("Cancel"))
        {
            if (Cursor.lockState == CursorLockMode.None)
                Cursor.lockState = CursorLockMode.Locked;
            else if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
        }

        Camera.main.transform.position = position;
        Camera.main.transform.rotation = rotation;
    }
}
