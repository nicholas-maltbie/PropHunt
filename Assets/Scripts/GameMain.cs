using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;
#if UNITY_EDITOR
using Unity.NetCode.Editor;
#endif

namespace PropHunt.Game
{

/// <summary>
/// Prophunt Client Server Control System, operates the client and server
/// for network communications setup and disconnect.
/// </summary>
#if !UNITY_CLIENT || UNITY_SERVER || UNITY_EDITOR
[UpdateBefore(typeof(TickServerSimulationSystem))]
#endif
#if !UNITY_SERVER
[UpdateBefore(typeof(TickClientSimulationSystem))]
#endif
[UpdateInWorld(UpdateInWorld.TargetWorld.Default)]
public class ProphuntClientServerControlSystem : ComponentSystem
{
    private const ushort networkPort = 25623;

    /// <summary>
    /// Setup struct for initializing server
    /// </summary>
    private struct InitializeClientServer : IComponentData
    {
    }

    /// <summary>
    /// Invoked when the object is created
    /// </summary>
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<InitializeClientServer>();
        var initEntity = EntityManager.CreateEntity(typeof(InitializeClientServer));
    }

    /// <summary>
    /// Updated every frame, but only do server setup first update after
    /// having been initialized.
    /// </summary>
    protected override void OnUpdate()
    {
        // Destroy initialize component so only update once. 
        EntityManager.DestroyEntity(GetSingletonEntity<InitializeClientServer>());
        foreach (var world in World.All)
        {
#if !UNITY_CLIENT || UNITY_SERVER || UNITY_EDITOR
            // Bind the server and start listening for connections
            if (world.GetExistingSystem<ServerSimulationSystemGroup>() != null)
            {
                NetworkEndPoint ep = NetworkEndPoint.AnyIpv4;
                ep.Port = networkPort;
                world.GetExistingSystem<NetworkStreamReceiveSystem>().Listen(ep);
            }
#endif
#if !UNITY_SERVER
            // Auto connect all clients to the server
            if (world.GetExistingSystem<ClientSimulationSystemGroup>() != null)
            {
                // Enable fixed tick rate
                world.EntityManager.CreateEntity(typeof(FixedClientTickRate));
                NetworkEndPoint ep = NetworkEndPoint.LoopbackIpv4;
                ep.Port = networkPort;
#if UNITY_EDITOR
                ep = NetworkEndPoint.Parse(ClientServerBootstrap.RequestedAutoConnect, networkPort);
#endif
                world.GetExistingSystem<NetworkStreamReceiveSystem>().Connect(ep);
            }
#endif
        }
    }
}

/// <summary>
/// This is the game main MonoBehaviour. This can be used to configure global
/// settings and how the project operates.
/// </summary>
public class GameMain : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    /// <summary>
    /// This is the setup, can add attributes or set global settings from here.
    /// Not configurable settings as of yet.
    /// </summary>
    /// <param name="entity">Entity to add attributes to (new entity being created).</param>
    /// <param name="dstManager">Entity manager to configure global settings</param>
    /// <param name="conversionSystem">Conversion settings from game object to entity</param>
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
#if !UNITY_CLIENT || UNITY_SERVER || UNITY_EDITOR
        // Setup server data settings here. 
#endif
    }
}

} // End namespace Prophunt.Game
