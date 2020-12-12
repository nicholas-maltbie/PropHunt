using System;
using System.Threading;
using PropHunt.Client.Components;
using PropHunt.Game;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using static PropHunt.Game.ClientGameSystem;

namespace PropHunt.Client.Systems
{
    /// <summary>
    /// class to hold arguments for listening to connection from server
    /// </summary>
    public class ListenConnect : EventArgs { }

    /// <summary>
    /// class to hold arguments for listening to disconnection from server
    /// </summary>
    public class ListenDisconnect : EventArgs { }

    /// <summary>
    /// System to clear all ghosts on the client
    /// </summary>
    [UpdateBefore(typeof(ConnectionSystem))]
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class ClearClientGhostEntities : ComponentSystem
    {
        public struct ClientClearGhosts : IComponentData { };

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ClientClearGhosts>();
        }

        protected override void OnUpdate()
        {
            var buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

            // // Also delete the existing ghost objects
            // Entities.ForEach((
            //     Entity ent,
            //     ref GhostComponent ghost) =>
            // {
            //     buffer.DestroyEntity(ent);
            // });
            buffer.DestroyEntity(GetSingletonEntity<ClientClearGhosts>());
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
                    PostUpdateCommands.AddComponent(ent, typeof(NetworkStreamRequestDisconnect));
                    Entity clearEntity = PostUpdateCommands.CreateEntity();
                    PostUpdateCommands.AddComponent<ClearClientGhostEntities.ClientClearGhosts>(clearEntity);
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
    [UpdateAfter(typeof(ClientGameSystem))]
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
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    [UpdateBefore(typeof(ClientGameSystem))]
    public class ConnectionSystem : ComponentSystem
    {
        /// <summary>
        /// Public instance of connection system for client to access 
        /// from mono  behaviours
        /// </summary>
        public static ConnectionSystem Instance;

        /// <summary>
        /// Events for when connecting to server
        /// </summary>
        public event EventHandler<ListenConnect> OnConnect;

        /// <summary>
        /// Events for when disconnecting from server
        /// </summary>
        public event EventHandler<ListenDisconnect> OnDisconnect;

        /// <summary>
        /// Is the player attempting to connect to the server
        /// 
        /// 1 indicates requesting connect, 0 indicates no request
        /// </summary>
        private int requestConnect;

        /// <summary>
        /// Target settings when requesting a connection
        /// </summary>
        private NetworkControlSettings targetSettings;

        /// <summary>
        /// Is the player attempting to disconnect from the server
        /// 
        /// 1 indicates requesting disconnect, 0 indicates no request
        /// </summary>
        private int requestDisconnect;

        /// <summary>
        /// Request to connect to the server
        /// </summary>
        public virtual void RequestConnect(NetworkControlSettings connectionSettings)
        {
            Interlocked.Exchange(ref this.requestConnect, 1);
            this.targetSettings = connectionSettings;
        }

        /// <summary>
        /// Request to disconnect from the server
        /// </summary>
        public virtual void RequestDisconnect()
        {
            Interlocked.Exchange(ref this.requestDisconnect, 1);
        }

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ConnectionComponent>();
            EntityManager.CreateEntity(typeof(ConnectionComponent));
            ConnectionSystem.Instance = this;
        }

        protected override void OnUpdate()
        {
            var connectionSingleton = GetSingleton<ConnectionComponent>();
            // UnityEngine.Debug.Log(GetSingleton<NetworkControlSettings>().NetworkAddress);

            Entities.ForEach((Entity ent, ref NetworkStreamConnection conn) =>
            {
                if (connectionSingleton.attemptingConnect && EntityManager.HasComponent<NetworkStreamInGame>(ent))
                {
                    connectionSingleton.isConnected = true;
                    connectionSingleton.attemptingConnect = false;
                    OnConnect?.Invoke(this, new ListenConnect());
                }
                if (connectionSingleton.attemptingDisconnect && EntityManager.HasComponent<NetworkStreamDisconnected>(ent))
                {
                    connectionSingleton.isConnected = false;
                    connectionSingleton.attemptingDisconnect = false;
                    OnDisconnect?.Invoke(this, new ListenDisconnect());
                }
            });

            // Load components into connection entity in thread safe way
            if (Interlocked.CompareExchange(ref this.requestConnect, 0, 1) == 1)
            {
                this.SetSingleton<NetworkControlSettings>(this.targetSettings);
                connectionSingleton.requestConnect = true;
            }
            if (Interlocked.CompareExchange(ref this.requestDisconnect, 0, 1) == 1)
            {
                connectionSingleton.requestDisconnect = true;
            }

            SetSingleton(connectionSingleton);
        }
    }
}