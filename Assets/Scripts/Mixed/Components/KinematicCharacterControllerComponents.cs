using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Player movement settings for when a Kinematic Character Controller is
    /// being controlled.
    /// </summary>
    [GhostComponent]
    public struct KCCMovementSettings : IComponentData
    {
        /// <summary>
        /// Player movement speed in units per second.
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float moveSpeed;

        /// <summary>
        /// Multiplier for sprint speed
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float sprintMultiplier;

        /// <summary>
        /// Sprint speed is move speed by a multiplier
        /// </summary>
        public float SprintSpeed => moveSpeed * sprintMultiplier;

        /// <summary>
        /// Max number of bounces per frame when moving
        /// </summary>
        [GhostField]
        public int moveMaxBounces;

        /// <summary>
        /// Decrease in momentum factor due to angle change when walking
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float moveAnglePower;

        /// <summary>
        /// Power of pushing objects
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float movePushPower;

        /// <summary>
        /// Proportional decrease in momentum due to pushing an object when moving
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float movePushDecay;

        /// <summary>
        /// Max number of bounces per frame when falling
        /// </summary>
        [GhostField]
        public int fallMaxBounces;

        /// <summary>
        /// Weight of object when falling (pushing) onto objects
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float fallPushPower;

        /// <summary>
        /// Decrease in momentum factor due to angle change when falling
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float fallAnglePower;

        /// <summary>
        /// Proportional decrease in momentum due to pushing an object when falling
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float fallPushDecay;

        /// <summary>
        /// Center of the character for pushing out of objects relative to the character transform
        /// </summary>
        [GhostField]
        public float3 characterCenter;
    }

    /// <summary>
    /// Settings for if a player is currently jumping
    /// </summary>
    public struct KCCJumping : IComponentData
    {
        /// <summary>
        /// Force of the player jump
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float jumpForce;

        /// <summary>
        /// Is the KCC attempting to jump?
        /// </summary>
        [GhostField]
        public bool attemptingJump;

        /// <summary>
        /// Duration of jumping grace period
        ///     - Player can jump while not grounded, as long as they haven't been grounded longer than this amount of time
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float jumpGraceTime;

        /// <summary>
        /// Minimum time between jumps
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float jumpCooldown;

        /// <summary>
        /// Time elapsed since last jump. 
        /// Used in conjunction with jumpCooldown
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float timeElapsedSinceJump;


    }

    /// <summary>
    /// Settings and data for if a character is currently grounded
    /// </summary>
    [GhostComponent]
    public struct KCCGrounded : IComponentData
    {
        /// <summary>
        /// Max angle that the character can walk at
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float maxWalkAngle;

        /// <summary>
        /// Is the character currently falling (not on the ground or on the ground and
        /// it's too steep)
        /// </summary>
        public bool Falling => !(this.onGround && this.distanceToGround <= this.groundFallingDistance) || this.angle > this.maxWalkAngle;

        /// <summary>
        /// Distance to check if character is thouching ground
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float groundCheckDistance;

        /// <summary>
        /// Threshold for when the player will stop falling onto the ground. Should be a value smaller
        /// than or equal to groundCheckDistance.
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float groundFallingDistance;

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

        /// <summary>
        /// Index of rigidbody of grounded object
        /// </summary>
        public int groundedRBIndex;

        /// <summary>
        /// Point on which the object is touching the ground
        /// </summary>
        public float3 groundedPoint;

        /// <summary>
        /// Entity hit
        /// </summary>
        public Entity hitEntity;

        /// <summary>
        /// Time spent falling
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float elapsedFallTime;

        /// <summary>
        /// Distance to ground on the previous frame
        /// </summary>
        public float previousDistanceToGround;

        /// <summary>
        /// Was this on the ground previous frame
        /// </summary>
        public bool previousOnGround;

        /// <summary>
        /// Angle of previously hit object
        /// </summary>
        public float previousAngle;

        /// <summary>
        /// Was this falling the previous frame
        /// </summary>
        public bool PreviousFalling => !(this.previousOnGround && this.previousDistanceToGround <= this.groundFallingDistance) || this.previousAngle > this.maxWalkAngle;
    }

    /// <summary>
    /// Structure holding a Kinematic Character Controller's current velocity broken in to parts,
    /// velocity due to player input and velocity due to the world around them.
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct KCCVelocity : IComponentData
    {
        /// <summary>
        /// Velocity due to player input
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float3 playerVelocity;

        /// <summary>
        /// Velocity due to world forces
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float3 worldVelocity;

        /// <summary>
        /// Velocity of floor for following grounded components, used as a holding variable
        /// </summary>
        public float3 floorVelocity;
    }

    /// <summary>
    /// Structure for controlling the direction and force of gravity to a kinematic
    /// character controller.
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct KCCGravity : IComponentData
    {

        /// <summary>
        /// Gravity as a three component vector
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
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