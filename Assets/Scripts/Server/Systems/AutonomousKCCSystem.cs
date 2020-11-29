using PropHunt.InputManagement;
using PropHunt.Mixed.Components;
using PropHunt.Server.Components;
using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

namespace PropHunt.Server.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
    public class AutonomousKCCSystem : SystemBase
    {
        public IUnityService unityService = new UnityService();

        protected override void OnUpdate()
        {
            float deltaTime = this.unityService.GetDeltaTime(base.Time);

            Entities.ForEach((
                ref AutonomousKCCAgent agent,
                ref KCCVelocity velocity,
                ref Rotation rotation,
                ref KCCJumping jumping,
                ref RandomWrapper randomState) =>
            {
                // Update the agent state
                // Decrement remaining time to next changes
                agent.remainingTimeDirectionChange -= deltaTime;
                agent.remainingTimeJump -= deltaTime;

                // If the player's time to next direction change is exceed, change direction
                if (agent.remainingTimeDirectionChange <= 0)
                {
                    // Determine new direction
                    float newDirection = randomState.random.NextFloat() * math.PI * 2;

                    // Change velocity to be this direction
                    float3 movementVector = new float3(math.sin(newDirection), 0, math.cos(newDirection));
                    velocity.playerVelocity = math.normalize(movementVector) * agent.moveSpeed;

                    // Rotate the character to look this direction
                    rotation.Value = quaternion.RotateY(newDirection);

                    // Reset the time to change direction
                    float directionChangeRange = agent.maxTimeDirectionChange - agent.minTimeDirectionChange;
                    agent.remainingTimeDirectionChange = randomState.random.NextFloat() * directionChangeRange + agent.minTimeDirectionChange;
                }
                // If the player's time to jump is exceeded, jump
                if (agent.remainingTimeJump <= 0)
                {
                    // Set jump to true
                    jumping.attemptingJump = true;
                    float jumpRange = agent.maxTimeJump - agent.minTimeJump;
                    agent.remainingTimeJump = randomState.random.NextFloat() * jumpRange + agent.minTimeJump;
                }
                else
                {
                    // Set jump to false
                    jumping.attemptingJump = false;
                }
            }).ScheduleParallel();
        }
    }
}