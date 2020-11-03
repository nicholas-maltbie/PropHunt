using PropHunt.Mixed;
using PropHunt.Mixed.Components;
using Unity.Entities;
using UnityEngine;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Player view authoring component, will attach a palyer
    /// view attribute to an component as it is converted to an entity.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class GhostSharedMaterialAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // var meshRendererMaterial = (MeshRenderer) gameObject.GetComponent<MeshRenderer>();
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            var materialId = SharedMaterials.Instance.GetIdForMaterial(meshRenderer.sharedMaterial);
            dstManager.AddComponentData(entity, new MaterialIdComponentData{
                materialId = materialId
            });
        }
    }
}