using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System for detecting objects that a focus detection is looking at
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateAfter(typeof(KCCUpdateGroup))]
    public class FocusDetectionSystem : PredictedStateSystem
    {
        protected override unsafe void OnUpdate()
        {
            CollisionWorld collisionWorld = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld.CollisionWorld;
            var tick = this.predictionManager.GetPredictingTick(base.World);

            Entities.WithReadOnly(collisionWorld)
                .ForEach((
                Entity entity,
                ref FocusDetection focus,
                in PlayerView playerView,
                in PredictedGhostComponent predicted,
                in Translation translation
            ) =>
            {
                // Only get the focused object if this is the current player
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, predicted))
                {
                    return;
                }

                // Send out a sphere cast based on the focus settings
                // CollisionFilter, collides with everything
                var filter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u, // all 1s, so all layers, collide with everything
                    GroupIndex = 0
                };
                // Create a sphere based on settings
                SphereGeometry sphereGeometry = new SphereGeometry() { Center = float3.zero, Radius = focus.focusRadius };
                BlobAssetReference<Collider> sphereCollider = SphereCollider.Create(sphereGeometry, filter);
                // Get points of sphere cast
                float3 castStart = translation.Value + playerView.offset + playerView.Forward * focus.focusOffset;
                float3 castEnd = castStart + playerView.Forward * focus.focusDistance;
                // setup collider cast
                ColliderCastInput input = new ColliderCastInput()
                {
                    Collider = (Collider*)sphereCollider.GetUnsafePtr(),
                    Orientation = quaternion.identity,
                    Start = castStart,
                    End = castEnd
                };
                // Setup hit collector
                SelfFilteringClosestHitCollector<ColliderCastHit> hitCollector =
                    new SelfFilteringClosestHitCollector<ColliderCastHit>(entity.Index, 1.0f, collisionWorld);

                // Check if the player is looking at something
                bool collisionOcurred = collisionWorld.CastCollider(input, ref hitCollector);

                if (collisionOcurred)
                {
                    // Get the thing that is hit
                    ColliderCastHit hit = hitCollector.ClosestHit;
                    focus.focusedObject = hit.Entity;
                    focus.focusedDistance = focus.focusOffset + hit.Fraction * math.length(castEnd - castStart);
                }
                else
                {
                    // Setup the hit to be nothing
                    focus.focusedObject = Entity.Null;
                    focus.focusedDistance = -1;
                }
            }).ScheduleParallel();
            this.Dependency.Complete();
        }
    }
}