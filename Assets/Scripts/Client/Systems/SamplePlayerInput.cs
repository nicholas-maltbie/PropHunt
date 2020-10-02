
using Unity.Entities;
using Unity.NetCode;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Commands;
using UnityEngine;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

namespace PropHunt.Client.Systems
{
    /// <summary>
    /// Systemt to sample player input at each tick.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    [UpdateAfter(typeof(MenuManagerSystem))]
    public class SamplePlayerInput : ComponentSystem
    {

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<NetworkIdComponent>();
        }

        protected override void OnUpdate()
        {
            var localInput = GetSingleton<CommandTargetComponent>().targetEntity;
            var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
            if (localInput == Entity.Null)
            {
                Entities.WithNone<PlayerInput>().ForEach((Entity ent, ref PlayerId playerId) =>
                {
                    if (playerId.playerId == localPlayerId)
                    {
                        PostUpdateCommands.AddBuffer<PlayerInput>(ent);
                        PostUpdateCommands.SetComponent(GetSingletonEntity<CommandTargetComponent>(), new CommandTargetComponent { targetEntity = ent });
                    }
                });
                return;
            }

            float targetPitch = 0;
            float targetYaw = 0;
            var input = default(PlayerInput);
            input.tick = World.GetExistingSystem<ClientSimulationSystemGroup>().ServerTick;

            if (MenuManagerSystem.MovementState == LockedInputState.ALLOW)
            {
                float pitchChange = Input.GetAxis("Mouse Y");
                float yawChange = Input.GetAxis("Mouse X");

                float deltaTime = Time.DeltaTime;
                Entities.ForEach((ref PlayerView pv, ref PlayerId playerId, ref Rotation rotation) =>
                {
                    if (playerId.playerId == localPlayerId)
                    {
                        pv.pitch += deltaTime * -1 * pitchChange * pv.viewRotationRate;
                        pv.yaw += deltaTime * yawChange * pv.viewRotationRate;

                        if (pv.pitch > pv.maxPitch)
                        {
                            pv.pitch = pv.maxPitch;
                        }
                        else if (pv.pitch < pv.minPitch)
                        {
                            pv.pitch = pv.minPitch;
                        }

                        // PlayerViewUtility.UpdatePlayerView(pitchChange, yawChange, ref playerView, deltaTime, 90, -90);
                        Debug.Log($"intended change: {pitchChange}, {yawChange}");
                        Debug.Log($"Current player view: {pv.pitch}, {pv.yaw}");

                        targetPitch = pv.pitch;
                        targetYaw = pv.yaw;

                        rotation.Value.value = quaternion.Euler(new float3(0, math.radians(pv.yaw), 0)).value;
                    }
                });

                input.horizMove = Input.GetAxis("Horizontal");
                input.vertMove = Input.GetAxis("Vertical");
                input.interact = (byte)(Input.GetButtonDown("Interact") ? 1 : 0);
                input.jump = (byte)(Input.GetButton("Jump") ? 1 : 0);
                input.sprint = (byte)(Input.GetButton("Sprint") ? 1 : 0);
            }
            else
            {
                input.horizMove = 0;
                input.vertMove = 0;
                input.interact = 0;
                input.jump = 0;
                input.sprint = 0;
            }
            input.targetPitch = targetPitch;
            input.targetYaw = targetYaw;

            var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
            inputBuffer.AddCommandData(input);
        }
    }

}