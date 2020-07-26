using PropHunt.Mixed.Components;
using PropHunt.Mixed.Systems;
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

            float3 position = Camera.main.transform.position;
            quaternion rotation = Camera.main.transform.rotation;
            Entities.
                ForEach(
                    (ref Translation transform, ref Rotation rot, ref PlayerId player, ref PlayerView view) =>
                    {
                        if (player.playerId == localPlayerId) {
                            position.x = transform.Value.x;
                            position.y = transform.Value.y + 1.7f;
                            position.z = transform.Value.z;
                            rotation.value = quaternion.Euler(view.pitch, view.yaw, 0).value;
                        }
                    }
                );

            Camera.main.transform.position = position;
            Camera.main.transform.rotation = rotation;
        }
    }
}
