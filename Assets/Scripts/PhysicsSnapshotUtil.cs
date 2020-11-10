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
            if(Instance != null)
            {
                return;
            }
            Instance = this;
        }

        private EntityManager dstManager;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!takeSnapShot)
            {
                return;
            }
            var system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();

            for(int i = 0; i < World.All.Count; i++)
            {
                var currWorld = World.All[i];
                if (currWorld.Name.Contains("Server"))
                {
                    var physicsWorld = currWorld.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
                    var jsonString = JsonUtility.ToJson(physicsWorld);
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
