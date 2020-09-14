using Unity.Entities;
using Unity.NetCode;
using static PropHunt.Game.ClientGameSystem;

namespace PropHunt.Client.Systems
{
    /// <summary>
    /// Secondary class to create connection object ot connect to the server
    /// </summary>
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class CreateConnectObjectSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (ConnectionSystem.connectRequested)
            {
                EntityManager.CreateEntity(typeof(InitClientGameComponent));
                ConnectionSystem.connectRequested = false;
            }
        }
    }

    /// <summary>
    /// System to handle disconnecting client from the server
    /// </summary>
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
        public static bool IsConnected {get; private set; }

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
                ConnectionSystem.IsConnected = EntityManager.HasComponent<NetworkStreamInGame>(ent);
                if (EntityManager.HasComponent<NetworkStreamDisconnected>(ent))
                {
                    ConnectionSystem.IsConnected = false;
                }
            });

            if (ConnectionSystem.disconnectRequested)
            {
                Entities.ForEach((Entity ent, ref NetworkStreamConnection conn) =>
                {
                    EntityManager.AddComponent(ent, typeof(NetworkStreamRequestDisconnect));
                });
                ConnectionSystem.disconnectRequested = false;
            }
        }
    }

}
