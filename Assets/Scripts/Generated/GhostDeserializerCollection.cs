using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct ProphuntGhostDeserializerCollection : IGhostDeserializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new string[]
        {
        };
        return arr;
    }

    public int Length => 0;
#endif
    public void Initialize(World world)
    {
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    public void Spawn(int serializer, int ghostId, uint snapshot, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

}
public struct EnableProphuntGhostReceiveSystemComponent : IComponentData
{}
public class ProphuntGhostReceiveSystem : GhostReceiveSystem<ProphuntGhostDeserializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableProphuntGhostReceiveSystemComponent>();
    }
}
