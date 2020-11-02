using UnityEngine;
using PropHunt.Client.Systems;

namespace PropHunt.UI
{
    /// <summary>
    /// Action to handle disconnecting from the server
    /// </summary>
    public class DisconnectAction : MonoBehaviour
    {

        /// <summary>
        /// Action to disconnect the client from the server.
        /// </summary>
        public void DisconnectClient()
        {
            ConnectionSystem.RequestDisconnect = true;
        }
    }
}