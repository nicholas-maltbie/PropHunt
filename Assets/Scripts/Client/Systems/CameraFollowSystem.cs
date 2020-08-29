using PropHunt.Mixed.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace PropHunt.Client.Systems
{

    /// <summary>
    /// Camera follow system to follow a player object.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class CameraFollowSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            Cursor.lockState = CursorLockMode.Locked;
            RequireSingletonForUpdate<NetworkIdComponent>();
            RequireSingletonForUpdate<EnableProphuntGhostReceiveSystemComponent>();
        }

        protected override void OnUpdate()
        {
            int localPlayerId = GetSingleton<NetworkIdComponent>().Value;

            Debug.Log("helloworld");

            float3 position = Camera.main.transform.position;
            quaternion rotation = Camera.main.transform.rotation;
            Entities.
                ForEach(
                    (ref Translation transform, ref Rotation rot, ref PlayerId player, ref PlayerView view) =>
                    {
                        Debug.Log($"{player.playerId}, {localPlayerId}");
                        Debug.Log($"trans {transform.Value}, rot {rot.Value}");
                        if (player.playerId == localPlayerId) {
                            position.x = transform.Value.x;
                            position.y = transform.Value.y;
                            position.z = transform.Value.z;
                            position += view.offset;
                            rotation.value = quaternion.Euler(math.radians(view.pitch), math.radians(view.yaw), 0).value;
                        }
                    }
                );

            Camera.main.transform.position = position;
            Camera.main.transform.rotation = rotation;
        }
    }
}
