using Unity.Entities;
using UnityEngine;
using PropHunt.Mixed.Components;

namespace PropHunt.Authoring
{

    /// <summary>
    /// Authoring component to attach a player id to an entity
    /// </summary>
    public class PlayerIdAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new PlayerId());
        }
    }

}