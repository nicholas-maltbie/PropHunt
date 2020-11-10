using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace PropHunt.Game
{
    public class PhysicsSnapshotUtil : MonoBehaviour
    {
        public static PhysicsSnapshotUtil Instance { get; private set; }
        private bool takeSnapShot = false;

        private void Awake()
        {
            if (Instance != null)
            {
                return;
            }
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (!takeSnapShot)
            {
                return;
            }

            for (int i = 0; i < World.All.Count; i++)
            {
                var currWorld = World.All[i];
                if (currWorld.Name.Contains("Server"))
                {
                    // Add a breakpoint here in order to take a look at the object.
                    // Still need to find a way to convert it into json.
                    var physicsWorld = currWorld.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
                    break;
                }
            }

            takeSnapShot = false;
        }

        public void TakeSnapshot()
        {
            takeSnapShot = true;
        }
    }
}