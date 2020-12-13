using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utilities;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{

    /// <summary>
    /// Player movement system that moves player controlled objects from
    /// a given player's input. Rotates the player view and object
    /// based on the character's current viewport.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateBefore(typeof(KCCUpdateGroup))]
    public class PlayerRotationSystem : PredictionStateSystem
    {
        protected override void OnUpdate()
        {
            var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
            var tick = predictionManager.GetPredictingTick(base.World);

            Entities.ForEach((
                DynamicBuffer<PlayerInput> inputBuffer,
                ref PlayerView view,
                ref Rotation rot,
                in PlayerId playerId,
                in PredictedGhostComponent prediction) =>
            {
                // Don't rotate on client, client is handled locally
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                {
                    return;
                }

                PlayerInput input;
                inputBuffer.GetDataAtTick(tick, out input);
                view.pitch = input.targetPitch;
                view.yaw = input.targetYaw;
                rot.Value.value = quaternion.Euler(new float3(0, math.radians(view.yaw), 0)).value;
            }).ScheduleParallel();
        }
    }
}