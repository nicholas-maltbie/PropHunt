using UnityEngine;

namespace PropHunt.UI
{
    /// <summary>
    /// Simple class to abstract commands to change UI for a menu screen
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        /// <summary>
        /// Request a new screen using a prefab name
        /// </summary>
        /// <param name="screenPrefab">Screen prefab to switch to</param>
        public void SetScreen(GameObject screenPrefab)
        {
            this.SetScreen(screenPrefab.name);
        }

        /// <summary>
        /// Request a new screen directly through a name
        /// </summary>
        /// <param name="name">Name of new screen to display</param>
        public void SetScreen(string name)
        {
            UIManager.RequestNewScreen(this, name);
        }
    }
}