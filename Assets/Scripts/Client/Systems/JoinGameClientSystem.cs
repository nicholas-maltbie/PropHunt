
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Client.Systems
{

    /// <summary>
    /// When client has a connection with network id, go in game and tell server to also go in game
    /// </summary>
    [UpdateBefore(typeof(GhostSpawnSystemGroup))]
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class JoinGameClientSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            var ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

            Entities.WithNone<NetworkStreamInGame>().ForEach((Entity ent, ref NetworkIdComponent id) =>
            {
                ecb.AddComponent<NetworkStreamInGame>(ent);
                var req = ecb.CreateEntity();
                ecb.AddComponent<JoinGameRequest>(req);
                ecb.AddComponent(req, new SendRpcCommandRequestComponent { TargetConnection = ent });
            });

            // When the player Joins the session
            // We will attach an 'UpdateMaterialComponentData' to all the
            // entities that have a MaterialIdComponentData in order to do an update.
            Entities.ForEach((Entity ent, ref MaterialIdComponentData mat) =>
            {
                ecb.AddComponent<UpdateMaterialComponentData>(ent);
            });
        }
    }

}