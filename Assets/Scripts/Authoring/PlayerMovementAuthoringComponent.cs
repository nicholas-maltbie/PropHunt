using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Physics;
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
        /// Speed of view rotation in radians per second
        /// </summary>
        public float viewRotationRate = 3.14f;

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
        public float anglePowerFall = 1.2f;

        /// <summary>
        /// Decrease in momentum factor due to angle change when walking
        /// </summary>
        public float anglePowerMove = 2.0f;

        /// <summary>
        /// Power of pushing objects
        /// </summary>
        public float pushPower = 10.0f;

        /// <summary>
        /// Max number of bounces per frame when moving
        /// </summary>
        public int maxBouncesMove = 3;

        /// <summary>
        /// Max number of bounces per frame when falling
        /// </summary>
        public int maxBouncesFall = 2;

        /// <summary>
        /// Proportional decrease in momentum due to pushing an object
        /// </summary>
        public float pushDecay = 0.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            PlayerMovement playerMovement = new PlayerMovement(){
                moveSpeed = this.moveSpeed,
                viewRotationRate = this.viewRotationRate,
                sprintMultiplier = this.sprintMultiplier,
                jumpForce = this.jumpForce,
                groundCheckDistance = this.groundCheckDistance,
                maxWalkAngle = this.maxWalkAngle,
                gravityForce = this.gravityForce,
                pushPower = this.pushPower,
                anglePowerFall = this.anglePowerFall,
                anglePowerMove = this.anglePowerMove,
                maxBouncesMove = this.maxBouncesMove,
                maxBouncesFall = this.maxBouncesFall,
                pushDecay = this.pushDecay
            };
            dstManager.AddComponentData(entity, playerMovement);
        }
    }
}
