using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

public struct CubeGhostSerializer : IGhostSerializer<CubeSnapshotData>
{
    private ComponentType componentTypeMovableCubeComponent;
    private ComponentType componentTypePlayerView;
    private ComponentType componentTypePhysicsCollider;
    private ComponentType componentTypeLocalToWorld;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<MovableCubeComponent> ghostMovableCubeComponentType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PlayerView> ghostPlayerViewType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Rotation> ghostRotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Translation> ghostTranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<CubeSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeMovableCubeComponent = ComponentType.ReadWrite<MovableCubeComponent>();
        componentTypePlayerView = ComponentType.ReadWrite<PlayerView>();
        componentTypePhysicsCollider = ComponentType.ReadWrite<PhysicsCollider>();
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        ghostMovableCubeComponentType = system.GetArchetypeChunkComponentType<MovableCubeComponent>(true);
        ghostPlayerViewType = system.GetArchetypeChunkComponentType<PlayerView>(true);
        ghostRotationType = system.GetArchetypeChunkComponentType<Rotation>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref CubeSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataMovableCubeComponent = chunk.GetNativeArray(ghostMovableCubeComponentType);
        var chunkDataPlayerView = chunk.GetNativeArray(ghostPlayerViewType);
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        snapshot.SetMovableCubeComponentPlayerId(chunkDataMovableCubeComponent[ent].PlayerId, serializerState);
        snapshot.SetPlayerViewpitch(chunkDataPlayerView[ent].pitch, serializerState);
        snapshot.SetPlayerViewyaw(chunkDataPlayerView[ent].yaw, serializerState);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
    }
}
