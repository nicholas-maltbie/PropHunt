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
    private ComponentType componentTypeKCCGravity;
    private ComponentType componentTypeKCCGrounded;
    private ComponentType componentTypeKCCJumping;
    private ComponentType componentTypeKCCMovementSettings;
    private ComponentType componentTypeKCCVelocity;
    private ComponentType componentTypePlayerId;
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
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<KCCGravity> ghostKCCGravityType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<KCCGrounded> ghostKCCGroundedType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<KCCJumping> ghostKCCJumpingType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<KCCMovementSettings> ghostKCCMovementSettingsType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<KCCVelocity> ghostKCCVelocityType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PlayerId> ghostPlayerIdType;
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
        componentTypeKCCGravity = ComponentType.ReadWrite<KCCGravity>();
        componentTypeKCCGrounded = ComponentType.ReadWrite<KCCGrounded>();
        componentTypeKCCJumping = ComponentType.ReadWrite<KCCJumping>();
        componentTypeKCCMovementSettings = ComponentType.ReadWrite<KCCMovementSettings>();
        componentTypeKCCVelocity = ComponentType.ReadWrite<KCCVelocity>();
        componentTypePlayerId = ComponentType.ReadWrite<PlayerId>();
        componentTypePlayerView = ComponentType.ReadWrite<PlayerView>();
        componentTypePhysicsCollider = ComponentType.ReadWrite<PhysicsCollider>();
        componentTypePhysicsGravityFactor = ComponentType.ReadWrite<PhysicsGravityFactor>();
        componentTypePhysicsMass = ComponentType.ReadWrite<PhysicsMass>();
        componentTypePhysicsVelocity = ComponentType.ReadWrite<PhysicsVelocity>();
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        componentTypeLinkedEntityGroup = ComponentType.ReadWrite<LinkedEntityGroup>();
        ghostKCCGravityType = system.GetArchetypeChunkComponentType<KCCGravity>(true);
        ghostKCCGroundedType = system.GetArchetypeChunkComponentType<KCCGrounded>(true);
        ghostKCCJumpingType = system.GetArchetypeChunkComponentType<KCCJumping>(true);
        ghostKCCMovementSettingsType = system.GetArchetypeChunkComponentType<KCCMovementSettings>(true);
        ghostKCCVelocityType = system.GetArchetypeChunkComponentType<KCCVelocity>(true);
        ghostPlayerIdType = system.GetArchetypeChunkComponentType<PlayerId>(true);
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
        var chunkDataKCCGravity = chunk.GetNativeArray(ghostKCCGravityType);
        var chunkDataKCCGrounded = chunk.GetNativeArray(ghostKCCGroundedType);
        var chunkDataKCCJumping = chunk.GetNativeArray(ghostKCCJumpingType);
        var chunkDataKCCMovementSettings = chunk.GetNativeArray(ghostKCCMovementSettingsType);
        var chunkDataKCCVelocity = chunk.GetNativeArray(ghostKCCVelocityType);
        var chunkDataPlayerId = chunk.GetNativeArray(ghostPlayerIdType);
        var chunkDataPlayerView = chunk.GetNativeArray(ghostPlayerViewType);
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        var chunkDataLinkedEntityGroup = chunk.GetBufferAccessor(ghostLinkedEntityGroupType);
        snapshot.SetKCCGravitygravityAcceleration(chunkDataKCCGravity[ent].gravityAcceleration, serializerState);
        snapshot.SetKCCGroundedmaxWalkAngle(chunkDataKCCGrounded[ent].maxWalkAngle, serializerState);
        snapshot.SetKCCGroundedgroundCheckDistance(chunkDataKCCGrounded[ent].groundCheckDistance, serializerState);
        snapshot.SetKCCJumpingjumpForce(chunkDataKCCJumping[ent].jumpForce, serializerState);
        snapshot.SetKCCJumpingattemptingJump(chunkDataKCCJumping[ent].attemptingJump, serializerState);
        snapshot.SetKCCMovementSettingsmoveSpeed(chunkDataKCCMovementSettings[ent].moveSpeed, serializerState);
        snapshot.SetKCCMovementSettingssprintMultiplier(chunkDataKCCMovementSettings[ent].sprintMultiplier, serializerState);
        snapshot.SetKCCMovementSettingsmoveMaxBounces(chunkDataKCCMovementSettings[ent].moveMaxBounces, serializerState);
        snapshot.SetKCCMovementSettingsmoveAnglePower(chunkDataKCCMovementSettings[ent].moveAnglePower, serializerState);
        snapshot.SetKCCMovementSettingsmovePushPower(chunkDataKCCMovementSettings[ent].movePushPower, serializerState);
        snapshot.SetKCCMovementSettingsmovePushDecay(chunkDataKCCMovementSettings[ent].movePushDecay, serializerState);
        snapshot.SetKCCMovementSettingsfallMaxBounces(chunkDataKCCMovementSettings[ent].fallMaxBounces, serializerState);
        snapshot.SetKCCMovementSettingsfallPushPower(chunkDataKCCMovementSettings[ent].fallPushPower, serializerState);
        snapshot.SetKCCMovementSettingsfallAnglePower(chunkDataKCCMovementSettings[ent].fallAnglePower, serializerState);
        snapshot.SetKCCMovementSettingsfallPushDecay(chunkDataKCCMovementSettings[ent].fallPushDecay, serializerState);
        snapshot.SetKCCVelocityplayerVelocity(chunkDataKCCVelocity[ent].playerVelocity, serializerState);
        snapshot.SetKCCVelocityworldVelocity(chunkDataKCCVelocity[ent].worldVelocity, serializerState);
        snapshot.SetPlayerIdplayerId(chunkDataPlayerId[ent].playerId, serializerState);
        snapshot.SetPlayerViewviewRotationRate(chunkDataPlayerView[ent].viewRotationRate, serializerState);
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
