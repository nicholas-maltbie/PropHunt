
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
    public class JoinGameClientSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<EnableProphuntGhostReceiveSystemComponent>();
        }

        protected override void OnUpdate()
        {
            Entities.WithNone<NetworkStreamInGame>().ForEach((Entity ent, ref NetworkIdComponent id) =>
            {
                PostUpdateCommands.AddComponent<NetworkStreamInGame>(ent);
                var req = PostUpdateCommands.CreateEntity();
                PostUpdateCommands.AddComponent<JoinGameRequest>(req);
                PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent { TargetConnection = ent });
            });

            // When the player Joins the session
            // We will attach an 'UpdateMaterialComponentData' to all the
            // entities that have a MaterialIdComponentData in order to do an update.
            Entities.ForEach((Entity ent, ref MaterialIdComponentData mat) => {
                PostUpdateCommands.AddComponent<UpdateMaterialComponentData>(ent);
            });
        }
    }

}
