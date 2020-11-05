
using PropHunt.Mixed.Components;
using PropHunt.Mixed;
using Unity.Entities;
using Unity.NetCode;
using Unity.Rendering;
using Unity.Collections;

namespace PropHunt.Client.Systems
{
    /// <summary>
    /// When a material of an entity needs to be updated, this system will update the render mesh.
    /// </summary>
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class UpdateMaterialSystem : SystemBase
    {
        EntityQuery updateSharedMaterialsQuery;
        EntityQuery sharedMaterialsQuery;

        protected override void OnCreate()
        {
            sharedMaterialsQuery = GetEntityQuery(typeof(SharedMaterialData));
            updateSharedMaterialsQuery = GetEntityQuery(typeof(MaterialIdComponentData), typeof(RenderMesh));
            updateSharedMaterialsQuery.SetChangedVersionFilter(typeof(MaterialIdComponentData));
        }

        protected override void OnUpdate()
        {
            // Create an Entity Command Buffer that will be used to do updates on RenderMesh shared component data.
            var ecb = EntityManager.World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

            // Obtain the Materials Library Entity by querying the Shared Material data.
            var sharedMaterialsArray = sharedMaterialsQuery.ToComponentDataArray<SharedMaterialData>(Allocator.Temp);

            // Check that any shared materials exists before continuing.
            if(sharedMaterialsArray.Length == 0){
                return;
            }

            // Right now we only have one shared materials entity configured, but maybe this could be extended in the future.
            var sharedMaterials = sharedMaterialsArray[0];
            Entities
            .WithNone<NetworkStreamInGame>()
            .WithStoreEntityQueryInField(ref updateSharedMaterialsQuery)
            .ForEach((Entity entity, int nativeThreadIndex, in MaterialIdComponentData materialId, in RenderMesh renderMesh) => {
                
                // Query the material data entity by using the singleton entity as key.
                var material = sharedMaterials.sharedMaterialsBlobAssetRef.Value.Materials[materialId.materialId];
                // Set the material.
                ecb.SetSharedComponent(entity, new RenderMesh
                {
                    mesh = renderMesh.mesh,
                    material = material.Value,
                    subMesh = renderMesh.subMesh,
                    layer = renderMesh.layer,
                    castShadows = renderMesh.castShadows,
                    needMotionVectorPass = renderMesh.needMotionVectorPass,
                    receiveShadows = renderMesh.receiveShadows,
                });
            }).WithoutBurst().Run();
        }
    }
}