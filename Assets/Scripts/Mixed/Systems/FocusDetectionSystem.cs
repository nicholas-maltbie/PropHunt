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
            var focusTargetGetter = base.GetComponentDataFromEntity<FocusTarget>(true);
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            var parallelWriter = ecb.AsParallelWriter();
            var tick = this.predictionManager.GetPredictingTick(base.World);

            Entities.WithReadOnly(focusTargetGetter)
                .WithReadOnly(collisionWorld)
                .ForEach((
                Entity entity,
                int entityInQueryIndex,
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

                // Save the previously looked object
                focus.previousLookObject = focus.lookObject;

                if (collisionOcurred)
                {
                    // Get the thing that is hit
                    ColliderCastHit hit = hitCollector.ClosestHit;
                    focus.lookObject = hit.Entity;
                    focus.lookDistance = focus.focusOffset + hit.Fraction * math.length(castEnd - castStart);
                }
                else
                {
                    // Setup the hit to be nothing
                    focus.lookObject = Entity.Null;
                    focus.lookDistance = -1;
                }
                    
                // See if the focused object has changed
                bool changeFocus = focus.lookObject != focus.previousLookObject;

                // Only do this if the focus changed
                // If the object has a focus target component, set focus to true
                if (changeFocus && focus.lookObject != Entity.Null && focusTargetGetter.HasComponent(focus.lookObject))
                {
                    // Focus the currently looked at object
                    parallelWriter.SetComponent<FocusTarget>(entityInQueryIndex, focus.lookObject, new FocusTarget { isFocused = true });
                }
                if (changeFocus && focus.previousLookObject != Entity.Null && focusTargetGetter.HasComponent(focus.previousLookObject))
                {
                    // De-focus the previously looked at object
                    parallelWriter.SetComponent<FocusTarget>(entityInQueryIndex, focus.previousLookObject, new FocusTarget { isFocused = false });
                }
            }).ScheduleParallel();
            this.Dependency.Complete();

            // Set the focus state of objects
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}