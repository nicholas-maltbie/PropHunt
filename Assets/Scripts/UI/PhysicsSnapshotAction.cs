using UnityEngine;
using PropHunt.Game;
using System;

namespace PropHunt.UI
{
    /// <summary>
    /// Action to connect client to server
    /// </summary>
    public class PhysicsSnapshotAction : MonoBehaviour
    {
        /// <summary>
        /// Command to start connecting to the server
        /// </summary>
        public void TakeSnapshot()
        {
            try
            {
                PhysicsSnapshotUtil.Instance.TakeSnapshot();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log($"Failed to take snapshot: {ex}");
            }
        }
    }
}