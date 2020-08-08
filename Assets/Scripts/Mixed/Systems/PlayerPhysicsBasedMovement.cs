
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace PropHunt.Mixed.Systems
{

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
        public static readonly float MaxAngleFall = 90;   

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
        /// <param name="force">Force being applied</param>
        /// <param name="collider">Collider controlling the character</param>
        /// <param name="rotation">Current character rotation</param>
        /// <param name="maxBounces">Maximum number of bounces when moving. 
        /// After this has been exceeded the bouncing will stop. By default 
        /// this is one assuming that each move is fairly small this should approximate
        /// normal movement.</param>
        /// <returns>The final location of the character.</returns>
        private unsafe float3 ProjectValidMovement(float3 start, float3 movement, float3 force, PhysicsCollider collider, quaternion rotation, int maxBounces=1)
        {
            ComponentDataFromEntity<PhysicsVelocity> pvTypeFromEntity = GetComponentDataFromEntity<PhysicsVelocity>(true);

            BuildPhysicsWorld physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
            CollisionWorld collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

            float epsilon = 0.001f;

            float3 from = start; // Starting location of movement
            float3 remaining = movement; // Remaining momentum
            int bounces = 0; // current number of bounces
            // Continue computing while there is momentum and bounces remaining
            while (math.length(remaining) > epsilon && bounces <= maxBounces) {
                // Get the target location given the momentum
                float3 target = from + remaining;

                // Do a cast of the collider to see if an object is hit during this
                // movement action
                Unity.Physics.ColliderCastHit hit = new Unity.Physics.ColliderCastHit();
                var input = new ColliderCastInput()
                {
                    Start = from,
                    End = target,
                    Collider = collider.ColliderPtr,
                    Orientation = rotation
                };
                if(!collisionWorld.CastCollider(input, out hit)) {
                    // If there is no hit, target can be returned as final position
                    return target;
                }
                // Apply some force to the object hit
                // Entity e = physicsWorld.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                // Apply force on entity hit
                Unity.Physics.Extensions.PhysicsWorldExtensions.ApplyImpulse(
                    physicsWorld.PhysicsWorld, hit.RigidBodyIndex, force, hit.Position);

                // Set the fraction of remaining movement (minus some small value)
                from = from + remaining * hit.Fraction;
                // Push slightly along normal to stop from getting caught in walls
                from = from + hit.SurfaceNormal * epsilon;
                // If the character hit something
                // Reduce the momentum by the remaining movement that ocurred
                remaining = remaining * (1 - hit.Fraction);
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

            var from = translation;
            var to = translation - new float3(0f, groundCheckDistance, 0f);
            var input = new ColliderCastInput()
            {
                End = to,
                Start = from,
                Collider = collider.ColliderPtr,
                Orientation = rotation
            };

            Unity.Physics.ColliderCastHit hit = new Unity.Physics.ColliderCastHit();

            if(collisionWorld.CastCollider(input, out hit)) {
                float angleBetween = math.abs(math.acos(math.dot(math.normalizesafe(hit.SurfaceNormal), new float3(0, 1, 0))));
                angleBetween = math.degrees(angleBetween);
                angleBetween = math.max(0, math.min(angleBetween, PlayerPhysicsBasedMovement.MaxAngleFall));
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

                PlayerInput input;
                inputBuffer.GetDataAtTick(tick, out input);

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
                    // Start falling by accelerating in the force of gravity
                    // Accelerate more based on the slope
                    float angleFactor = 1;
                    if (angle > settings.maxWalkAngle)
                    {
                        angleFactor = angle / PlayerPhysicsBasedMovement.MaxAngleFall;
                    }
                    settings.velocity += settings.gravityForce * deltaTime * math.pow(angleFactor, 2);
                }
                // Have hit the ground, stop moving
                else {
                    settings.velocity = float3.zero;
                }

                // Player controlled movement
                float3 finalPos = ProjectValidMovement(trans.Value, movementVelocity * deltaTime, movementVelocity, collider, rot.Value, maxBounces: 3);
                // Gravity controlled movement (Don't let the player bounce from this)
                finalPos = ProjectValidMovement(finalPos, settings.velocity * deltaTime, settings.velocity, collider, rot.Value, maxBounces: 1);
                trans.Value = finalPos;
            });
        }

    }

}
