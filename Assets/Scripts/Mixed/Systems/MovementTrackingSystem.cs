
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System to track moving of objects
    /// </summary>
    [UpdateInGroup(typeof(GhostSimulationSystemGroup))]
    public class MovementTrackingSystemTrack : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((
                ref MovementTracking movementTracking,
                in Translation translation,
                in Rotation rotation) =>
                {
                    MovementTracking.UpdateState(ref movementTracking, translation.Value, rotation.Value);
                }).Schedule();
        }
    }
}
