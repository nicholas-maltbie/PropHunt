using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Player movement authoring component, will attach a palyer
    /// movement attribute to an component as it is converted to an entity.
    /// </summary>
    public class PlayerMovementAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        /// <summary>
        /// Speed of movement in units per second
        /// </summary>
        public float moveSpeed = 5f;

        /// <summary>
        /// Multiplier for sprinting speed
        /// </summary>
        public float sprintMultiplier = 2f;

        /// <summary>
        /// Force of gravity exerted on player
        /// </summary>
        public Vector3 gravityForce = new Vector3(0, -9.8f, 0);

        /// <summary>
        /// Force of the player jump
        /// </summary>
        public float jumpForce = 5.0f;

        /// <summary>
        /// Maximum angle that a player can walk at
        /// </summary>
        public float maxWalkAngle = 45;

        /// <summary>
        /// Distance at which a player is considered 'grounded'
        /// </summary>
        public float groundCheckDistance = 0.1f;

        /// <summary>
        /// Decrease in momentum factor due to angle change when falling
        /// </summary>
        public float fallAnglePower = 1.2f;

        /// <summary>
        /// Decrease in momentum factor due to angle change when walking
        /// </summary>
        public float moveAnglePower = 2.0f;

        /// <summary>
        /// Power of pushing objects
        /// </summary>
        public float pushPower = 20.0f;

        /// <summary>
        /// Max number of bounces per frame when moving
        /// </summary>
        public int moveMaxBounces = 3;

        /// <summary>
        /// Max number of bounces per frame when falling
        /// </summary>
        public int fallMaxBounces = 2;

        /// <summary>
        /// Proportional decrease in momentum due to pushing an object
        /// </summary>
        public float pushDecay = 0.0f;

        /// <summary>
        /// Duration of jumping grace period
        ///     - Player can jump while not grounded, as long as they haven't been grounded longer than this amount of time
        ///     - Make sure the value of jumpGraceTime < jumpCooldown. 
        ///       Otherwise, double jumps are possible.
        /// </summary>
        public float jumpGraceTime = 0.0f;

        /// <summary>
        /// Time spent falling
        /// </summary>
        public float jumpCooldown = 0.1f;

        /// <summary>
        /// Distance to which the player will continue falling towards the ground
        /// </summary>
        public float groundFallingDistance = 0.01f;

        /// <summary>
        /// Maximum distance to push a character per second, should be the character's diameter
        /// </summary>
        public float maxPush = 2;

        /// <summary>
        /// Maximum amount a character can snap up when hitting steps/stairs (also any other object they hit very close
        /// th their feet).
        /// </summary>
        public float stepOffset = 0.1f;

        /// <summary>
        /// Maximum distance a player can be 'snapped' down per frame
        /// </summary>
        public float snapDownOffset = 0.25f;

        /// <summary>
        /// Maximum rate at which a player can be 'snapped' down in meters per second
        /// </summary>
        public float snapDownSpeed = 0.5f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new KCCMovementSettings()
            {
                moveSpeed = this.moveSpeed,
                sprintMultiplier = this.sprintMultiplier,
                moveMaxBounces = this.moveMaxBounces,
                moveAnglePower = this.moveAnglePower,
                movePushPower = this.pushPower,
                movePushDecay = this.pushDecay,
                fallMaxBounces = this.fallMaxBounces,
                fallAnglePower = this.fallAnglePower,
                fallPushPower = this.pushPower,
                fallPushDecay = this.pushDecay,
                maxPush = this.maxPush,
                stepOffset = this.stepOffset,
                snapDownOffset = this.snapDownOffset,
                snapDownSpeed = this.snapDownSpeed,
            });
            dstManager.AddComponentData(entity, new KCCJumping()
            {
                jumpForce = this.jumpForce,
                jumpGraceTime = this.jumpGraceTime,
                jumpCooldown = this.jumpCooldown,
            });
            dstManager.AddComponentData(entity, new KCCGrounded()
            {
                maxWalkAngle = this.maxWalkAngle,
                groundCheckDistance = this.groundCheckDistance,
                groundFallingDistance = this.groundFallingDistance,
            });
            dstManager.AddComponentData(entity, new KCCVelocity()
            {
                playerVelocity = float3.zero,
                worldVelocity = float3.zero
            });
            dstManager.AddComponentData(entity, new KCCGravity()
            {
                gravityAcceleration = this.gravityForce,
            });
        }
    }
}