using Unity.Entities;
using UnityEngine;
using PropHunt.Mixed.Components;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Behaviour to attach a movement tracking component to an entity on creation
    /// </summary>
    public class MovementTrackingAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent(entity, ComponentType.ReadWrite<MovementTracking>());
        }
    }
}