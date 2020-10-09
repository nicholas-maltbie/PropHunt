using PropHunt.Client.Components;
using Unity.Entities;
using Unity.NetCode;
using Unity.Scenes;
using UnityEngine;
using static PropHunt.Game.ClientGameSystem;

namespace PropHunt.Client.Systems
{
    /// <summary>
    /// System to clear all ghosts on the client
    /// </summary>
    [UpdateBefore(typeof(ConnectionSystem))]
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class ClearClientGhostEntities : SystemBase
    {
        protected EndSimulationEntityCommandBufferSystem commandBufferSystem;

        public struct ClientClearGhosts : IComponentData { };

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ClientClearGhosts>();
            this.commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var buffer = this.commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            // Also delete the existing ghost objects
            Entities.ForEach((
                Entity ent,
                int entityInQueryIndex,
                ref GhostComponent ghost) =>
            {
                buffer.DestroyEntity(entityInQueryIndex, ent);
            }).ScheduleParallel();
            this.commandBufferSystem.CreateCommandBuffer().DestroyEntity(GetSingletonEntity<ClientClearGhosts>());
        }
    }

    /// <summary>
    /// System to handle disconnecting from the server
    /// </summary>
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    [UpdateAfter(typeof(ConnectToServerSystem))]
    public class DisconnectFromServerSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ConnectionComponent>();
        }

        protected override void OnUpdate()
        {
            var connectionSingleton = GetSingleton<ConnectionComponent>();
            if (connectionSingleton.requestDisconnect)
            {
                Debug.Log("Attempting to disconnect");
                Entities.ForEach((Entity ent, ref NetworkStreamConnection conn) =>
                {
                    EntityManager.AddComponent(ent, typeof(NetworkStreamRequestDisconnect));
                    EntityManager.CreateEntity(ComponentType.ReadOnly(typeof(ClearClientGhostEntities.ClientClearGhosts)));
                });
                connectionSingleton.requestDisconnect = false;
                connectionSingleton.attemptingDisconnect = true;

                SetSingleton(connectionSingleton);
            }
        }
    }

    /// <summary>
    /// Secondary class to create connection object ot connect to the server
    /// </summary>
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    [UpdateAfter(typeof(ConnectionSystem))]
    public class ConnectToServerSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ConnectionComponent>();
        }

        protected override void OnUpdate()
        {
            var connectionSingleton = GetSingleton<ConnectionComponent>();
            if (connectionSingleton.requestConnect)
            {
                EntityManager.CreateEntity(typeof(InitClientGameComponent));
                connectionSingleton.requestConnect = false;
                connectionSingleton.attemptingConnect = true;

                SetSingleton(connectionSingleton);
            }
        }
    }

    /// <summary>
    /// System to handle disconnecting client from the server
    /// </summary>
    [UpdateBefore(typeof(GhostReceiveSystem))]
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class ConnectionSystem : ComponentSystem
    {
        /// <summary>
        /// Is the player attempting to connect to the server
        /// </summary>
        private static bool requestConnect;

        /// <summary>
        /// Is the player attempting to disconnect from the server
        /// </summary>
        private static bool requestDisconnect;

        /// <summary>
        /// Invoke whenever a disconnect is requested
        /// </summary>
        public static void DisconnectFromServer()
        {
            ConnectionSystem.requestDisconnect = true;
        }

        /// <summary>
        /// Invoke whenever a connect is requested
        /// </summary>
        public static void ConnectToServer()
        {
            ConnectionSystem.requestConnect = true;
        }

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ConnectionComponent>();
            EntityManager.CreateEntity(typeof(ConnectionComponent));
        }

        protected override void OnUpdate()
        {
            var connectionSingleton = GetSingleton<ConnectionComponent>();
            
            Entities.ForEach((Entity ent, ref NetworkStreamConnection conn) =>
            {
                if (EntityManager.HasComponent<NetworkStreamInGame>(ent))
                {
                    connectionSingleton.isConnected = true;
                    connectionSingleton.attemptingConnect = false;
                }
                if (EntityManager.HasComponent<NetworkStreamDisconnected>(ent))
                {
                    connectionSingleton.isConnected = false;
                    connectionSingleton.attemptingDisconnect = false;
                }
            });

            // Load static components into connection entity
            if (ConnectionSystem.requestConnect)
            {
                connectionSingleton.requestConnect = ConnectionSystem.requestConnect;
                ConnectionSystem.requestConnect = false;
            }
            if (ConnectionSystem.requestDisconnect)
            {
                connectionSingleton.requestDisconnect = ConnectionSystem.requestDisconnect;
                ConnectionSystem.requestDisconnect = false;

            }

            SetSingleton(connectionSingleton);
        }
    }

}