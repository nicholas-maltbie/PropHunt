using PropHunt.Server.Components;
using Unity.Entities;
using UnityEngine;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Behaviour to create a moving platform component based on settings
    /// </summary>
    public class RandomizedStateRandomWrapper : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // Create a rng with a random seed value
            dstManager.AddComponentData(entity, new RandomWrapper()
            {
                random = new Unity.Mathematics.Random((uint) UnityEngine.Random.Range(1, int.MaxValue))
            });
        }
    }
}