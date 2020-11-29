using UnityEngine;
using UnityEngine.UI;
using System.Net;
using PropHunt.Client.Systems;
using PropHunt.Constants;
using PropHunt.Game;

namespace PropHunt.UI
{
    /// <summary>
    /// Action to connect client to server
    /// </summary>
    public class ConnectAction : MonoBehaviour
    {
        /// <summary>
        /// Instance for this connect action
        /// </summary>
        public static ConnectAction Instance;

        /// <summary>
        /// Text object with server address
        /// </summary>
        public InputField serverAddress;

        /// <summary>
        /// Text object with server port
        /// </summary>
        public InputField serverPort;

        /// <summary>
        /// Debug information
        /// </summary>
        public Text debugInformation;

        public void OnEnable()
        {
            this.serverAddress.text = ProphuntClientServerControlSystem.DefaultNetworkAddress;
            this.serverPort.text = ProphuntClientServerControlSystem.DefaultNetworkPort.ToString();

            ConnectAction.Instance = this;
        }

        public void SetDebugText(string text)
        {
            this.debugInformation.text = text;
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
                var networkControl = NetworkControlSettingsSystem.Instance;
                networkControl.SetSingleton<NetworkControlSettings>(new NetworkControlSettings
                {
                    NetworkAddress = networkAddress,
                    NetworkPort = networkPort
                });
                ConnectionSystem.Instance.RequestConnect(new NetworkControlSettings
                {
                    NetworkAddress = networkAddress,
                    NetworkPort = networkPort
                });
                
            }
        }
    }
}