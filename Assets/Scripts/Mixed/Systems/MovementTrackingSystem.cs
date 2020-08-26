
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{
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
