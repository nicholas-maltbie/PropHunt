
using Unity.Entities;
using Unity.NetCode;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Commands;
using UnityEngine;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using PropHunt.InputManagement;

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
        public IUnityService unityService;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<NetworkIdComponent>();

            if (this.unityService == null)
            {
                this.unityService = new UnityService();
            }
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
            var playerInput = default(PlayerInput);
            playerInput.tick = World.GetExistingSystem<ClientSimulationSystemGroup>().ServerTick;

            float pitchChange = 0;
            float yawChange = 0;

            if (MenuManagerSystem.MovementState == LockedInputState.ALLOW)
            {
                pitchChange = this.unityService.GetAxis("Mouse Y");
                yawChange = this.unityService.GetAxis("Mouse X");
                playerInput.horizMove = this.unityService.GetAxis("Horizontal");
                playerInput.vertMove = this.unityService.GetAxis("Vertical");
                playerInput.interact = (byte)(this.unityService.GetButtonDown("Interact") ? 1 : 0);
                playerInput.jump = (byte)(this.unityService.GetButton("Jump") ? 1 : 0);
                playerInput.sprint = (byte)(this.unityService.GetButton("Sprint") ? 1 : 0);
            }
            else
            {
                playerInput.horizMove = 0;
                playerInput.vertMove = 0;
                playerInput.interact = 0;
                playerInput.jump = 0;
                playerInput.sprint = 0;
            }

            float deltaTime = unityService.GetDeltaTime();
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

                    targetPitch = pv.pitch;
                    targetYaw = pv.yaw;
                }
            });

            playerInput.targetPitch = targetPitch;
            playerInput.targetYaw = targetYaw;

            var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
            inputBuffer.AddCommandData(playerInput);
        }
    }

}