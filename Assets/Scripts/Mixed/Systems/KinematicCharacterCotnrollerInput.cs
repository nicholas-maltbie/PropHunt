
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utilities;
using PropHunt.InputManagement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Systems
{

    /// <summary>
    /// Parse player input and set kinematic character controller to follow movement
    /// commands based on user input.
    /// </summary>
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateBefore(typeof(KCCUpdateGroup))]
    public class KinematicCharacterControllerInput : PredictionStateSystem
    {
        protected override void OnUpdate()
        {
            var tick = this.predictionManager.GetPredictingTick(base.World);
            float deltaTime = this.unityService.GetDeltaTime(base.Time);

            Entities.ForEach((
                DynamicBuffer<PlayerInput> inputBuffer,
                ref KCCVelocity velocity,
                ref KCCJumping jump,
                in PredictedGhostComponent prediction,
                in PlayerView view,
                in KCCMovementSettings settings) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                {
                    return;
                }

                inputBuffer.GetDataAtTick(tick, out PlayerInput input);

                // Rotate movement vector around current attitude (only care about horizontal)
                float3 inputVector = new float3(input.horizMove, 0, input.vertMove);
                // Don't allow the total movement to be more than the 1x max move speed
                float3 direction = inputVector / math.max(math.length(inputVector), 1);

                float speedMultiplier = input.IsSprinting ? settings.SprintSpeed : settings.moveSpeed;

                quaternion horizPlaneView = quaternion.RotateY(math.radians(view.yaw));
                // Make movement vector based on player input
                velocity.playerVelocity = math.mul(horizPlaneView, direction) * speedMultiplier;
                // including jump action
                jump.attemptingJump = input.IsJumping;
            }).ScheduleParallel();
        }
    }
}