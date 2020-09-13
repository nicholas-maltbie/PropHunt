
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
    public class UpdateMaterialSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var ecb = EntityManager.World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

            Entities.WithNone<NetworkStreamInGame>().ForEach((Entity ent, ref UpdateMaterialComponentData updateMatTag, ref MaterialIdComponentData materialId) =>
            {
                // We only want to update this entity once, so this tag will be removed afterwards.
                PostUpdateCommands.RemoveComponent<UpdateMaterialComponentData>(ent);
                var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(ent);
                var material = SharedMaterials.Instance.GetMaterialById(materialId.materialId);

                // Set the material.
                ecb.SetSharedComponent(ent, new RenderMesh
                {
                    mesh = renderMesh.mesh,
                    material = material,
                    subMesh = renderMesh.subMesh,
                    layer = renderMesh.layer,
                    castShadows = renderMesh.castShadows,
                    needMotionVectorPass = renderMesh.needMotionVectorPass,
                    receiveShadows = renderMesh.receiveShadows,
                });
            });
        }
    }

}
