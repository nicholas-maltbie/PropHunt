using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Unity.Rendering;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Cube Properties authoring component.
    /// Will attach a cube material color attribute as it is converted to an entity.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class CubePropertiesAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        /// <summary>
        /// The color of the material of the cube.
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
