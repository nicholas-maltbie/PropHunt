using PropHunt.SceneManagement;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics.Systems;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PropHunt.Client.Systems.ClearClientGhostEntities;
using static PropHunt.Game.ClientGameSystem;

namespace PropHunt.Client.Systems
{
    /// <summary>
    /// System to clear all ghosts on the client
    /// </summary>
    [UpdateAfter(typeof(ConnectionSystem))]
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
            // Also delete the existing ghost objects after disconnecting from the server
            if (ConnectionSystem.IsConnected == false)
            {
                var buffer = this.commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
                Entities.ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    GhostComponent ghost) =>
                    {
                        buffer.DestroyEntity(entityInQueryIndex, entity);
                    }
                ).ScheduleParallel();

                World.GetOrCreateSystem<SceneSystem>().UnloadScene(SubSceneReferences.Instance.GetSceneByName("TestRoom").SceneGUID);
                this.commandBufferSystem.CreateCommandBuffer().DestroyEntity(GetSingletonEntity<ClientClearGhosts>());
            }
        }
    }

    /// <summary>
    /// Secondary class to create connection object ot connect to the server
    /// </summary>
    [UpdateBefore(typeof(GhostReceiveSystem))]
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class CreateConnectObjectSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (ConnectionSystem.connectRequested)
            {
                EntityManager.CreateEntity(typeof(InitClientGameComponent));
                World.GetOrCreateSystem<SceneSystem>().LoadSceneAsync(SubSceneReferences.Instance.GetSceneByName("TestRoom").SceneGUID);
                ConnectionSystem.connectRequested = false;
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
        /// Has a disconnect been requested
        /// </summary>
        public static bool disconnectRequested;

        /// <summary>
        /// Has a connection been requested
        /// </summary>
        public static bool connectRequested;

        /// <summary>
        /// Is the client currently connected
        /// </summary>
        public static bool IsConnected { get; private set; }

        /// <summary>
        /// Invoke whenever a disconnect is requested
        /// </summary>
        public static void DisconnectFromServer()
        {
            ConnectionSystem.disconnectRequested = true;
        }

        /// <summary>
        /// Invoke whenever a connect is requested
        /// </summary>
        public static void ConnectToServer()
        {
            ConnectionSystem.connectRequested = true;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity ent, ref NetworkStreamConnection conn) =>
            {
                if (EntityManager.HasComponent<NetworkStreamInGame>(ent))
                {
                    ConnectionSystem.IsConnected = true;
                }
                if (EntityManager.HasComponent<NetworkStreamDisconnected>(ent))
                {
                    ConnectionSystem.IsConnected = false;
                }
            });
            if (ConnectionSystem.disconnectRequested)
            {
                Debug.Log("Attempting to disconnect");
                EntityManager.CreateEntity(typeof(ClientClearGhosts));
                Entities.ForEach((Entity ent, ref NetworkStreamConnection conn) =>
                {
                    PostUpdateCommands.AddComponent(ent, typeof(NetworkStreamRequestDisconnect));
                });
                ConnectionSystem.disconnectRequested = false;
            }
        }
    }

}