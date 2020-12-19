using PropHunt.Constants;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Utilities;
using PropHunt.Mixed.Utility;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System group for all Kinematic Character Controller Actions
    /// </summary>
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateAfter(typeof(KinematicCharacterControllerInput))]
    public class KCCUpdateGroup : ComponentSystemGroup { }

    /// <summary>
    /// Updates the grounded data on a kinematic character controller
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(KCCUpdateGroup), OrderFirst = true)]
    public class KCCGroundedSystem : PredictedStateSystem
    {
        /// <summary>
        /// Maximum degrees between ground and player 
        /// </summary>
        public static readonly float MaxAngleFallDegrees = 90;

        protected unsafe override void OnUpdate()
        {
            CollisionWorld collisionWorld = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld.CollisionWorld;
            float deltaTime = unityService.GetDeltaTime(base.Time);
            var tick = this.predictionManager.GetPredictingTick(base.World);

            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var ecbParallelWriter = ecb.AsParallelWriter();

            Entities.WithReadOnly(collisionWorld).ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref KCCGrounded grounded,
                in KCCGravity gravity,
                in PhysicsCollider collider,
                in Rotation rotation,
                in Translation translation,
                in PredictedGhostComponent predicted) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, predicted))
                {
                    return;
                }

                SelfFilteringClosestHitCollector<ColliderCastHit> hitCollector =
                    new SelfFilteringClosestHitCollector<ColliderCastHit>(entity.Index, 1.0f, collisionWorld);

                float3 from = translation.Value;
                float3 to = from + gravity.Down * grounded.groundCheckDistance;

                var input = new ColliderCastInput()
                {
                    End = to,
                    Start = from,
                    Collider = collider.ColliderPtr,
                    Orientation = rotation.Value
                };

                bool collisionOcurred = collisionWorld.CastCollider(input, ref hitCollector);
                Unity.Physics.ColliderCastHit hit = hitCollector.ClosestHit;

                grounded.previousAngle = grounded.angle;
                grounded.previousOnGround = grounded.onGround;
                grounded.previousDistanceToGround = grounded.distanceToGround;
                grounded.previousHit = grounded.hitEntity;

                if (collisionOcurred)
                {
                    float angleBetween = math.abs(math.acos(math.dot(math.normalizesafe(hit.SurfaceNormal), gravity.Up)));
                    float angleDegrees = math.degrees(angleBetween);
                    grounded.angle = math.max(0, math.min(angleDegrees, KCCGroundedSystem.MaxAngleFallDegrees));
                    grounded.onGround = true;
                    grounded.distanceToGround = hit.Fraction * grounded.groundCheckDistance;
                    grounded.groundedRBIndex = hit.RigidBodyIndex;
                    grounded.groundedPoint = hit.Position;
                    grounded.hitEntity = hit.Entity;
                    grounded.surfaceNormal = hit.SurfaceNormal;
                }
                else
                {
                    grounded.onGround = false;
                    grounded.distanceToGround = -1;
                    grounded.angle = -1;
                    grounded.groundedRBIndex = -1;
                    grounded.groundedPoint = float3.zero;
                    grounded.hitEntity = Entity.Null;
                    grounded.surfaceNormal = float3.zero;
                }

                // Falling is generated from other values, can be falling
                //  if hitting a steep slope on the ground.
                if (grounded.Falling)
                {
                    grounded.elapsedFallTime += deltaTime;
                }
                else
                {
                    grounded.elapsedFallTime = 0;
                }

                // If the floor has a MovementTracking component, set the parent of
                //  the player entity to be the floor so the player can move along
                //  with the floor

            }).ScheduleParallel();

            this.Dependency.Complete();

            // Playback events
            ecb.Playback(base.EntityManager);
        }
    }

    /// <summary>
    /// This snaps characters down onto the ground if they are floating within a small distance
    /// of the ground.
    /// </summary>
    [UpdateInGroup(typeof(KCCUpdateGroup))]
    [UpdateAfter(typeof(KCCMovementSystem))]
    [BurstCompile]
    public class KCCSnapDown : PredictedStateSystem
    {
        protected unsafe override void OnUpdate()
        {
            var collisionWorld = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld.CollisionWorld;

            float deltaTime = unityService.GetDeltaTime(base.Time);
            var kccGravityGetter = this.GetComponentDataFromEntity<KCCGravity>(true);
            var tick = this.predictionManager.GetPredictingTick(base.World);

            Entities.WithReadOnly(collisionWorld)
                .WithReadOnly(kccGravityGetter)
                .ForEach((
                Entity entity,
                ref Translation translation,
                in Rotation rotation,
                in KCCGrounded grounded,
                in KCCVelocity velocity,
                in KCCMovementSettings settings,
                in PhysicsCollider collider,
                in PredictedGhostComponent predicted) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, predicted))
                {
                    return;
                }
                // Don't snap down if they are moving up (either in world or player velocity)
                if (KCCUtils.HasMovementAlongAxis(velocity, kccGravityGetter[entity].Up) || !grounded.StandingOnGround)
                {
                    return;
                }

                SelfFilteringClosestHitCollector<ColliderCastHit> hitCollector =
                    new SelfFilteringClosestHitCollector<ColliderCastHit>(entity.Index, 1.0f, collisionWorld);

                float3 from = translation.Value;
                float3 to = from + kccGravityGetter[entity].Down * settings.snapDownOffset;

                var input = new ColliderCastInput()
                {
                    End = to,
                    Start = from,
                    Collider = collider.ColliderPtr,
                    Orientation = rotation.Value
                };

                bool collisionOcurred = collisionWorld.CastCollider(input, ref hitCollector);
                Unity.Physics.ColliderCastHit hit = hitCollector.ClosestHit;
                float distanceToGround = hit.Fraction * settings.snapDownOffset;

                if (collisionOcurred && distanceToGround > KCCConstants.Epsilon)
                {
                    float cappedSpeed = math.min(distanceToGround, settings.snapDownSpeed * deltaTime);
                    // Shift character down to that location (plus some wiggle epsilon room)
                    translation.Value = translation.Value + kccGravityGetter[entity].Down * (cappedSpeed - KCCConstants.Epsilon * 2);
                }
            }).ScheduleParallel();

            this.Dependency.Complete();
        }
    }

    /// <summary>
    /// Applies character movement to a kinematic character controller
    /// </summary>
    [UpdateInGroup(typeof(KCCUpdateGroup))]
    [BurstCompile]
    public class KCCMovementSystem : PredictedStateSystem
    {
        /// <summary>
        /// Command buffer system for pushing objects
        /// </summary>
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            this.commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = this.commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var collisionWorld = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld.CollisionWorld;
            var physicsMassGetter = this.GetComponentDataFromEntity<PhysicsMass>(true);
            var kccGroundedGetter = this.GetComponentDataFromEntity<KCCGrounded>(true);
            var kccGravityGetter = this.GetComponentDataFromEntity<KCCGravity>(true);
            float deltaTime = this.unityService.GetDeltaTime(base.Time);
            var tick = this.predictionManager.GetPredictingTick(base.World);

            Entities.WithReadOnly(collisionWorld)
                .WithReadOnly(kccGravityGetter)
                .WithReadOnly(kccGroundedGetter)
                .WithReadOnly(physicsMassGetter)
                .ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref Translation translation,
                in KCCVelocity velocity,
                in PhysicsCollider physicsCollider,
                in Rotation rotation,
                in KCCMovementSettings movementSettings,
                in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                {
                    return;
                }

                KCCGrounded grounded = kccGroundedGetter[entity];
                KCCGravity gravity = kccGravityGetter[entity];
                float verticalSnapMove = !grounded.Falling || !grounded.PreviousFalling ? movementSettings.stepOffset : 0;

                float3 projectedMovement = velocity.playerVelocity * deltaTime;
                // If the player is standing on the ground, project their movement onto the ground plane
                // This allows them to walk up gradual slopes without facing a hit in movement speed
                if (!grounded.Falling)
                {
                    // Move = Normalize(Proj(Move, GroundSurface)) * Move
                    projectedMovement = math.normalize(KCCUtils.ProjectVectorOntoPlane(projectedMovement, grounded.surfaceNormal)) * math.length(projectedMovement);
                }

                // Adjust character translation due to player movement
                translation.Value = KCCUtils.ProjectValidMovement(
                    commandBuffer,
                    entityInQueryIndex,
                    collisionWorld,
                    translation.Value,
                    projectedMovement,
                    physicsCollider,
                    entity.Index,
                    rotation.Value,
                    physicsMassGetter,
                    verticalSnapUp: verticalSnapMove,
                    gravityDirection: gravity.Down,
                    maxBounces: movementSettings.moveMaxBounces,
                    pushPower: movementSettings.movePushPower,
                    pushDecay: movementSettings.movePushDecay,
                    anglePower: movementSettings.moveAnglePower
                );
                // Adjust character translation due to gravity/world forces
                translation.Value = KCCUtils.ProjectValidMovement(
                    commandBuffer,
                    entityInQueryIndex,
                    collisionWorld,
                    translation.Value,
                    velocity.worldVelocity * deltaTime,
                    physicsCollider,
                    entity.Index,
                    rotation.Value,
                    physicsMassGetter,
                    verticalSnapUp: 0,
                    gravityDirection: gravity.Down,
                    maxBounces: movementSettings.fallMaxBounces,
                    pushPower: movementSettings.fallPushPower,
                    pushDecay: movementSettings.fallPushDecay,
                    anglePower: movementSettings.fallAnglePower
                );
            }).ScheduleParallel();

            this.Dependency.Complete();
            this.commandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }

    /// <summary>
    /// Jumping, happens after gravity and before moving character.
    /// Will effect the world velocity of the character (since jumping
    /// will decay due to gravity)
    /// </summary>
    [UpdateInGroup(typeof(KCCUpdateGroup))]
    [UpdateBefore(typeof(KCCMovementSystem))]
    [UpdateAfter(typeof(KCCGravitySystem))]
    [BurstCompile]
    public class KCCJumpSystem : PredictedStateSystem
    {
        protected override void OnUpdate()
        {
            float deltaTime = unityService.GetDeltaTime(base.Time);
            var tick = this.predictionManager.GetPredictingTick(base.World);
            Entities.ForEach((
                ref KCCVelocity velocity,
                ref KCCJumping jumping,
                in KCCGrounded grounded,
                in KCCGravity gravity,
                in PredictedGhostComponent predicted) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, predicted))
                {
                    return;
                }
                // If the KCC is attempting to jump and is grounded, jump
                if (jumping.attemptingJump && KCCUtils.CanJump(jumping, grounded))
                {
                    velocity.worldVelocity += gravity.Up * jumping.jumpForce;
                    jumping.timeElapsedSinceJump = 0.0f;
                }
                // Track jumping cooldown if not jumping
                else if (jumping.timeElapsedSinceJump < jumping.jumpCooldown)
                {
                    jumping.timeElapsedSinceJump += deltaTime;
                }
                // Otherwise do nothing
            }).ScheduleParallel();
        }
    }

    /// <summary>
    /// Pushes kinematic character controllers out of objects they are stuck in
    /// </summary>
    [UpdateInGroup(typeof(KCCUpdateGroup))]
    [UpdateBefore(typeof(KCCMovementSystem))]
    [BurstCompile]
    public class KCCPushOverlappingSystem : PredictedStateSystem
    {
        protected unsafe override void OnUpdate()
        {
            var collisionWorld = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld.CollisionWorld;
            float deltaTime = unityService.GetDeltaTime(base.Time);
            var tick = this.predictionManager.GetPredictingTick(base.World);

            Entities.WithReadOnly(collisionWorld).ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref Translation translation,
                in PhysicsCollider collider,
                in Rotation rotation,
                in KCCGravity gravity,
                in KCCMovementSettings movementSettings,
                in PredictedGhostComponent predicted) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, predicted))
                {
                    return;
                }

                // Project the character collider down and see what they collide with
                //  Only filter for intersections
                SelfFilteringClosestHitCollector<ColliderCastHit> overlapCollector =
                    new SelfFilteringClosestHitCollector<ColliderCastHit>(entity.Index, 1.0f, collisionWorld);

                float3 from = translation.Value;
                float3 to = from + gravity.Down * KCCConstants.Epsilon;

                var overlapInput = new ColliderCastInput()
                {
                    End = to,
                    Start = from,
                    Collider = collider.ColliderPtr,
                    Orientation = rotation.Value
                };

                bool overlapOcurred = collisionWorld.CastCollider(overlapInput, ref overlapCollector);
                Unity.Physics.ColliderCastHit overlapHit = overlapCollector.ClosestHit;

                // Skip if no overlap ocurred
                if (!overlapOcurred || overlapHit.Fraction > 0)
                {
                    return;
                }

                // Draw a ray from the center of the character collider to 
                //  the point of collision
                // If the ray intersects the other object before it reaches the edge of our own collider,
                //  then we know that we are overlapping with the object
                float3 hitPoint = overlapHit.Position - overlapHit.SurfaceNormal * KCCConstants.Epsilon;
                float3 sourcePoint = hitPoint + overlapHit.SurfaceNormal * (movementSettings.maxPush * deltaTime);
                int hitObject = overlapHit.Entity.Index;
                int selfIndex = entity.Index;

                // Hit collector to only collide with our object and the object we overlap with
                var hitCollector = new FilteringClosestHitCollector<Unity.Physics.RaycastHit>(
                    selfIndex, hitObject, 1.0f, collisionWorld);

                // Draw a ray from the center of the character to the hit object
                var input = new RaycastInput()
                {
                    Filter = collider.Value.Value.Filter,
                    Start = sourcePoint,
                    End = hitPoint,
                };

                // Do the raycast computation
                bool collisionOcurred = collisionWorld.CastRay(input, ref hitCollector);
                // UnityEngine.Debug.DrawLine(sourcePoint, hitPoint, UnityEngine.Color.green);

                // Push our character collider out of the object we are overlapping with
                if (collisionOcurred)
                {
                    // Hit something
                    var raycastHit = hitCollector.ClosestHit;
                    // Get the distance of overlap (1 - hit fraction) * distance
                    float3 direction = hitPoint - sourcePoint;
                    float distance = math.length(direction);
                    float overlapDistance = (1 - raycastHit.Fraction) * distance;
                    // UnityEngine.Debug.DrawLine(sourcePoint, raycastHit.Position, UnityEngine.Color.red);
                    // UnityEngine.Debug.DrawLine(raycastHit.Position, hitPoint, UnityEngine.Color.cyan);
                    // Get movement in direction touching object
                    float3 push = overlapHit.SurfaceNormal * (overlapDistance + KCCConstants.Epsilon * 2);
                    // Push character collider by this much
                    translation.Value = translation.Value + push;
                }
            }).ScheduleParallel();
        }
    }

    /// <summary>
    /// Applies gravity to kinematic character controller. Does
    /// this after checking if character is grounded
    /// </summary>
    [UpdateInGroup(typeof(KCCUpdateGroup))]
    [UpdateBefore(typeof(KCCMovementSystem))]
    [BurstCompile]
    public class KCCGravitySystem : PredictedStateSystem
    {
        protected override void OnUpdate()
        {
            float deltaTime = unityService.GetDeltaTime(base.Time);
            var tick = this.predictionManager.GetPredictingTick(base.World);

            Entities.ForEach((
                ref KCCVelocity velocity,
                in KCCGrounded grounded,
                in KCCGravity gravity,
                in PredictedGhostComponent predicted) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, predicted))
                {
                    return;
                }

                bool movingUp = KCCUtils.HasMovementAlongAxis(velocity, gravity.Up);

                // If the player is not grounded, push down by
                // gravity's acceleration
                if (grounded.Falling)
                {
                    // have world velocity decrease due to air resistance (future feature)

                    // fall due to gravity
                    velocity.worldVelocity += gravity.gravityAcceleration * deltaTime;
                }
                // else: Have hit the ground, don't accelerate due to gravity
                else if (!movingUp)
                {
                    velocity.worldVelocity = float3.zero;
                }
            }).ScheduleParallel();
        }
    }
}