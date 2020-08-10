
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// Collider processor to handle filtering out an object to stop
    /// hitting itself.
    /// </summary>
    public struct SelfFilteringClosestHitCollector<T> : ICollector<T> where T : struct,IQueryResult
    {
        /// <summary>
        /// Can this exit early on the current hit object
        /// </summary>
        public bool EarlyOutOnFirstHit => false;

        /// <summary>
        /// Maximum fraction away that the hit object was encountered
        /// along the path of the raycast
        /// </summary>
        /// <value></value>
        public float MaxFraction { get; private set; }

        /// <summary>
        /// Number of objects hit
        /// </summary>
        public int NumHits { get; private set; }

        /// <summary>
        /// Previously hit object
        /// </summary>
        private T oldHit;

        /// <summary>
        /// Most recent (closest hit object)
        /// </summary>
        public T ClosestHit {get; private set; }

        /// <summary>
        /// Pointer to the self collider
        /// </summary>
        private BlobAssetReference<Unity.Physics.Collider> selfCollider;

        private CollisionWorld collisionWorld;

        /// <summary>
        /// Creates a self filtering object collision detection
        /// </summary>
        /// <param name="collider">Collider to ignore</param>
        /// <param name="maxFraction">Maximum fraction that an object can be encountered
        /// as a portion of the current raycast draw</param>
        /// <param name="collisionWorld">World of all colliable objects</param>
        public SelfFilteringClosestHitCollector(PhysicsCollider collider, float maxFraction, CollisionWorld collisionWorld)
        {
            this.MaxFraction = maxFraction;
            this.oldHit = default(T);
            this.ClosestHit = default(T);
            this.NumHits = 0;
            this.selfCollider = collider.Value;
            this.collisionWorld = collisionWorld;
        }

        #region ICollector

        /// <inheritdoc/>
        public bool AddHit(T hit)
        {
            Assert.IsTrue(hit.Fraction <= MaxFraction);
            if (collisionWorld.Bodies[hit.RigidBodyIndex].Collider == this.selfCollider)
            {
                return false;
            }
            MaxFraction = hit.Fraction;
            this.oldHit = ClosestHit;
            this.ClosestHit = hit;
            this.NumHits = 1;
            return true;
        }

        #endregion
    }

    /// <summary>
    /// Player movement system that moves player controlled objects from
    /// a given player's input. Moves a physics body attached to a
    /// character based on input with some respect for physics.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateAfter(typeof(PlayerRotationSystem))]
    public class PlayerPhysicsBasedMovement : ComponentSystem
    {
        /// <summary>
        /// Maximum angle between ground and character.
        /// </summary>
        public static readonly float MaxAngleFallDegrees = 90;

        /// <summary>
        /// Max angle to use when calculating the shoving angle of a character.
        /// </summary>
        public static readonly float MaxAngleShoveRadians = math.PI / 2;   

        /// <summary>
        /// Gets the final position of a character attempting to move from a starting
        /// location with a given movement. The goal of this is to move the character
        /// but have the character 'bounce' off of objects at angles that are 
        /// are along the plane perpendicular to the normal of the surface hit.
        /// This will cancel motion through the object and instead deflect it 
        /// into another direction giving the effect of sliding along objects
        /// while the character's momentum is absorbed into the wall.
        /// </summary>
        /// <param name="start">Starting location</param>
        /// <param name="movement">Intended direction of movement</param>
        /// <param name="collider">Collider controlling the character</param>
        /// <param name="rotation">Current character rotation</param>
        /// <param name="anglePower">Power to raise decay of movement due to
        /// changes in angle between intended movement and angle of surface.
        /// Will be angleFactor= 1 / (1 + normAngle) where normAngle is a normalized value
        /// between 0-1 where 1 is max angle (90 deg) and 0 is min angle (0 degrees).
        /// AnglePower is the power to which the angle factor is raised
        /// for a sharper decline. Value of zero negates this property.
        /// <param name="maxBounces">Maximum number of bounces when moving. 
        /// After this has been exceeded the bouncing will stop. By default 
        /// this is one assuming that each move is fairly small this should approximate
        /// normal movement.</param>
        /// <param name="pushPower">Multiplier for power when pushing on an object.
        /// Larger values mean more pushing.</param>
        /// <param name="pushDecay">How much does the current push decay the remaining
        /// movement by. Values between [0, 1]. A zero would mean all energy is lost
        /// when pushing, 1 means no momentum is lost by pushing. A value of 0.5
        /// would mean half of the energy is lost</param>
        /// <returns>The final location of the character.</returns>
        private unsafe float3 ProjectValidMovement(float3 start, float3 movement,
            PhysicsCollider collider, quaternion rotation, float anglePower=2, int maxBounces=1,
            float pushPower = 25, float pushDecay = 0)
        {
            ComponentDataFromEntity<PhysicsVelocity> pvTypeFromEntity = GetComponentDataFromEntity<PhysicsVelocity>(true);

            BuildPhysicsWorld physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
            CollisionWorld collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

            float epsilon = 0.001f;

            float3 from = start; // Starting location of movement
            float3 remaining = movement; // Remaining momentum
            int bounces = 0; // current number of bounces
            // Continue computing while there is momentum and bounces remaining
            while (math.length(remaining) > epsilon && bounces <= maxBounces)
            {
                // Get the target location given the momentum
                float3 target = from + remaining;

                // Do a cast of the collider to see if an object is hit during this
                // movement action
                var input = new ColliderCastInput()
                {
                    Start = from,
                    End = target,
                    Collider = collider.ColliderPtr,
                    Orientation = rotation
                };
            
                SelfFilteringClosestHitCollector<ColliderCastHit> hitCollector = new SelfFilteringClosestHitCollector<ColliderCastHit>(collider, 1.0f, collisionWorld);

                bool collisionOcurred = collisionWorld.CastCollider(input, ref hitCollector);

                if(!collisionOcurred && hitCollector.NumHits == 0)
                {
                    // If there is no hit, target can be returned as final position
                    return target;
                }

                Unity.Physics.ColliderCastHit hit = hitCollector.ClosestHit;

                // Set the fraction of remaining movement (minus some small value)
                from = from + remaining * hit.Fraction;
                // Push slightly along normal to stop from getting caught in walls
                from = from + hit.SurfaceNormal * epsilon;

                // Apply some force to the object hit if it is moveable
                // Entity e = physicsWorld.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                // Apply force on entity hit
                Entity hitEntity = collisionWorld.Bodies[hit.RigidBodyIndex].Entity;
                if (EntityManager.HasComponent<PhysicsVelocity>(hitEntity) && 
                    EntityManager.HasComponent<PhysicsMass>(hitEntity) &&
                    EntityManager.HasComponent<Rotation>(hitEntity) &&
                    EntityManager.HasComponent<Translation>(hitEntity)) {
                    PhysicsVelocity pv = EntityManager.GetComponentData<PhysicsVelocity>(hitEntity);
                    PhysicsMass pm = EntityManager.GetComponentData<PhysicsMass>(hitEntity);
                    Rotation rot = EntityManager.GetComponentData<Rotation>(hitEntity);
                    Translation trans = EntityManager.GetComponentData<Translation>(hitEntity);
                    pv.ApplyImpulse(pm, trans, rot, movement * pushPower, hit.Position);
                    EntityManager.SetComponentData(hitEntity, pv);
                    // If pushing something, reduce remaining force significantly
                    remaining *= pushDecay;
                }

                // Get angle between surface normal and remaining movement
                float angleBetween = math.length(math.dot(hit.SurfaceNormal, remaining)) / math.length(remaining);
                // Normalize angle between to be between 0 and 1
                angleBetween = math.min(PlayerPhysicsBasedMovement.MaxAngleShoveRadians, math.abs(angleBetween));
                float normalizedAngle = angleBetween / PlayerPhysicsBasedMovement.MaxAngleShoveRadians;
                // Create angle factor using 1 / (1 + normalizedAngle)
                float angleFactor = 1.0f / (1.0f + normalizedAngle);
                // If the character hit something
                // Reduce the momentum by the remaining movement that ocurred
                remaining = remaining * (1 - hit.Fraction) * math.pow(angleFactor, anglePower);
                // Rotate the remaining remaining movement to be projected along the plane 
                // of the surface hit (emulate pushing against the object)
                // A is our vector and B is normal of plane
                // A || B = B × (A×B / |B|) / |B|
                // From http://www.euclideanspace.com/maths/geometry/elements/plane/lineOnPlane/index.htm
                float3 planeNormal = hit.SurfaceNormal;
                float momentumLeft = math.length(remaining);
                remaining = math.cross(planeNormal, math.cross(remaining, planeNormal) / math.length(planeNormal)) / math.length(planeNormal);
                remaining = math.normalizesafe(remaining) * momentumLeft;
                // Track number of times the character has bounced
                bounces++;
            }
            return from;
        }
        
        /// <summary>
        /// Gets the angle between a character and the ground their standing on
        /// </summary>
        /// <param name="translation">starting position</param>
        /// <param name="groundCheckDistance">Max distance to reach for the ground</param>
        /// <param name="collider">Collider controlling the character</param>
        /// <param name="rotation">Current character rotation</param>
        /// <returns>A two component float, first component is the
        /// distance to the ground. Second component is angle betwen ground and the character.
        /// If the values of the components are -1, then that means that no object
        /// was found within groundCheckDistance</returns>
        public unsafe float2 AngleBetweenGround(float3 translation, float groundCheckDistance, PhysicsCollider collider, quaternion rotation)
        {
            BuildPhysicsWorld physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
            CollisionWorld collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

            SelfFilteringClosestHitCollector<ColliderCastHit> hitCollector = new SelfFilteringClosestHitCollector<ColliderCastHit>(collider, 1.0f, collisionWorld);

            var from = translation;
            var to = translation - new float3(0f, groundCheckDistance, 0f);
            var input = new ColliderCastInput()
            {
                End = to,
                Start = from,
                Collider = collider.ColliderPtr,
                Orientation = rotation
            };

            bool collisionOcurred = collisionWorld.CastCollider(input, ref hitCollector);
            Unity.Physics.ColliderCastHit hit = hitCollector.ClosestHit;

            if(collisionOcurred) {
                float angleBetween = math.abs(math.acos(math.dot(math.normalizesafe(hit.SurfaceNormal), new float3(0, 1, 0))));
                angleBetween = math.degrees(angleBetween);
                angleBetween = math.max(0, math.min(angleBetween, PlayerPhysicsBasedMovement.MaxAngleFallDegrees));
                return new float2(hit.Fraction * groundCheckDistance, angleBetween);
            }
            
            return new float2(-1, -1);
        }

        protected override void OnUpdate()
        {
            var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
            var tick = group.PredictingTick;
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((DynamicBuffer<PlayerInput> inputBuffer,
                ref PredictedGhostComponent prediction,
                ref PlayerMovement settings, ref PhysicsCollider collider,
                ref Translation trans, ref Rotation rot) =>
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

                quaternion horizPlaneView = rot.Value;

                // Make movement vector based on player input
                float3 movementVelocity = math.mul(horizPlaneView, direction) * speedMultiplier;

                // Check if is grounded for jumping
                float2 groundedCheck = this.AngleBetweenGround(trans.Value, settings.groundCheckDistance, collider, rot.Value);
                float dist = groundedCheck.x;
                float angle = groundedCheck.y;
                bool grounded = dist >= 0 && angle < settings.maxWalkAngle;
                // Jump if grounded and player hits jump button
                if (grounded && input.IsJumping)
                {
                    settings.velocity = new float3(0, settings.jumpForce, 0);
                }
                // Let the player drift to some value very close to the ground
                else if (!grounded) {
                    settings.velocity += settings.gravityForce * deltaTime;
                }
                // Have hit the ground, stop moving
                else {
                    settings.velocity = float3.zero;
                }
                float3 finalPos = trans.Value;
                // Player controlled movement
                finalPos = ProjectValidMovement(trans.Value, movementVelocity * deltaTime, collider, rot.Value, anglePower: 2f, maxBounces: 3);
                // Gravity controlled movement (Don't let the player bounce from this)
                finalPos = ProjectValidMovement(finalPos, settings.velocity * deltaTime, collider, rot.Value, anglePower: 1.1f, maxBounces: 2);
                trans.Value = finalPos;
            });
        }

    }

}
