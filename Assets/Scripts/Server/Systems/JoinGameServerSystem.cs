
using System;
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace PropHunt.Server.Systems
{
    /// <summary>
    /// When server receives go in game request, go in game and delete request
    /// </summary>
    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
    public class JoinGameServerSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<EnableProphuntGhostSendSystemComponent>();
        }

        protected override void OnUpdate()
        {
            Entities.WithNone<SendRpcCommandRequestComponent>().ForEach((Entity reqEnt, ref JoinGameRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
            {
                PostUpdateCommands.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);
                UnityEngine.Debug.Log(String.Format("Server setting connection {0} to in game", EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value));

                // Setup the character avatar
                var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
                var ghostId = ProphuntGhostSerializerCollection.FindGhostType<TestCharacterSnapshotData>();
                var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
                var player = EntityManager.Instantiate(prefab);
                EntityManager.SetComponentData(player, new PlayerId { playerId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value});


                PostUpdateCommands.AddBuffer<PlayerInput>(player);
                PostUpdateCommands.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent { targetEntity = player });
                PostUpdateCommands.SetComponent(player, new Translation { Value = new float3(0, 5, 0) } );
                
                PostUpdateCommands.DestroyEntity(reqEnt);
            });
        }
    }
}
