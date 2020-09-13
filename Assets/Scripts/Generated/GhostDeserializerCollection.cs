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
            "SpinningCylinderGhostSerializer",
            "HorizontalSpinningCylinderGhostSerializer",
            "ExampleMovingPlatformGhostSerializer",
        };
        return arr;
    }

    public int Length => 5;
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
        var curSpinningCylinderGhostSpawnSystem = world.GetOrCreateSystem<SpinningCylinderGhostSpawnSystem>();
        m_SpinningCylinderSnapshotDataNewGhostIds = curSpinningCylinderGhostSpawnSystem.NewGhostIds;
        m_SpinningCylinderSnapshotDataNewGhosts = curSpinningCylinderGhostSpawnSystem.NewGhosts;
        curSpinningCylinderGhostSpawnSystem.GhostType = 2;
        var curHorizontalSpinningCylinderGhostSpawnSystem = world.GetOrCreateSystem<HorizontalSpinningCylinderGhostSpawnSystem>();
        m_HorizontalSpinningCylinderSnapshotDataNewGhostIds = curHorizontalSpinningCylinderGhostSpawnSystem.NewGhostIds;
        m_HorizontalSpinningCylinderSnapshotDataNewGhosts = curHorizontalSpinningCylinderGhostSpawnSystem.NewGhosts;
        curHorizontalSpinningCylinderGhostSpawnSystem.GhostType = 3;
        var curExampleMovingPlatformGhostSpawnSystem = world.GetOrCreateSystem<ExampleMovingPlatformGhostSpawnSystem>();
        m_ExampleMovingPlatformSnapshotDataNewGhostIds = curExampleMovingPlatformGhostSpawnSystem.NewGhostIds;
        m_ExampleMovingPlatformSnapshotDataNewGhosts = curExampleMovingPlatformGhostSpawnSystem.NewGhosts;
        curExampleMovingPlatformGhostSpawnSystem.GhostType = 4;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_CubeSnapshotDataFromEntity = system.GetBufferFromEntity<CubeSnapshotData>();
        m_TestCharacterSnapshotDataFromEntity = system.GetBufferFromEntity<TestCharacterSnapshotData>();
        m_SpinningCylinderSnapshotDataFromEntity = system.GetBufferFromEntity<SpinningCylinderSnapshotData>();
        m_HorizontalSpinningCylinderSnapshotDataFromEntity = system.GetBufferFromEntity<HorizontalSpinningCylinderSnapshotData>();
        m_ExampleMovingPlatformSnapshotDataFromEntity = system.GetBufferFromEntity<ExampleMovingPlatformSnapshotData>();
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
            case 2:
                return GhostReceiveSystem<ProphuntGhostDeserializerCollection>.InvokeDeserialize(m_SpinningCylinderSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 3:
                return GhostReceiveSystem<ProphuntGhostDeserializerCollection>.InvokeDeserialize(m_HorizontalSpinningCylinderSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 4:
                return GhostReceiveSystem<ProphuntGhostDeserializerCollection>.InvokeDeserialize(m_ExampleMovingPlatformSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
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
            case 2:
                m_SpinningCylinderSnapshotDataNewGhostIds.Add(ghostId);
                m_SpinningCylinderSnapshotDataNewGhosts.Add(GhostReceiveSystem<ProphuntGhostDeserializerCollection>.InvokeSpawn<SpinningCylinderSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 3:
                m_HorizontalSpinningCylinderSnapshotDataNewGhostIds.Add(ghostId);
                m_HorizontalSpinningCylinderSnapshotDataNewGhosts.Add(GhostReceiveSystem<ProphuntGhostDeserializerCollection>.InvokeSpawn<HorizontalSpinningCylinderSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 4:
                m_ExampleMovingPlatformSnapshotDataNewGhostIds.Add(ghostId);
                m_ExampleMovingPlatformSnapshotDataNewGhosts.Add(GhostReceiveSystem<ProphuntGhostDeserializerCollection>.InvokeSpawn<ExampleMovingPlatformSnapshotData>(snapshot, ref reader, compressionModel));
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
    private BufferFromEntity<SpinningCylinderSnapshotData> m_SpinningCylinderSnapshotDataFromEntity;
    private NativeList<int> m_SpinningCylinderSnapshotDataNewGhostIds;
    private NativeList<SpinningCylinderSnapshotData> m_SpinningCylinderSnapshotDataNewGhosts;
    private BufferFromEntity<HorizontalSpinningCylinderSnapshotData> m_HorizontalSpinningCylinderSnapshotDataFromEntity;
    private NativeList<int> m_HorizontalSpinningCylinderSnapshotDataNewGhostIds;
    private NativeList<HorizontalSpinningCylinderSnapshotData> m_HorizontalSpinningCylinderSnapshotDataNewGhosts;
    private BufferFromEntity<ExampleMovingPlatformSnapshotData> m_ExampleMovingPlatformSnapshotDataFromEntity;
    private NativeList<int> m_ExampleMovingPlatformSnapshotDataNewGhostIds;
    private NativeList<ExampleMovingPlatformSnapshotData> m_ExampleMovingPlatformSnapshotDataNewGhosts;
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
