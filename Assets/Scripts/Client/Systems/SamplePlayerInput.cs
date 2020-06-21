
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
    public class SamplePlayerInput : ComponentSystem
    {
        /// <summary>
        /// Enum to control locking the current player input. This could be for things such
        /// as a pause menu or other options.
        /// </summary>
        private enum LockedInputState {ALLOW, DENY};

        /// <summary>
        /// Current movement input state of the player
        /// </summary>
        private LockedInputState movementState = LockedInputState.ALLOW;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<NetworkIdComponent>();
            RequireSingletonForUpdate<EnableProphuntGhostReceiveSystemComponent>();
        }

        protected override void OnUpdate()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    this.movementState = LockedInputState.DENY;
                }
                else if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    this.movementState = LockedInputState.ALLOW;
                }
            }

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

            if (this.movementState == LockedInputState.ALLOW) 
            {
                input.horizMove = Input.GetAxis("Horizontal");
                input.vertMove  = Input.GetAxis("Vertical");
                input.pitchChange = Input.GetAxis("Mouse Y");
                input.yawChange = Input.GetAxis("Mouse X");
                input.interact = (byte) (Input.GetButtonDown("Interact") ? 1 : 0);
                input.jump = (byte) (Input.GetButtonDown("Jump") ? 1 : 0);
            }
            
            var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
            inputBuffer.AddCommandData(input);
        }
    }

}
