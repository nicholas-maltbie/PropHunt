using PropHunt.Mixed.Components;
using Unity.Entities;
using UnityEngine;

namespace PropHunt.Authoring
{

    /// <summary>
    /// Attaches Player Moveable Tag to an Entity.
    /// </summary>
    public class PlayerMoveableAuthoringComponent : MonoBehaviour
    {
        /// <summary>
        /// Player movement speed.
        /// </summary>
        public float Speed = 5.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            PlayerMoveable playerMoveableComponentData = new PlayerMoveable();
            playerMoveableComponentData.speed = this.Speed;
            dstManager.AddComponentData(entity, playerMoveableComponentData);
        }
        
    }

}
