using UnityEngine;
using Unity.Entities;
using static PropHunt.Game.ProphuntClientServerControlSystem;
using UnityEngine.UI;
using System.Net;

namespace PropHunt.UI
{
    /// <summary>
    /// Action to connect client to server
    /// </summary>
    public class ConnectAction : MonoBehaviour
    {
        /// <summary>
        /// Text object with server address
        /// </summary>
        public InputField serverAddress;

        /// <summary>
        /// Text object with server port
        /// </summary>
        public InputField serverPort;

        public void OnEnable()
        {
            this.serverAddress.text =
                PropHunt.Game.ProphuntClientServerControlSystem.DefaultNetworkAddress;
            this.serverPort.text =
                PropHunt.Game.ProphuntClientServerControlSystem.NetworkPort.ToString();
        }

        /// <summary>
        /// Command to start connecting to the server
        /// </summary>
        public void ConnectedToServer()
        {
            // Setup variables
            string networkAddress = serverAddress.text;
            IPAddress parsedAddress;
            ushort networkPort;
            try
            {
                // Parse parameters
                IPAddress.TryParse(networkAddress, out parsedAddress);
                ushort.TryParse(serverPort.text, out networkPort);

                // Verify parameters
                // TODO: write verify code

                // Assign connection parameters
                PropHunt.Game.ProphuntClientServerControlSystem.NetworkAddress = networkAddress;
                PropHunt.Game.ProphuntClientServerControlSystem.NetworkPort = networkPort;

                EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
                var initEntity = em.CreateEntity(typeof(InitializeClientServer));
            }
            catch
            {
                UnityEngine.Debug.Log(
                    $"Failed to parse IP Address {serverAddress.text} and Port {serverPort.text}");
            }
        }
    }
}
