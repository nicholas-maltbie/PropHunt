using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Client.Systems
{

    /// <summary>
    /// System to handle disconnecting client from the server
    /// </summary>
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class ConnectionSystem : ComponentSystem
    {
        /// <summary>
        /// Has a disconnect been requested
        /// </summary>
        private static bool disconnectRequested;

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

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<EnableProphuntGhostReceiveSystemComponent>();
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

            UnityEngine.Debug.Log($"Current connection state is {ConnectionSystem.IsConnected}");

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
