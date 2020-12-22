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
            var ecb = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
            Entities.WithNone<SendRpcCommandRequestComponent>().ForEach((Entity reqEnt, ref JoinGameRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
            {
                int connectionId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value;

                ecb.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);
                UnityEngine.Debug.Log(String.Format("Server setting connection {0} to in game", connectionId));

                // Setup the character avatar
                Entity ghostCollection = GetSingletonEntity<GhostPrefabCollectionComponent>();
                DynamicBuffer<GhostPrefabBuffer> ghostPrefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection);
                int ghostId = GetPlayerGhostIndex(ghostPrefabs);
                var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection)[ghostId].Value;
                var player = ecb.Instantiate(prefab);
                ecb.AddComponent<PlayerId>(player);
                ecb.AddComponent<Translation>(player);
                ecb.AddComponent<GhostOwnerComponent>(player);
                ecb.SetComponent(player, new PlayerId { playerId = connectionId });
                ecb.SetComponent(player, new Translation { Value = new float3(0, 5, 0) });
                ecb.SetComponent(player, new GhostOwnerComponent { NetworkId = connectionId });

                ecb.AddBuffer<PlayerInput>(player);

                ecb.AddComponent<CommandTargetComponent>(reqSrc.SourceConnection);
                ecb.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent { targetEntity = player });

                PostUpdateCommands.DestroyEntity(reqEnt);
            });
        }
    }
}