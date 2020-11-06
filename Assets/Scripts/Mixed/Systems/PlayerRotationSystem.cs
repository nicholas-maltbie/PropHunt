using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Systems;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace PropHunt.Server.Mixed
{

    /// <summary>
    /// Player movement system that moves player controlled objects from
    /// a given player's input. Rotates the player view and object
    /// based on the character's current viewport.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    [UpdateBefore(typeof(KCCUpdateGroup))]
    public class PlayerRotationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
            var tick = group.PredictingTick;
            var deltaTime = Time.DeltaTime;
            var isClient = World.GetExistingSystem<ClientSimulationSystemGroup>() != null;

            Entities.ForEach((
                DynamicBuffer<PlayerInput> inputBuffer,
                ref PlayerView view,
                ref Rotation rot,
                in PlayerId playerId,
                in PredictedGhostComponent prediction) =>
            {
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