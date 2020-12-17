using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Behaviour to create a moving platform component based on settings
    /// </summary>
    public class MovementTrackingAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool avoidTransferMomentum;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // Create a rng with a random seed value
            dstManager.AddComponentData(entity, new MovementTracking()
            {
                avoidTransferMomentum = this.avoidTransferMomentum,
                ChangeAttitude = quaternion.Euler(float3.zero),
                Displacement = float3.zero
            });
        }
    }
}