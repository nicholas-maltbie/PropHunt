
using Unity.Entities;
using Unity.NetCode;

[GhostDefaultComponent(GhostDefaultComponentAttribute.Type.Server|GhostDefaultComponentAttribute.Type.PredictedClient)]
public struct PlayerView : IComponentData
{
    [GhostDefaultField(100, true)]
    public float pitch;
    
    [GhostDefaultField(100, true)]
    public float yaw;
}
