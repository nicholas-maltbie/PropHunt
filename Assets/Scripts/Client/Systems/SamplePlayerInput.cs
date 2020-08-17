
using Unity.Entities;
using Unity.NetCode;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Commands;
using UnityEngine;
using Unity.Burst;

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
            RequireSingletonForUpdate<EnableProphuntGhostReceiveSystemComponent>();
        }

        protected override void OnUpdate()
        {
            var localInput = GetSingleton<CommandTargetComponent>().targetEntity;
            if (localInput == Entity.Null)
            {
                var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
                Entities.WithNone<PlayerInput>().ForEach((Entity ent, ref PlayerId playerId) =>
                {
                    if (playerId.playerId == localPlayerId)
                    {
                        PostUpdateCommands.AddBuffer<PlayerInput>(ent);
                        PostUpdateCommands.SetComponent(GetSingletonEntity<CommandTargetComponent>(), new CommandTargetComponent {targetEntity = ent});
                    }
                });
                return;
            }
            var input = default(PlayerInput);
            input.tick = World.GetExistingSystem<ClientSimulationSystemGroup>().ServerTick;

            if (MenuManagerSystem.MovementState == LockedInputState.ALLOW) 
            {
                input.horizMove = Input.GetAxis("Horizontal");
                input.vertMove  = Input.GetAxis("Vertical");
                input.pitchChange = Input.GetAxis("Mouse Y");
                input.yawChange = Input.GetAxis("Mouse X");
                input.interact = (byte) (Input.GetButtonDown("Interact") ? 1 : 0);
                input.jump = (byte) (Input.GetButton("Jump") ? 1 : 0);
                input.sprint = (byte) (Input.GetButton("Sprint") ? 1 : 0);
            }
            else
            {
                input.horizMove = 0;
                input.vertMove  = 0;
                input.pitchChange = 0;
                input.yawChange = 0;
                input.interact = 0;
                input.jump = 0;
                input.sprint = 0;
            }
            
            var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
            inputBuffer.AddCommandData(input);
        }
    }

}
