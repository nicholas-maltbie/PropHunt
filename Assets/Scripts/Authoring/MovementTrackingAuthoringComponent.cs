using Unity.Entities;
using UnityEngine;
using PropHunt.Mixed.Components;

namespace PropHunt.Authoring
{
    public class MovementTrackingAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent(entity, ComponentType.ReadWrite<MovementTracking>());
        }
    }
}