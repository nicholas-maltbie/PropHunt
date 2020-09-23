using PropHunt.Client.Systems;
using UnityEngine;

namespace PropHunt.PlayMode.Tests.Utility
{
    /// <summary>
    /// Wait until client is connected or until a time expired
    /// </summary>
    public class WaitForConnected : CustomYieldInstruction
    {
        readonly ConnectionSystem connectionManager;
        readonly float timeout;
        readonly float startTime;
        readonly bool targetState;
        bool timedOut;

        public bool TimedOut => timedOut;

        public override bool keepWaiting {
            get {
                if (Time.realtimeSinceStartup - startTime >= timeout) {
                    timedOut = true;
                }

                return ConnectionSystem.IsConnected != targetState && !timedOut;
            }
        }

        public WaitForConnected(ConnectionSystem connectionManager, bool state = true, float newTimeout = 10)
        {
            this.connectionManager = connectionManager;
            this.targetState = state;
            this.timeout = newTimeout;
            this.startTime = Time.realtimeSinceStartup;
        }
    }
}
