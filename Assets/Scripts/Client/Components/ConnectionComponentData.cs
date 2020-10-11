using Unity.Entities;

namespace PropHunt.Client.Components
{
    /// <summary>
    /// Component for identifying a material id of a shared library of materials between the client and the server.
    /// </summary>
    public struct ConnectionComponent : IComponentData
    {
        /// <summary>
        /// Current status of the character
        /// </summary>
        public bool isConnected;

        /// <summary>
        /// Has there been a request to connect to the server
        /// </summary>
        public bool requestConnect;

        /// <summary>
        /// Has there been a request to disconnect from the server
        /// </summary>
        public bool requestDisconnect;

        /// <summary>
        /// Is the character attempting to connect to the server
        /// </summary>
        public bool attemptingConnect;

        /// <summary>
        /// Is the character attempting to disconnect from the server
        /// </summary>
        public bool attemptingDisconnect;
    }
}