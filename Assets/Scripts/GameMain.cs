using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace PropHunt.Game
{
    public class ProphuntClientServerControlSystem
    {
        /// <summary>
        /// Network address for local connection (loopback)
        /// </summary>
        public static string DefaultNetworkAddress = "127.0.0.1";

        /// <summary>
        /// Network port for default connection to the server
        /// </summary>
        public static ushort DefaultNetworkPort = 25623;

        /// <summary>
        /// Network address of server being connected to.
        /// </summary>
        public static string NetworkAddress = DefaultNetworkAddress;

        /// <summary>
        /// Port for host connection
        /// </summary>
        public static ushort NetworkPort = 25623;
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
            // Create singleton, require singleton for update so system runs once
            EntityManager.CreateEntity(typeof(InitServerGameComponent));
            Debug.Log("Creating server world");
        }

        protected override void OnUpdate()
        {
            // Destroy singleton to prevent system from running again
            EntityManager.DestroyEntity(GetSingletonEntity<InitServerGameComponent>());
            var network = World.GetExistingSystem<NetworkStreamReceiveSystem>();
            if (World.GetExistingSystem<ServerSimulationSystemGroup>() != null)
            {
                // Server world automatically listens for connections from any host
                NetworkEndPoint ep = NetworkEndPoint.AnyIpv4;
                ep.Port = ProphuntClientServerControlSystem.NetworkPort;
                network.Listen(ep);
            }
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
            // EntityManager.CreateEntity(typeof(InitClientGameComponent));
            Debug.Log("Creating client world");
        }

        protected override void OnUpdate()
        {
            EntityManager.DestroyEntity(GetSingletonEntity<InitClientGameComponent>());

            var network = World.GetExistingSystem<NetworkStreamReceiveSystem>();
            if (World.GetExistingSystem<ClientSimulationSystemGroup>() != null)
            {
                // Client worlds automatically connect to localhost
                UnityEngine.Debug.Log($"Connecting to {ProphuntClientServerControlSystem.NetworkAddress}:{ProphuntClientServerControlSystem.NetworkPort}");
                NetworkEndPoint ep = NetworkEndPoint.Parse(ProphuntClientServerControlSystem.NetworkAddress, ProphuntClientServerControlSystem.NetworkPort);
                network.Connect(ep);
            }
        }
    }
} // End namespace Prophunt.Game