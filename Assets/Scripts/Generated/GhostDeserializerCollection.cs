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
            "TestCharacterGhostSerializer",
        };
        return arr;
    }

    public int Length => 1;
#endif
    public void Initialize(World world)
    {
        var curTestCharacterGhostSpawnSystem = world.GetOrCreateSystem<TestCharacterGhostSpawnSystem>();
        m_TestCharacterSnapshotDataNewGhostIds = curTestCharacterGhostSpawnSystem.NewGhostIds;
        m_TestCharacterSnapshotDataNewGhosts = curTestCharacterGhostSpawnSystem.NewGhosts;
        curTestCharacterGhostSpawnSystem.GhostType = 0;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_TestCharacterSnapshotDataFromEntity = system.GetBufferFromEntity<TestCharacterSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<ProphuntGhostDeserializerCollection>.InvokeDeserialize(m_TestCharacterSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    public void Spawn(int serializer, int ghostId, uint snapshot, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                m_TestCharacterSnapshotDataNewGhostIds.Add(ghostId);
                m_TestCharacterSnapshotDataNewGhosts.Add(GhostReceiveSystem<ProphuntGhostDeserializerCollection>.InvokeSpawn<TestCharacterSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<TestCharacterSnapshotData> m_TestCharacterSnapshotDataFromEntity;
    private NativeList<int> m_TestCharacterSnapshotDataNewGhostIds;
    private NativeList<TestCharacterSnapshotData> m_TestCharacterSnapshotDataNewGhosts;
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
