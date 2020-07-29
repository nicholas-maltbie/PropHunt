using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Unity.Rendering;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Player movement authoring component, will attach a palyer
    /// movement attribute to an component as it is converted to an entity.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class CubePropertiesAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        /// <summary>
        /// Speed of movement in units per second
        /// </summary>
        public Color materialColor;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity,  new MaterialColor{
                Value = (Vector4) materialColor,
            });
        }
    }
}
