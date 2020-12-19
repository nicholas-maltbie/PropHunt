
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Client.Systems
{

    /// <summary>
    /// When client has a connection with network id, go in game and tell server to also go in game
    /// </summary>
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    [UpdateAfter(typeof(GhostSendSystem))]
    public class JoinGameClientSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.WithNone<NetworkStreamInGame>().ForEach((Entity ent, ref NetworkIdComponent id) =>
            {
                EntityManager.AddComponent<NetworkStreamInGame>(ent);
                var req = EntityManager.CreateEntity();
                EntityManager.AddComponent<JoinGameRequest>(req);
                EntityManager.AddComponentData(req, new SendRpcCommandRequestComponent { TargetConnection = ent });
            });

            // When the player Joins the session
            // We will attach an 'UpdateMaterialComponentData' to all the
            // entities that have a MaterialIdComponentData in order to do an update.
            Entities.ForEach((Entity ent, ref MaterialIdComponentData mat) =>
            {
                PostUpdateCommands.AddComponent<UpdateMaterialComponentData>(ent);
            });
        }
    }

}