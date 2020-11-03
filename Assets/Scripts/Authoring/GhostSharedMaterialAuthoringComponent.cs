using PropHunt.Mixed;
using PropHunt.Mixed.Components;
using Unity.Entities;
using UnityEngine;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Ghost shared material authoring component
    /// Will attach a Material Id to a ghosted component that uses a shared material.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class GhostSharedMaterialAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // We obtain the Mesh Renderer from the component, and get the material from there.
            // That way we don't have to manually link the materials and Ids.
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            var materialId = SharedMaterials.Instance.GetIdForMaterial(meshRenderer.sharedMaterial);
            dstManager.AddComponentData(entity, new MaterialIdComponentData
            {
                materialId = materialId
            });
        }
    }
}