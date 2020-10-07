using PropHunt.SceneManagement;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Scenes;

namespace PropHunt.Server.Systems
{
    /// <summary>
    /// When server receives go in game request, go in game and delete request
    /// </summary>
    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
    public class SetupGameSystem : ComponentSystem
    {
        public struct SetupGameScene : IComponentData 
        {
            public FixedString64 defaultSubScene;
        }

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SetupGameScene>();
            Entity setupGame = EntityManager.CreateEntity(typeof(SetupGameScene));
            EntityManager.SetComponentData(setupGame, new SetupGameScene
            {
                defaultSubScene = "TestRoom"
            });
        }

        protected override void OnUpdate()
        {
            SetupGameScene setupGame = GetSingleton<SetupGameScene>();
            PostUpdateCommands.DestroyEntity(GetSingletonEntity<SetupGameScene>());
            World.GetOrCreateSystem<SceneSystem>().LoadSceneAsync(SubSceneReferences.Instance.GetSceneByName("TestRoom").SceneGUID);
        }
    }
}