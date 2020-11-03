
using PropHunt.Mixed.Components;
using PropHunt.Mixed;
using Unity.Entities;
using Unity.NetCode;
using Unity.Rendering;

namespace PropHunt.Client.Systems
{
    /// <summary>
    /// When a material of an entity needs to be updated, this system will update the render mesh.
    /// </summary>
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class UpdateMaterialSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = EntityManager.World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
            var getSharedMaterialsFromEntity = GetComponentDataFromEntity<SharedMaterialData>(true);
            var sharedMaterialEntity = EntityManager.CreateEntityQuery(typeof(SharedMaterialData)).GetSingletonEntity();

            Entities.WithNone<NetworkStreamInGame>()
            .WithReadOnly(getSharedMaterialsFromEntity)
            .ForEach((Entity ent, in UpdateMaterialComponentData updateMatTag, in MaterialIdComponentData materialId) =>
            {
                // We only want to update this entity once, so this tag will be removed afterwards.
                ecb.RemoveComponent<UpdateMaterialComponentData>(ent);
                var sharedMaterialData = getSharedMaterialsFromEntity[sharedMaterialEntity];

                var material = sharedMaterialData.sharedMaterialsBlobAssetRef.Value.Materials[materialId.materialId];
                var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(ent);

                // Set the material.
                ecb.SetSharedComponent(ent, new RenderMesh
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