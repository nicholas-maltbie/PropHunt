using System;
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace PropHunt.Server.Systems
{
    /// <summary>
    /// When server receives go in game request, go in game and delete request
    /// </summary>
    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
    [UpdateAfter(typeof(GhostSendSystem))]
    public class JoinGameServerSystem : ComponentSystem
    {
        /// <summary>
        /// 
        /// </summary>
        private int characterId;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<GhostPrefabCollectionComponent>();
        }

        protected int GetPlayerGhostIndex(DynamicBuffer<GhostPrefabBuffer> ghostPrefabBuffers)
        {
            for (int i = 0; i < ghostPrefabBuffers.Length; i++)
            {
                var found = ghostPrefabBuffers[i].Value;
                // The prefab with a PlayerId will be returned
                if (EntityManager.HasComponent<PlayerId>(found))
                {
                    return i;
                }
            }
            return -1;
        }

        protected override void OnUpdate()
        {
            Entities.WithNone<SendRpcCommandRequestComponent>().ForEach((Entity reqEnt, ref JoinGameRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
            {
                int connectionId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value;

                EntityManager.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);
                UnityEngine.Debug.Log(String.Format("Server setting connection {0} to in game", connectionId));

                // Setup the character avatar
                Entity ghostCollection = GetSingletonEntity<GhostPrefabCollectionComponent>();
                DynamicBuffer<GhostPrefabBuffer> ghostPrefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection);
                int ghostId = GetPlayerGhostIndex(ghostPrefabs);
                var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection)[ghostId].Value;
                var player = PostUpdateCommands.Instantiate(prefab);
                PostUpdateCommands.AddComponent<PlayerId>(player);
                PostUpdateCommands.AddComponent<Translation>(player);
                PostUpdateCommands.AddComponent<GhostOwnerComponent>(player);
                PostUpdateCommands.SetComponent(player, new PlayerId { playerId = connectionId });
                PostUpdateCommands.SetComponent(player, new Translation { Value = new float3(0, 5, 0) });
                PostUpdateCommands.SetComponent(player, new GhostOwnerComponent { NetworkId = connectionId });

                PostUpdateCommands.AddBuffer<PlayerInput>(player);

                PostUpdateCommands.AddComponent<CommandTargetComponent>(reqSrc.SourceConnection);
                PostUpdateCommands.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent { targetEntity = player });

                PostUpdateCommands.DestroyEntity(reqEnt);
            });
        }
    }
}