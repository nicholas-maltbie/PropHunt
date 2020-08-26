using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct ProphuntGhostSerializerCollection : IGhostSerializerCollection
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
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(CubeSnapshotData))
            return 0;
        if (typeof(T) == typeof(TestCharacterSnapshotData))
            return 1;
        if (typeof(T) == typeof(SpinningCylinderSnapshotData))
            return 2;
        if (typeof(T) == typeof(HorizontalSpinningCylinderSnapshotData))
            return 3;
        if (typeof(T) == typeof(ExampleMovingPlatformSnapshotData))
            return 4;
        return -1;
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_CubeGhostSerializer.BeginSerialize(system);
        m_TestCharacterGhostSerializer.BeginSerialize(system);
        m_SpinningCylinderGhostSerializer.BeginSerialize(system);
        m_HorizontalSpinningCylinderGhostSerializer.BeginSerialize(system);
        m_ExampleMovingPlatformGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_CubeGhostSerializer.CalculateImportance(chunk);
            case 1:
                return m_TestCharacterGhostSerializer.CalculateImportance(chunk);
            case 2:
                return m_SpinningCylinderGhostSerializer.CalculateImportance(chunk);
            case 3:
                return m_HorizontalSpinningCylinderGhostSerializer.CalculateImportance(chunk);
            case 4:
                return m_ExampleMovingPlatformGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_CubeGhostSerializer.SnapshotSize;
            case 1:
                return m_TestCharacterGhostSerializer.SnapshotSize;
            case 2:
                return m_SpinningCylinderGhostSerializer.SnapshotSize;
            case 3:
                return m_HorizontalSpinningCylinderGhostSerializer.SnapshotSize;
            case 4:
                return m_ExampleMovingPlatformGhostSerializer.SnapshotSize;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int Serialize(ref DataStreamWriter dataStream, SerializeData data)
    {
        switch (data.ghostType)
        {
            case 0:
            {
                return GhostSendSystem<ProphuntGhostSerializerCollection>.InvokeSerialize<CubeGhostSerializer, CubeSnapshotData>(m_CubeGhostSerializer, ref dataStream, data);
            }
            case 1:
            {
                return GhostSendSystem<ProphuntGhostSerializerCollection>.InvokeSerialize<TestCharacterGhostSerializer, TestCharacterSnapshotData>(m_TestCharacterGhostSerializer, ref dataStream, data);
            }
            case 2:
            {
                return GhostSendSystem<ProphuntGhostSerializerCollection>.InvokeSerialize<SpinningCylinderGhostSerializer, SpinningCylinderSnapshotData>(m_SpinningCylinderGhostSerializer, ref dataStream, data);
            }
            case 3:
            {
                return GhostSendSystem<ProphuntGhostSerializerCollection>.InvokeSerialize<HorizontalSpinningCylinderGhostSerializer, HorizontalSpinningCylinderSnapshotData>(m_HorizontalSpinningCylinderGhostSerializer, ref dataStream, data);
            }
            case 4:
            {
                return GhostSendSystem<ProphuntGhostSerializerCollection>.InvokeSerialize<ExampleMovingPlatformGhostSerializer, ExampleMovingPlatformSnapshotData>(m_ExampleMovingPlatformGhostSerializer, ref dataStream, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private CubeGhostSerializer m_CubeGhostSerializer;
    private TestCharacterGhostSerializer m_TestCharacterGhostSerializer;
    private SpinningCylinderGhostSerializer m_SpinningCylinderGhostSerializer;
    private HorizontalSpinningCylinderGhostSerializer m_HorizontalSpinningCylinderGhostSerializer;
    private ExampleMovingPlatformGhostSerializer m_ExampleMovingPlatformGhostSerializer;
}

public struct EnableProphuntGhostSendSystemComponent : IComponentData
{}
public class ProphuntGhostSendSystem : GhostSendSystem<ProphuntGhostSerializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableProphuntGhostSendSystemComponent>();
    }

    public override bool IsEnabled()
    {
        return HasSingleton<EnableProphuntGhostSendSystemComponent>();
    }
}
