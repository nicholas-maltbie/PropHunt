using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using PropHunt.Mixed.Components;

namespace PropHunt.Mixed.Utilities
{
    /// <summary>
    /// Class for kinematic character controller utilities
    /// </summary>
    public static class KCCUtils
    {
        /// <summary>
        /// Check if a character can jump from their current grounded and jumping states
        /// </summary>
        /// <param name="jumping">Current jumping state</param>
        /// <param name="grounded">Current grounded state</param>
        public static bool CanJump(KCCJumping jumping, KCCGrounded grounded)
        {
            return !grounded.Falling && grounded.elapsedFallTime <= jumping.jumpGraceTime && jumping.timeElapsedSinceJump >= jumping.jumpCooldown;
        }

        /// <summary>
        /// Checks if a character is moving along a given line (direction). Will
        /// return true if the player has some positive motion along the given line in any axis
        /// in either the world or player velocity
        /// </summary>
        /// <param name="velocity">Player velocity (world and input)</param>
        /// <param name="axis">Direction to check if the player is moving along</param>
        /// <returns>True if the player has some motion along that line</returns>
        public static bool HasMovementAlongAxis(KCCVelocity velocity, float3 axis)
        {
            return math.dot(velocity.worldVelocity, axis) > 0 || math.dot(velocity.playerVelocity, axis) > 0;
        }

        /// <summary>
        /// Maximum angle between an object and a character 
        /// </summary>
        public static readonly float MaxAngleShoveRadians = math.radians(90.0f);

        /// <summary>
        /// Check if a physics mass is kinematic
        /// </summary>
        /// <param name="mass">Physics mass to verify</param>
        /// <returns>True if kinematic, false otherwise</returns>
        public static bool IsKinematic(PhysicsMass mass)
        {
            float3 intertia = mass.InverseInertia;
            return intertia.x == 0 && intertia.y == 0 && intertia.z == 0;
        }

        /// <summary>
        /// Small distance for acccounting for non deterministic simulation and float errors
        /// </summary>
        public static readonly float Epsilon = 0.001f;

