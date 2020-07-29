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

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            PlayerMovement playerMovement = new PlayerMovement();
            playerMovement.moveSpeed = this.moveSpeed;
            playerMovement.viewRotationRate = this.viewRotationRate;
            playerMovement.sprintMultiplier = this.sprintMultiplier;
            playerMovement.jumpForce = this.jumpForce;
            playerMovement.groundCheckDistance = this.groundCheckDistance;
            playerMovement.maxWalkAngle = this.maxWalkAngle;
            playerMovement.gravityForce = this.gravityForce;
            dstManager.AddComponentData(entity, playerMovement);
        }
    }
}
