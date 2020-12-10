using PropHunt.Constants;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace PropHunt.Game
{
    public struct NetworkControlSettings : IComponentData
    {
        /// <summary>
        /// Network address of server being connected to.
        /// </summary>
        public FixedString64 NetworkAddress;

        /// <summary>
        /// Port for host connection
        /// </summary>
        public ushort NetworkPort;

        public static NetworkControlSettings GetDefault()
        {
            return new NetworkControlSettings
            {
                NetworkAddress = ProphuntClientServerControlSystem.DefaultNetworkAddress,
                NetworkPort = ProphuntClientServerControlSystem.DefaultNetworkPort
            };
        }
    }

    public class NetworkControlSettingsSystem : ComponentSystem
    {
        // global instance of network control settings
        public static NetworkControlSettingsSystem Instance;

        protected override void OnCreate()
        {
            NetworkControlSettingsSystem.Instance = this;
            EntityManager.CreateEntity(typeof(NetworkControlSettings));
            this.SetSingleton<NetworkControlSettings>(NetworkControlSettings.GetDefault());
        }

        protected override void OnUpdate() { }
    }

    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
    public class ServerGameSystem : ComponentSystem
    {
        // Singleton component to trigger connections once from a control system
        public struct InitServerGameComponent : IComponentData
        {
        }

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<InitServerGameComponent>();
            RequireSingletonForUpdate<NetworkControlSettings>();
            // Create singleton, require singleton for update so system runs once
            EntityManager.CreateEntity(typeof(InitServerGameComponent));
            Debug.Log("Creating server world");
        }

        protected override void OnUpdate()
        {
            // Destroy singleton to prevent system from running again
            EntityManager.DestroyEntity(GetSingletonEntity<InitServerGameComponent>());
#if UNITY_SERVER || UNITY_EDITOR
            var network = World.GetExistingSystem<NetworkStreamReceiveSystem>();
            if (World.GetExistingSystem<ServerSimulationSystemGroup>() != null)
            {
                // Server world automatically listens for connections from any host
                NetworkEndPoint ep = NetworkEndPoint.AnyIpv4;
                ep.Port = GetSingleton<NetworkControlSettings>().NetworkPort;
                network.Listen(ep);
            }
#endif
        }
    }

    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class ClientGameSystem : ComponentSystem
    {
        // Singleton component to trigger connections once from a control system
        public struct InitClientGameComponent : IComponentData
        {
        }
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<InitClientGameComponent>();
            Debug.Log("Creating client world");
        }

        protected override void OnUpdate()
        {
            EntityManager.DestroyEntity(GetSingletonEntity<InitClientGameComponent>());

#if UNITY_CLIENT || UNITY_EDITOR
            var network = World.GetExistingSystem<NetworkStreamReceiveSystem>();
            if (World.GetExistingSystem<ClientSimulationSystemGroup>() != null)
            {
                NetworkControlSettings settings = this.GetSingleton<NetworkControlSettings>();
                // Client worlds automatically connect to localhost
                UnityEngine.Debug.Log($"Connecting to {settings.NetworkAddress}:{settings.NetworkPort}");
                PropHunt.UI.ConnectAction.Instance.SetDebugText($"Attempting to connect to {settings.NetworkAddress}:{settings.NetworkPort}");
                NetworkEndPoint ep = NetworkEndPoint.Parse(settings.NetworkAddress.ConvertToString(), settings.NetworkPort);
                network.Connect(ep);
            }
#endif
        }
    }
} // End namespace Prophunt.Game