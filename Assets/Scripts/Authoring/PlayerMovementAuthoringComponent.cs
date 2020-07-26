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
        public float gravityForce = 9.8f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            PlayerMovement playerMovement = new PlayerMovement();
            playerMovement.moveSpeed = moveSpeed;
            playerMovement.viewRotationRate = viewRotationRate;
            playerMovement.sprintMultiplier = sprintMultiplier;
            playerMovement.gravityForce = gravityForce;
            dstManager.AddComponentData(entity, playerMovement);
        }
    }
}