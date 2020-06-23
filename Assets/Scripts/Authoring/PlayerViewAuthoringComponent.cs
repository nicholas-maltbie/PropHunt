using PropHunt.Mixed.Components;
using Unity.Entities;
using UnityEngine;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Player view authoring component, will attach a palyer
    /// view attribute to an component as it is converted to an entity.
    /// </summary>
    public class PlayerViewAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new PlayerView());
        }
    }
}
