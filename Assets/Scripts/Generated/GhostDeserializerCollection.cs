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
            "CubeGhostSerializer",
            "TestCharacterGhostSerializer",
        };
        return arr;
    }

    public int Length => 2;
#endif
    public void Initialize(World world)
    {
        var curCubeGhostSpawnSystem = world.GetOrCreateSystem<CubeGhostSpawnSystem>();
        m_CubeSnapshotDataNewGhostIds = curCubeGhostSpawnSystem.NewGhostIds;
        m_CubeSnapshotDataNewGhosts = curCubeGhostSpawnSystem.NewGhosts;
        curCubeGhostSpawnSystem.GhostType = 0;
        var curTestCharacterGhostSpawnSystem = world.GetOrCreateSystem<TestCharacterGhostSpawnSystem>();
        m_TestCharacterSnapshotDataNewGhostIds = curTestCharacterGhostSpawnSystem.NewGhostIds;
        m_TestCharacterSnapshotDataNewGhosts = curTestCharacterGhostSpawnSystem.NewGhosts;
        curTestCharacterGhostSpawnSystem.GhostType = 1;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_CubeSnapshotDataFromEntity = system.GetBufferFromEntity<CubeSnapshotData>();
        m_TestCharacterSnapshotDataFromEntity = system.GetBufferFromEntity<TestCharacterSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<ProphuntGhostDeserializerCollection>.InvokeDeserialize(m_CubeSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 1:
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
                m_CubeSnapshotDataNewGhostIds.Add(ghostId);
                m_CubeSnapshotDataNewGhosts.Add(GhostReceiveSystem<ProphuntGhostDeserializerCollection>.InvokeSpawn<CubeSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 1:
                m_TestCharacterSnapshotDataNewGhostIds.Add(ghostId);
                m_TestCharacterSnapshotDataNewGhosts.Add(GhostReceiveSystem<ProphuntGhostDeserializerCollection>.InvokeSpawn<TestCharacterSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<CubeSnapshotData> m_CubeSnapshotDataFromEntity;
    private NativeList<int> m_CubeSnapshotDataNewGhostIds;
    private NativeList<CubeSnapshotData> m_CubeSnapshotDataNewGhosts;
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
