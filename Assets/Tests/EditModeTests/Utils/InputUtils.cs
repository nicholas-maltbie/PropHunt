using PropHunt.Mixed.Commands;
using Unity.Entities;

namespace Assets.Tests.EditModeTests.Utils
{
    /// <summary>
    /// Utils class that include some helpers to handle input.
    /// </summary>
    public class InputUtils
    {
        /// <summary>
        /// Adds a new Input to the player entity provided.
        /// </summary>
        /// <param name="entityManager">The current entity manager to use.</param>
        /// <param name="entity">The entity with a Player Input component.</param>
        /// <param name="currentTick">The current tick.</param>
        /// <param name="horizMove">The horizontal move, goes from 0 to 1.</param>
        /// <param name="vertMove">The vertical move, goes from 0 to 1.</param>
        /// <param name="targetYaw">The target yaw.</param>
        /// <param name="targetPitch">The target pitch.</param>
        /// <param name="jump">The jump action state (0 or 1).</param>
        /// <param name="interact">The interact action state (0 or 1).</param>
        /// <param name="sprint">The sprint action state (0 or 1).</param>
        public static void AddInput(EntityManager entityManager, Entity entity, uint currentTick = 0, float horizMove = 0.0f, float vertMove = 0.0f, float targetYaw = 0.0f, float targetPitch = 0.0f, byte jump = 0, byte interact = 0, byte sprint = 0)
        {
            var inputBuffer = entityManager.GetBuffer<PlayerInput>(entity);
            inputBuffer.Add(new PlayerInput
            {
                tick = currentTick,
                horizMove = horizMove,
                vertMove = vertMove,
                targetPitch = targetPitch,
                targetYaw = targetYaw,
                interact = interact,
                jump = jump,
                sprint = sprint
            });
        }
    }
}