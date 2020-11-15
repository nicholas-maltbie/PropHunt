using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using System.Net;
using static PropHunt.Game.ClientGameSystem;
using PropHunt.Game;
using PropHunt.Client.Systems;
using System;

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
            this.serverAddress.text = PropHunt.Game.ProphuntClientServerControlSystem.DefaultNetworkAddress;
            this.serverPort.text = PropHunt.Game.ProphuntClientServerControlSystem.NetworkPort.ToString();
        }

        /// <summary>
        /// Command to start connecting to the server
        /// </summary>
        public void ConnectToServer()
        {
            // Setup variables
            string networkAddress = serverAddress.text;
            IPAddress parsedAddress;
            ushort networkPort;
            // Parse parameters
            bool validIP = IPAddress.TryParse(networkAddress, out parsedAddress);
            bool validPort = ushort.TryParse(serverPort.text, out networkPort);

            // Verify parameters
            // TODO: write verify code
            if (!validIP || !validPort)
            {
                UnityEngine.Debug.Log($"Failed to parse IP Address {serverAddress.text} and Port {serverPort.text}");
            }
            else
            {
                // Assign connection parameters
                PropHunt.Game.ProphuntClientServerControlSystem.NetworkAddress = networkAddress;
                PropHunt.Game.ProphuntClientServerControlSystem.NetworkPort = networkPort;

                ConnectionSystem.Instance.RequestConnect();
            }
        }
    }
}