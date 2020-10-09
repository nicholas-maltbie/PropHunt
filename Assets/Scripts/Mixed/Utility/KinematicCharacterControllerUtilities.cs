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
                    UnityEngine.Debug.DrawLine(from, target, UnityEngine.Color.red);
                    // If there is no hit, target can be returned as final position
                    return target;
                }

                Unity.Physics.ColliderCastHit hit = hitCollector.ClosestHit;

                // Set the fraction of remaining movement (minus some small value)
                from = from + remaining * hit.Fraction;
                // Push slightly along normal to stop from getting caught in walls
                from = from + hit.SurfaceNormal * epsilon;

                // Apply some force to the object hit if it is moveable, Apply force on entity hit
                bool isKinematic = physicsMassGetter.HasComponent(hit.Entity) && IsKinematic(physicsMassGetter[hit.Entity]);
                if (hit.RigidBodyIndex < collisionWorld.NumDynamicBodies && !isKinematic)
                {
                    commandBuffer.AddBuffer<PushForce>(jobIndex, hit.Entity);
                    commandBuffer.AppendToBuffer(jobIndex, hit.Entity, new PushForce() { force = movement * pushPower, point = hit.Position });
                    // If pushing something, reduce remaining force significantly
                    remaining *= pushDecay;
                }

                // Get angle between surface normal and remaining movement
                float angleBetween = math.length(math.dot(hit.SurfaceNormal, remaining)) / math.length(remaining);
                // Normalize angle between to be between 0 and 1
                angleBetween = math.min(KCCUtils.MaxAngleShoveRadians, math.abs(angleBetween));
                float normalizedAngle = angleBetween / KCCUtils.MaxAngleShoveRadians;
                // Create angle factor using 1 / (1 + normalizedAngle)
                float angleFactor = 1.0f / (1.0f + normalizedAngle);
                // If the character hit something
                // Reduce the momentum by the remaining movement that ocurred
                remaining *= (1 - hit.Fraction) * math.pow(angleFactor, anglePower);
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
                
                float distanceToFeet = hit.Position.y - from.y;
                UnityEngine.Debug.DrawLine(from, hit.Position, UnityEngine.Color.cyan);
                UnityEngine.Debug.DrawLine(hit.Position, hit.Position - new float3(0, distanceToFeet, 0), UnityEngine.Color.green);
                // Snap character vertically up if they hit something
                //  close enough to their feet
                if (distanceToFeet < verticalSnapUp)
                {
                    // Increment vertical (y) value of new position by
                    //  the distance to the feet of the character
                    from = from + new float3(0, distanceToFeet, 0);
                }
            }
            return from;
        }
    }
}