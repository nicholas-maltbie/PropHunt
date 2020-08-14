

using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Player movement settings for when a Kinematic Character Controller is
    /// being controlled.
    /// </summary>
    [GhostDefaultComponent(GhostDefaultComponentAttribute.Type.All)]
    public struct KCCMovementSettings : IComponentData
    {        
        /// <summary>
        /// Player movement speed in units per second.
        /// </summary>
        [GhostDefaultField(100, true)]
        public float moveSpeed;

        /// <summary>
        /// Multiplier for sprint speed
        /// </summary>
        [GhostDefaultField(100, true)]
        public float sprintMultiplier;

        /// <summary>
        /// Sprint speed is move speed by a multiplier
        /// </summary>
        public float SprintSpeed => moveSpeed * sprintMultiplier;

        /// <summary>
        /// Max number of bounces per frame when moving
        /// </summary>
        [GhostDefaultField]
        public int moveMaxBounces;

        /// <summary>
        /// Decrease in momentum factor due to angle change when walking
        /// </summary>
        [GhostDefaultField(100, true)]
        public float moveAnglePower;

        /// <summary>
        /// Power of pushing objects
        /// </summary>
        [GhostDefaultField(100, true)]
        public float movePushPower;

        /// <summary>
        /// Proportional decrease in momentum due to pushing an object when moving
        /// </summary>
        [GhostDefaultField(100, true)]
        public float movePushDecay;

        /// <summary>
        /// Max number of bounces per frame when falling
        /// </summary>
        [GhostDefaultField]
        public int fallMaxBounces;

        /// <summary>
        /// Weight of object when falling (pushing) onto objects
        /// </summary>
        [GhostDefaultField(100, true)]
        public float fallPushPower;

        /// <summary>
        /// Decrease in momentum factor due to angle change when falling
        /// </summary>
        [GhostDefaultField(100, true)]
        public float fallAnglePower;

        /// <summary>
        /// Proportional decrease in momentum due to pushing an object when falling
        /// </summary>
        [GhostDefaultField(100, true)]
        public float fallPushDecay;
    }

    /// <summary>
    /// Settings for if a player is currently jumping
    /// </summary>
    public struct KCCJumping : IComponentData
    {
        /// <summary>
        /// Force of the player jump
        /// </summary>
        [GhostDefaultField(100, true)]
        public float jumpForce;

        /// <summary>
        /// Is the KCC attempting to jump?
        /// </summary>
        [GhostDefaultField]
        public bool attemptingJump;
    }

    /// <summary>
    /// Settings and data for if a character is currently grounded
    /// </summary>
    [GhostDefaultComponent(GhostDefaultComponentAttribute.Type.All)]
    public struct KCCGrounded : IComponentData
    {
        /// <summary>
        /// Max angle that the character can walk at
        /// </summary>
        [GhostDefaultField(100, true)]
        public float maxWalkAngle;

        /// <summary>
        /// Is the character currently falling (not on the ground or on the ground and
        /// it's too steep)
        /// </summary>
        public bool Falling => !this.onGround || this.angle > this.maxWalkAngle;

        /// <summary>
        /// Distance to check if character is thouching ground
        /// </summary>
        [GhostDefaultField(100, true)]
        public float groundCheckDistance;

        /// <summary>
        /// Angle between the ground the the player's 'up' vector (-gravity fector).
        /// Will always be a positive value between 0 and 90 degrees if the ground is hit. Is 
        /// measured in degrees. Will be a value of -1 if no hit ocurred.
        /// Does not need to synced between clients.
        /// </summary>
        public float angle;

        /// <summary>
        /// Distance to the ground (will be less than or equal to ground check distance).
        /// Will be 0 if on the ground. This value will be -1 if the
        /// ground was not hit.
        /// Does not need to synced between clients.
        /// </summary>
        public float distanceToGround;

        /// <summary>
        /// Did the ray actually hit the ground within the rayCastLength?
        /// Does not need to synced between clients.
        /// </summary>
        public bool onGround;
    }

    /// <summary>
    /// Structure holding a Kinematic Character Controller's current velocity broken in to parts,
    /// velocity due to player input and velocity due to the world around them.
    /// </summary>
    [GhostDefaultComponent(
        GhostDefaultComponentAttribute.Type.Server |
        GhostDefaultComponentAttribute.Type.PredictedClient)]
    public struct KCCVelocity : IComponentData
    {
        /// <summary>
        /// Velocity due to player input
        /// </summary>
        [GhostDefaultField(100, true)]
        public float3 playerVelocity;

        /// <summary>
        /// Velocity due to world forces
        /// </summary>
        [GhostDefaultField(100, true)]
        public float3 worldVelocity;
    }

    /// <summary>
    /// Structure for controlling the direction and force of gravity to a kinematic
    /// character controller.
    /// </summary>
    [GhostDefaultComponent(GhostDefaultComponentAttribute.Type.All)]
    public struct KCCGravity : IComponentData
    {

        /// <summary>
        /// Gravity as a three component vector
        /// </summary>
        [GhostDefaultField(100, true)]
        public float3 gravityAcceleration;

        /// <summary>
        /// Normalized direction of gravity
        /// </summary>
        /// <returns>Vector of length one of direction of gravity</returns>
        public float3 Down => math.normalizesafe(this.gravityAcceleration);

        /// <summary>
        /// Normalized direction of up (opposite of down) given gravity
        /// </summary>
        public float3 Up => -1 * this.Down;
    }
}