        /// <summary>
        /// Gets the final position of a character attempting to move from a starting
        /// location with a given movement. The goal of this is to move the character
        /// but have the character 'bounce' off of objects at angles that are 
        /// are along the plane perpendicular to the normal of the surface hit.
        /// This will cancel motion through the object and instead deflect it 
        /// into another direction giving the effect of sliding along objects
        /// while the character's momentum is absorbed into the wall.
        /// </summary>
        /// <param name="commandBuffer">Command buffer for adding push events to objects</param>
        /// <param name="jobIndex">Index of this job for command buffer use</param>
        /// <param name="collisionWorld">World for checking collisions and other colliable objects</param>
        /// <param name="start">Starting location</param>
        /// <param name="movement">Intended direction of movement</param>
        /// <param name="collider">Collider controlling the character</param>
        /// <param name="entityIndex">Index of this entity</param>
        /// <param name="rotation">Current character rotation</param>
        /// <param name="physicsMassAccessor">Accessor to physics mass components</param>
        /// <param name="verticalSnapUp">Amount of distance the player can 'snap' vertically
        /// up when walking up stairs</param>
        /// <param name="gravityDirection">Direction of gravity for snapping vertically</param>
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
        /// <param name="epsilon">Epsilon parameter for pushing away from objects to avoid colliding
        /// with static objects. Should be some small float value that can be
        /// tuned for how much to push away while not allowing being shoved into objects.</param>
        /// <returns>The final location of the character.</returns>
        public static unsafe float3 ProjectValidMovement(
            EntityCommandBuffer.ParallelWriter commandBuffer,
            int jobIndex,
            CollisionWorld collisionWorld,
            float3 start,
            float3 movement,
            PhysicsCollider collider,
            int entityIndex,
            quaternion rotation,
            ComponentDataFromEntity<PhysicsMass> physicsMassGetter,
            float verticalSnapUp,
            float3 gravityDirection,
            float anglePower = 2,
            int maxBounces = 1,
            float pushPower = 25,
            float pushDecay = 0,
            float epsilon = 0.001f)
        {
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

                SelfFilteringClosestHitCollector<ColliderCastHit> hitCollector =
                    new SelfFilteringClosestHitCollector<ColliderCastHit>(entityIndex, 1.0f, collisionWorld);

                bool collisionOcurred = collisionWorld.CastCollider(input, ref hitCollector);

                if (!collisionOcurred && hitCollector.NumHits == 0)
                {
                    // If there is no hit, target can be returned as final position
                    return target;
                }

                Unity.Physics.ColliderCastHit hit = hitCollector.ClosestHit;

                // Set the fraction of remaining movement (minus some small value)
                from = from + remaining * hit.Fraction;
                // Push slightly along normal to stop from getting caught in walls
                from = from + hit.SurfaceNormal * epsilon;
                // Decrease remaining movement by 
                remaining *= (1 - hit.Fraction);

                // Apply some force to the object hit if it is moveable, Apply force on entity hit
                bool isKinematic = physicsMassGetter.HasComponent(hit.Entity) && IsKinematic(physicsMassGetter[hit.Entity]);
                if (hit.RigidBodyIndex < collisionWorld.NumDynamicBodies && !isKinematic)
                {
                    commandBuffer.AddBuffer<PushForce>(jobIndex, hit.Entity);
                    commandBuffer.AppendToBuffer(jobIndex, hit.Entity, new PushForce() { force = movement * pushPower, point = hit.Position });
                    // If pushing something, reduce remaining force significantly
                    remaining *= pushDecay;
                }

                // Normal vector of the plane the character is standing on
                float3 planeNormal = hit.SurfaceNormal;

                // Snap character vertically up if they hit something
                //  close enough to their feet
                float distanceToFeet = hit.Position.y - from.y;
                if (distanceToFeet > 0 && distanceToFeet < verticalSnapUp)
                {
                    // Increment vertical (y) value of new position by
                    //  the distance to the feet of the character
                    from = from - distanceToFeet * math.normalizesafe(gravityDirection);
                    // Project rest of movement onto plane perpendicular to gravity
                    planeNormal = -gravityDirection;
                }
                // Only apply angular change if hitting something
                else
                {
                    // Get angle between surface normal and remaining movement
                    float angleBetween = math.length(math.dot(hit.SurfaceNormal, remaining)) / math.length(remaining);
                    // Normalize angle between to be between 0 and 1
                    // 0 means no angle, 1 means 90 degree angle
                    angleBetween = math.min(KCCUtils.MaxAngleShoveRadians, math.abs(angleBetween));
                    float normalizedAngle = angleBetween / KCCUtils.MaxAngleShoveRadians;
                    // Create angle factor using 1 / (1 + normalizedAngle)
                    float angleFactor = 1.0f / (1.0f + normalizedAngle);
                    // Reduce the momentum by the remaining movement that ocurred
                    remaining *= math.pow(angleFactor, anglePower);
                }
                // Rotate the remaining remaining movement to be projected along the plane 
                // of the surface hit (emulate pushing against the object)
                float momentumLeft = math.length(remaining);
                remaining = ProjectVectorOntoPlane(remaining, planeNormal);
                remaining = math.normalizesafe(remaining) * momentumLeft;

                // Track number of times the character has bounced
                bounces++;
            }
            return from;
        }

        /// <summary>
        /// Projects a vector onto a plane. Does NOT preserve the original length of the vector,
        /// is simplay a projection onto a plane.
        /// </summary>
        /// <param name="vector">Vector to project</param>
        /// <param name="planeNormal">plane to project vector onto</param>
        /// <returns>Projected vector onto plane</returns>
        public static float3 ProjectVectorOntoPlane(float3 vector, float3 planeNormal)
        {
            // A is our vector and B is normal of plane
            // A || B = B × (A×B / |B|) / |B|
            // From http://www.euclideanspace.com/maths/geometry/elements/plane/lineOnPlane/index.htm
            return math.cross(planeNormal, math.cross(vector, planeNormal) / math.length(planeNormal)) / math.length(planeNormal);
        }
    }
}