using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using PropHunt.Mixed.Components;
using Unity.Physics;
using Unity.Transforms;
using Unity.Rendering;

public struct TestCharacterGhostSerializer : IGhostSerializer<TestCharacterSnapshotData>
{
    private ComponentType componentTypePlayerId;
    private ComponentType componentTypePlayerMovement;
    private ComponentType componentTypePlayerView;
    private ComponentType componentTypePhysicsCollider;
    private ComponentType componentTypePhysicsGravityFactor;
    private ComponentType componentTypePhysicsMass;
    private ComponentType componentTypePhysicsVelocity;
    private ComponentType componentTypeLocalToWorld;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    private ComponentType componentTypeLinkedEntityGroup;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PlayerId> ghostPlayerIdType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PlayerMovement> ghostPlayerMovementType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PlayerView> ghostPlayerViewType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Rotation> ghostRotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Translation> ghostTranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkBufferType<LinkedEntityGroup> ghostLinkedEntityGroupType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild0RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild0TranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Rotation> ghostChild1RotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentDataFromEntity<Translation> ghostChild1TranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<TestCharacterSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypePlayerId = ComponentType.ReadWrite<PlayerId>();
        componentTypePlayerMovement = ComponentType.ReadWrite<PlayerMovement>();
        componentTypePlayerView = ComponentType.ReadWrite<PlayerView>();
        componentTypePhysicsCollider = ComponentType.ReadWrite<PhysicsCollider>();
        componentTypePhysicsGravityFactor = ComponentType.ReadWrite<PhysicsGravityFactor>();
        componentTypePhysicsMass = ComponentType.ReadWrite<PhysicsMass>();
        componentTypePhysicsVelocity = ComponentType.ReadWrite<PhysicsVelocity>();
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        componentTypeLinkedEntityGroup = ComponentType.ReadWrite<LinkedEntityGroup>();
        ghostPlayerIdType = system.GetArchetypeChunkComponentType<PlayerId>(true);
        ghostPlayerMovementType = system.GetArchetypeChunkComponentType<PlayerMovement>(true);
        ghostPlayerViewType = system.GetArchetypeChunkComponentType<PlayerView>(true);
        ghostRotationType = system.GetArchetypeChunkComponentType<Rotation>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
        ghostLinkedEntityGroupType = system.GetArchetypeChunkBufferType<LinkedEntityGroup>(true);
        ghostChild0RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild0TranslationType = system.GetComponentDataFromEntity<Translation>(true);
        ghostChild1RotationType = system.GetComponentDataFromEntity<Rotation>(true);
        ghostChild1TranslationType = system.GetComponentDataFromEntity<Translation>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref TestCharacterSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataPlayerId = chunk.GetNativeArray(ghostPlayerIdType);
        var chunkDataPlayerMovement = chunk.GetNativeArray(ghostPlayerMovementType);
        var chunkDataPlayerView = chunk.GetNativeArray(ghostPlayerViewType);
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        var chunkDataLinkedEntityGroup = chunk.GetBufferAccessor(ghostLinkedEntityGroupType);
        snapshot.SetPlayerIdplayerId(chunkDataPlayerId[ent].playerId, serializerState);
        snapshot.SetPlayerMovementmoveSpeed(chunkDataPlayerMovement[ent].moveSpeed, serializerState);
        snapshot.SetPlayerMovementsprintMultiplier(chunkDataPlayerMovement[ent].sprintMultiplier, serializerState);
        snapshot.SetPlayerMovementviewRotationRate(chunkDataPlayerMovement[ent].viewRotationRate, serializerState);
        snapshot.SetPlayerMovementvelocity(chunkDataPlayerMovement[ent].velocity, serializerState);
        snapshot.SetPlayerMovementjumpForce(chunkDataPlayerMovement[ent].jumpForce, serializerState);
        snapshot.SetPlayerMovementmaxWalkAngle(chunkDataPlayerMovement[ent].maxWalkAngle, serializerState);
        snapshot.SetPlayerMovementgroundCheckDistance(chunkDataPlayerMovement[ent].groundCheckDistance, serializerState);
        snapshot.SetPlayerMovementgravityForce(chunkDataPlayerMovement[ent].gravityForce, serializerState);
        snapshot.SetPlayerViewpitch(chunkDataPlayerView[ent].pitch, serializerState);
        snapshot.SetPlayerViewyaw(chunkDataPlayerView[ent].yaw, serializerState);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
        snapshot.SetChild0RotationValue(ghostChild0RotationType[chunkDataLinkedEntityGroup[ent][1].Value].Value, serializerState);
        snapshot.SetChild0TranslationValue(ghostChild0TranslationType[chunkDataLinkedEntityGroup[ent][1].Value].Value, serializerState);
        snapshot.SetChild1RotationValue(ghostChild1RotationType[chunkDataLinkedEntityGroup[ent][2].Value].Value, serializerState);
        snapshot.SetChild1TranslationValue(ghostChild1TranslationType[chunkDataLinkedEntityGroup[ent][2].Value].Value, serializerState);
    }
}
