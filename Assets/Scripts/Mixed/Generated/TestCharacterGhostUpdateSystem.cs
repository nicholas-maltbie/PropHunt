using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Networking.Transport.Utilities;
using Unity.NetCode;
using Unity.Entities;
using PropHunt.Mixed.Components;
using Unity.Transforms;

[UpdateInGroup(typeof(GhostUpdateSystemGroup))]
public class TestCharacterGhostUpdateSystem : JobComponentSystem
{
    [BurstCompile]
    struct UpdateInterpolatedJob : IJobChunk
    {
        [ReadOnly] public NativeHashMap<int, GhostEntity> GhostMap;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [NativeDisableContainerSafetyRestriction] public NativeArray<uint> minMaxSnapshotTick;
#pragma warning disable 649
        [NativeSetThreadIndex]
        public int ThreadIndex;
#pragma warning restore 649
#endif
        [ReadOnly] public ArchetypeChunkBufferType<TestCharacterSnapshotData> ghostSnapshotDataType;
        [ReadOnly] public ArchetypeChunkEntityType ghostEntityType;
        public ArchetypeChunkComponentType<KCCGravity> ghostKCCGravityType;
        public ArchetypeChunkComponentType<KCCGrounded> ghostKCCGroundedType;
        public ArchetypeChunkComponentType<KCCJumping> ghostKCCJumpingType;
        public ArchetypeChunkComponentType<KCCMovementSettings> ghostKCCMovementSettingsType;
        public ArchetypeChunkComponentType<PlayerId> ghostPlayerIdType;
        [ReadOnly] public ArchetypeChunkBufferType<LinkedEntityGroup> ghostLinkedEntityGroupType;
        [NativeDisableParallelForRestriction] public ComponentDataFromEntity<Rotation> ghostRotationFromEntity;
        [NativeDisableParallelForRestriction] public ComponentDataFromEntity<Translation> ghostTranslationFromEntity;

        public uint targetTick;
        public float targetTickFraction;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var deserializerState = new GhostDeserializerState
            {
                GhostMap = GhostMap
            };
            var ghostEntityArray = chunk.GetNativeArray(ghostEntityType);
            var ghostSnapshotDataArray = chunk.GetBufferAccessor(ghostSnapshotDataType);
            var ghostKCCGravityArray = chunk.GetNativeArray(ghostKCCGravityType);
            var ghostKCCGroundedArray = chunk.GetNativeArray(ghostKCCGroundedType);
            var ghostKCCJumpingArray = chunk.GetNativeArray(ghostKCCJumpingType);
            var ghostKCCMovementSettingsArray = chunk.GetNativeArray(ghostKCCMovementSettingsType);
            var ghostPlayerIdArray = chunk.GetNativeArray(ghostPlayerIdType);
            var ghostLinkedEntityGroupArray = chunk.GetBufferAccessor(ghostLinkedEntityGroupType);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var minMaxOffset = ThreadIndex * (JobsUtility.CacheLineSize/4);
#endif
            for (int entityIndex = 0; entityIndex < ghostEntityArray.Length; ++entityIndex)
            {
                var snapshot = ghostSnapshotDataArray[entityIndex];
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                var latestTick = snapshot.GetLatestTick();
                if (latestTick != 0)
                {
                    if (minMaxSnapshotTick[minMaxOffset] == 0 || SequenceHelpers.IsNewer(minMaxSnapshotTick[minMaxOffset], latestTick))
                        minMaxSnapshotTick[minMaxOffset] = latestTick;
                    if (minMaxSnapshotTick[minMaxOffset + 1] == 0 || SequenceHelpers.IsNewer(latestTick, minMaxSnapshotTick[minMaxOffset + 1]))
                        minMaxSnapshotTick[minMaxOffset + 1] = latestTick;
                }
#endif
                // If there is no data found don't apply anything (would be default state), required for prespawned ghosts
                TestCharacterSnapshotData snapshotData;
                if (!snapshot.GetDataAtTick(targetTick, targetTickFraction, out snapshotData))
                    return;

                var ghostKCCGravity = ghostKCCGravityArray[entityIndex];
                var ghostKCCGrounded = ghostKCCGroundedArray[entityIndex];
                var ghostKCCJumping = ghostKCCJumpingArray[entityIndex];
                var ghostKCCMovementSettings = ghostKCCMovementSettingsArray[entityIndex];
                var ghostPlayerId = ghostPlayerIdArray[entityIndex];
                var ghostRotation = ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][0].Value];
                var ghostTranslation = ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][0].Value];
                var ghostChild0Rotation = ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][1].Value];
                var ghostChild0Translation = ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][1].Value];
                var ghostChild1Rotation = ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][2].Value];
                var ghostChild1Translation = ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][2].Value];
                ghostKCCGravity.gravityAcceleration = snapshotData.GetKCCGravitygravityAcceleration(deserializerState);
                ghostKCCGrounded.maxWalkAngle = snapshotData.GetKCCGroundedmaxWalkAngle(deserializerState);
                ghostKCCGrounded.groundCheckDistance = snapshotData.GetKCCGroundedgroundCheckDistance(deserializerState);
                ghostKCCJumping.jumpForce = snapshotData.GetKCCJumpingjumpForce(deserializerState);
                ghostKCCJumping.attemptingJump = snapshotData.GetKCCJumpingattemptingJump(deserializerState);
                ghostKCCMovementSettings.moveSpeed = snapshotData.GetKCCMovementSettingsmoveSpeed(deserializerState);
                ghostKCCMovementSettings.sprintMultiplier = snapshotData.GetKCCMovementSettingssprintMultiplier(deserializerState);
                ghostKCCMovementSettings.moveMaxBounces = snapshotData.GetKCCMovementSettingsmoveMaxBounces(deserializerState);
                ghostKCCMovementSettings.moveAnglePower = snapshotData.GetKCCMovementSettingsmoveAnglePower(deserializerState);
                ghostKCCMovementSettings.movePushPower = snapshotData.GetKCCMovementSettingsmovePushPower(deserializerState);
                ghostKCCMovementSettings.movePushDecay = snapshotData.GetKCCMovementSettingsmovePushDecay(deserializerState);
                ghostKCCMovementSettings.fallMaxBounces = snapshotData.GetKCCMovementSettingsfallMaxBounces(deserializerState);
                ghostKCCMovementSettings.fallPushPower = snapshotData.GetKCCMovementSettingsfallPushPower(deserializerState);
                ghostKCCMovementSettings.fallAnglePower = snapshotData.GetKCCMovementSettingsfallAnglePower(deserializerState);
                ghostKCCMovementSettings.fallPushDecay = snapshotData.GetKCCMovementSettingsfallPushDecay(deserializerState);
                ghostPlayerId.playerId = snapshotData.GetPlayerIdplayerId(deserializerState);
                ghostRotation.Value = snapshotData.GetRotationValue(deserializerState);
                ghostTranslation.Value = snapshotData.GetTranslationValue(deserializerState);
                ghostChild0Rotation.Value = snapshotData.GetChild0RotationValue(deserializerState);
                ghostChild0Translation.Value = snapshotData.GetChild0TranslationValue(deserializerState);
                ghostChild1Rotation.Value = snapshotData.GetChild1RotationValue(deserializerState);
                ghostChild1Translation.Value = snapshotData.GetChild1TranslationValue(deserializerState);
                ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][0].Value] = ghostRotation;
                ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][0].Value] = ghostTranslation;
                ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][1].Value] = ghostChild0Rotation;
                ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][1].Value] = ghostChild0Translation;
                ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][2].Value] = ghostChild1Rotation;
                ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][2].Value] = ghostChild1Translation;
                ghostKCCGravityArray[entityIndex] = ghostKCCGravity;
                ghostKCCGroundedArray[entityIndex] = ghostKCCGrounded;
                ghostKCCJumpingArray[entityIndex] = ghostKCCJumping;
                ghostKCCMovementSettingsArray[entityIndex] = ghostKCCMovementSettings;
                ghostPlayerIdArray[entityIndex] = ghostPlayerId;
            }
        }
    }
    [BurstCompile]
    struct UpdatePredictedJob : IJobChunk
    {
        [ReadOnly] public NativeHashMap<int, GhostEntity> GhostMap;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [NativeDisableContainerSafetyRestriction] public NativeArray<uint> minMaxSnapshotTick;
#endif
#pragma warning disable 649
        [NativeSetThreadIndex]
        public int ThreadIndex;
#pragma warning restore 649
        [NativeDisableParallelForRestriction] public NativeArray<uint> minPredictedTick;
        [ReadOnly] public ArchetypeChunkBufferType<TestCharacterSnapshotData> ghostSnapshotDataType;
        [ReadOnly] public ArchetypeChunkEntityType ghostEntityType;
        public ArchetypeChunkComponentType<PredictedGhostComponent> predictedGhostComponentType;
        public ArchetypeChunkComponentType<KCCGravity> ghostKCCGravityType;
        public ArchetypeChunkComponentType<KCCGrounded> ghostKCCGroundedType;
        public ArchetypeChunkComponentType<KCCJumping> ghostKCCJumpingType;
        public ArchetypeChunkComponentType<KCCMovementSettings> ghostKCCMovementSettingsType;
        public ArchetypeChunkComponentType<KCCVelocity> ghostKCCVelocityType;
        public ArchetypeChunkComponentType<PlayerId> ghostPlayerIdType;
        public ArchetypeChunkComponentType<PlayerView> ghostPlayerViewType;
        [ReadOnly] public ArchetypeChunkBufferType<LinkedEntityGroup> ghostLinkedEntityGroupType;
        [NativeDisableParallelForRestriction] public ComponentDataFromEntity<Rotation> ghostRotationFromEntity;
        [NativeDisableParallelForRestriction] public ComponentDataFromEntity<Translation> ghostTranslationFromEntity;
        public uint targetTick;
        public uint lastPredictedTick;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var deserializerState = new GhostDeserializerState
            {
                GhostMap = GhostMap
            };
            var ghostEntityArray = chunk.GetNativeArray(ghostEntityType);
            var ghostSnapshotDataArray = chunk.GetBufferAccessor(ghostSnapshotDataType);
            var predictedGhostComponentArray = chunk.GetNativeArray(predictedGhostComponentType);
            var ghostKCCGravityArray = chunk.GetNativeArray(ghostKCCGravityType);
            var ghostKCCGroundedArray = chunk.GetNativeArray(ghostKCCGroundedType);
            var ghostKCCJumpingArray = chunk.GetNativeArray(ghostKCCJumpingType);
            var ghostKCCMovementSettingsArray = chunk.GetNativeArray(ghostKCCMovementSettingsType);
            var ghostKCCVelocityArray = chunk.GetNativeArray(ghostKCCVelocityType);
            var ghostPlayerIdArray = chunk.GetNativeArray(ghostPlayerIdType);
            var ghostPlayerViewArray = chunk.GetNativeArray(ghostPlayerViewType);
            var ghostLinkedEntityGroupArray = chunk.GetBufferAccessor(ghostLinkedEntityGroupType);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var minMaxOffset = ThreadIndex * (JobsUtility.CacheLineSize/4);
#endif
            for (int entityIndex = 0; entityIndex < ghostEntityArray.Length; ++entityIndex)
            {
                var snapshot = ghostSnapshotDataArray[entityIndex];
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                var latestTick = snapshot.GetLatestTick();
                if (latestTick != 0)
                {
                    if (minMaxSnapshotTick[minMaxOffset] == 0 || SequenceHelpers.IsNewer(minMaxSnapshotTick[minMaxOffset], latestTick))
                        minMaxSnapshotTick[minMaxOffset] = latestTick;
                    if (minMaxSnapshotTick[minMaxOffset + 1] == 0 || SequenceHelpers.IsNewer(latestTick, minMaxSnapshotTick[minMaxOffset + 1]))
                        minMaxSnapshotTick[minMaxOffset + 1] = latestTick;
                }
#endif
                TestCharacterSnapshotData snapshotData;
                snapshot.GetDataAtTick(targetTick, out snapshotData);

                var predictedData = predictedGhostComponentArray[entityIndex];
                var lastPredictedTickInst = lastPredictedTick;
                if (lastPredictedTickInst == 0 || predictedData.AppliedTick != snapshotData.Tick)
                    lastPredictedTickInst = snapshotData.Tick;
                else if (!SequenceHelpers.IsNewer(lastPredictedTickInst, snapshotData.Tick))
                    lastPredictedTickInst = snapshotData.Tick;
                if (minPredictedTick[ThreadIndex] == 0 || SequenceHelpers.IsNewer(minPredictedTick[ThreadIndex], lastPredictedTickInst))
                    minPredictedTick[ThreadIndex] = lastPredictedTickInst;
                predictedGhostComponentArray[entityIndex] = new PredictedGhostComponent{AppliedTick = snapshotData.Tick, PredictionStartTick = lastPredictedTickInst};
                if (lastPredictedTickInst != snapshotData.Tick)
                    continue;

                var ghostKCCGravity = ghostKCCGravityArray[entityIndex];
                var ghostKCCGrounded = ghostKCCGroundedArray[entityIndex];
                var ghostKCCJumping = ghostKCCJumpingArray[entityIndex];
                var ghostKCCMovementSettings = ghostKCCMovementSettingsArray[entityIndex];
                var ghostKCCVelocity = ghostKCCVelocityArray[entityIndex];
                var ghostPlayerId = ghostPlayerIdArray[entityIndex];
                var ghostPlayerView = ghostPlayerViewArray[entityIndex];
                var ghostRotation = ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][0].Value];
                var ghostTranslation = ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][0].Value];
                var ghostChild0Rotation = ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][1].Value];
                var ghostChild0Translation = ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][1].Value];
                var ghostChild1Rotation = ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][2].Value];
                var ghostChild1Translation = ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][2].Value];
                ghostKCCGravity.gravityAcceleration = snapshotData.GetKCCGravitygravityAcceleration(deserializerState);
                ghostKCCGrounded.maxWalkAngle = snapshotData.GetKCCGroundedmaxWalkAngle(deserializerState);
                ghostKCCGrounded.groundCheckDistance = snapshotData.GetKCCGroundedgroundCheckDistance(deserializerState);
                ghostKCCJumping.jumpForce = snapshotData.GetKCCJumpingjumpForce(deserializerState);
                ghostKCCJumping.attemptingJump = snapshotData.GetKCCJumpingattemptingJump(deserializerState);
                ghostKCCMovementSettings.moveSpeed = snapshotData.GetKCCMovementSettingsmoveSpeed(deserializerState);
                ghostKCCMovementSettings.sprintMultiplier = snapshotData.GetKCCMovementSettingssprintMultiplier(deserializerState);
                ghostKCCMovementSettings.moveMaxBounces = snapshotData.GetKCCMovementSettingsmoveMaxBounces(deserializerState);
                ghostKCCMovementSettings.moveAnglePower = snapshotData.GetKCCMovementSettingsmoveAnglePower(deserializerState);
                ghostKCCMovementSettings.movePushPower = snapshotData.GetKCCMovementSettingsmovePushPower(deserializerState);
                ghostKCCMovementSettings.movePushDecay = snapshotData.GetKCCMovementSettingsmovePushDecay(deserializerState);
                ghostKCCMovementSettings.fallMaxBounces = snapshotData.GetKCCMovementSettingsfallMaxBounces(deserializerState);
                ghostKCCMovementSettings.fallPushPower = snapshotData.GetKCCMovementSettingsfallPushPower(deserializerState);
                ghostKCCMovementSettings.fallAnglePower = snapshotData.GetKCCMovementSettingsfallAnglePower(deserializerState);
                ghostKCCMovementSettings.fallPushDecay = snapshotData.GetKCCMovementSettingsfallPushDecay(deserializerState);
                ghostKCCVelocity.playerVelocity = snapshotData.GetKCCVelocityplayerVelocity(deserializerState);
                ghostKCCVelocity.worldVelocity = snapshotData.GetKCCVelocityworldVelocity(deserializerState);
                ghostPlayerId.playerId = snapshotData.GetPlayerIdplayerId(deserializerState);
                ghostPlayerView.viewRotationRate = snapshotData.GetPlayerViewviewRotationRate(deserializerState);
                ghostPlayerView.pitch = snapshotData.GetPlayerViewpitch(deserializerState);
                ghostPlayerView.yaw = snapshotData.GetPlayerViewyaw(deserializerState);
                ghostRotation.Value = snapshotData.GetRotationValue(deserializerState);
                ghostTranslation.Value = snapshotData.GetTranslationValue(deserializerState);
                ghostChild0Rotation.Value = snapshotData.GetChild0RotationValue(deserializerState);
                ghostChild0Translation.Value = snapshotData.GetChild0TranslationValue(deserializerState);
                ghostChild1Rotation.Value = snapshotData.GetChild1RotationValue(deserializerState);
                ghostChild1Translation.Value = snapshotData.GetChild1TranslationValue(deserializerState);
                ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][0].Value] = ghostRotation;
                ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][0].Value] = ghostTranslation;
                ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][1].Value] = ghostChild0Rotation;
                ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][1].Value] = ghostChild0Translation;
                ghostRotationFromEntity[ghostLinkedEntityGroupArray[entityIndex][2].Value] = ghostChild1Rotation;
                ghostTranslationFromEntity[ghostLinkedEntityGroupArray[entityIndex][2].Value] = ghostChild1Translation;
                ghostKCCGravityArray[entityIndex] = ghostKCCGravity;
                ghostKCCGroundedArray[entityIndex] = ghostKCCGrounded;
                ghostKCCJumpingArray[entityIndex] = ghostKCCJumping;
                ghostKCCMovementSettingsArray[entityIndex] = ghostKCCMovementSettings;
                ghostKCCVelocityArray[entityIndex] = ghostKCCVelocity;
                ghostPlayerIdArray[entityIndex] = ghostPlayerId;
                ghostPlayerViewArray[entityIndex] = ghostPlayerView;
            }
        }
    }
    private ClientSimulationSystemGroup m_ClientSimulationSystemGroup;
    private GhostPredictionSystemGroup m_GhostPredictionSystemGroup;
    private EntityQuery m_interpolatedQuery;
    private EntityQuery m_predictedQuery;
    private GhostUpdateSystemGroup m_GhostUpdateSystemGroup;
    private uint m_LastPredictedTick;
    protected override void OnCreate()
    {
        m_GhostUpdateSystemGroup = World.GetOrCreateSystem<GhostUpdateSystemGroup>();
        m_ClientSimulationSystemGroup = World.GetOrCreateSystem<ClientSimulationSystemGroup>();
        m_GhostPredictionSystemGroup = World.GetOrCreateSystem<GhostPredictionSystemGroup>();
        m_interpolatedQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new []{
                ComponentType.ReadWrite<TestCharacterSnapshotData>(),
                ComponentType.ReadOnly<GhostComponent>(),
                ComponentType.ReadWrite<KCCGravity>(),
                ComponentType.ReadWrite<KCCGrounded>(),
                ComponentType.ReadWrite<KCCJumping>(),
                ComponentType.ReadWrite<KCCMovementSettings>(),
                ComponentType.ReadWrite<PlayerId>(),
                ComponentType.ReadOnly<LinkedEntityGroup>(),
            },
            None = new []{ComponentType.ReadWrite<PredictedGhostComponent>()}
        });
        m_predictedQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new []{
                ComponentType.ReadOnly<TestCharacterSnapshotData>(),
                ComponentType.ReadOnly<GhostComponent>(),
                ComponentType.ReadOnly<PredictedGhostComponent>(),
                ComponentType.ReadWrite<KCCGravity>(),
                ComponentType.ReadWrite<KCCGrounded>(),
                ComponentType.ReadWrite<KCCJumping>(),
                ComponentType.ReadWrite<KCCMovementSettings>(),
                ComponentType.ReadWrite<KCCVelocity>(),
                ComponentType.ReadWrite<PlayerId>(),
                ComponentType.ReadWrite<PlayerView>(),
                ComponentType.ReadOnly<LinkedEntityGroup>(),
            }
        });
        RequireForUpdate(GetEntityQuery(ComponentType.ReadWrite<TestCharacterSnapshotData>(),
            ComponentType.ReadOnly<GhostComponent>()));
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ghostEntityMap = m_GhostUpdateSystemGroup.GhostEntityMap;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        var ghostMinMaxSnapshotTick = m_GhostUpdateSystemGroup.GhostSnapshotTickMinMax;
#endif
        if (!m_predictedQuery.IsEmptyIgnoreFilter)
        {
            var updatePredictedJob = new UpdatePredictedJob
            {
                GhostMap = ghostEntityMap,
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                minMaxSnapshotTick = ghostMinMaxSnapshotTick,
#endif
                minPredictedTick = m_GhostPredictionSystemGroup.OldestPredictedTick,
                ghostSnapshotDataType = GetArchetypeChunkBufferType<TestCharacterSnapshotData>(true),
                ghostEntityType = GetArchetypeChunkEntityType(),
                predictedGhostComponentType = GetArchetypeChunkComponentType<PredictedGhostComponent>(),
                ghostKCCGravityType = GetArchetypeChunkComponentType<KCCGravity>(),
                ghostKCCGroundedType = GetArchetypeChunkComponentType<KCCGrounded>(),
                ghostKCCJumpingType = GetArchetypeChunkComponentType<KCCJumping>(),
                ghostKCCMovementSettingsType = GetArchetypeChunkComponentType<KCCMovementSettings>(),
                ghostKCCVelocityType = GetArchetypeChunkComponentType<KCCVelocity>(),
                ghostPlayerIdType = GetArchetypeChunkComponentType<PlayerId>(),
                ghostPlayerViewType = GetArchetypeChunkComponentType<PlayerView>(),
                ghostLinkedEntityGroupType = GetArchetypeChunkBufferType<LinkedEntityGroup>(true),
                ghostRotationFromEntity = GetComponentDataFromEntity<Rotation>(),
                ghostTranslationFromEntity = GetComponentDataFromEntity<Translation>(),

                targetTick = m_ClientSimulationSystemGroup.ServerTick,
                lastPredictedTick = m_LastPredictedTick
            };
            m_LastPredictedTick = m_ClientSimulationSystemGroup.ServerTick;
            if (m_ClientSimulationSystemGroup.ServerTickFraction < 1)
                m_LastPredictedTick = 0;
            inputDeps = updatePredictedJob.Schedule(m_predictedQuery, JobHandle.CombineDependencies(inputDeps, m_GhostUpdateSystemGroup.LastGhostMapWriter));
            m_GhostPredictionSystemGroup.AddPredictedTickWriter(inputDeps);
        }
        if (!m_interpolatedQuery.IsEmptyIgnoreFilter)
        {
            var updateInterpolatedJob = new UpdateInterpolatedJob
            {
                GhostMap = ghostEntityMap,
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                minMaxSnapshotTick = ghostMinMaxSnapshotTick,
#endif
                ghostSnapshotDataType = GetArchetypeChunkBufferType<TestCharacterSnapshotData>(true),
                ghostEntityType = GetArchetypeChunkEntityType(),
                ghostKCCGravityType = GetArchetypeChunkComponentType<KCCGravity>(),
                ghostKCCGroundedType = GetArchetypeChunkComponentType<KCCGrounded>(),
                ghostKCCJumpingType = GetArchetypeChunkComponentType<KCCJumping>(),
                ghostKCCMovementSettingsType = GetArchetypeChunkComponentType<KCCMovementSettings>(),
                ghostPlayerIdType = GetArchetypeChunkComponentType<PlayerId>(),
                ghostLinkedEntityGroupType = GetArchetypeChunkBufferType<LinkedEntityGroup>(true),
                ghostRotationFromEntity = GetComponentDataFromEntity<Rotation>(),
                ghostTranslationFromEntity = GetComponentDataFromEntity<Translation>(),
                targetTick = m_ClientSimulationSystemGroup.InterpolationTick,
                targetTickFraction = m_ClientSimulationSystemGroup.InterpolationTickFraction
            };
            inputDeps = updateInterpolatedJob.Schedule(m_interpolatedQuery, JobHandle.CombineDependencies(inputDeps, m_GhostUpdateSystemGroup.LastGhostMapWriter));
        }
        return inputDeps;
    }
}
public partial class TestCharacterGhostSpawnSystem : DefaultGhostSpawnSystem<TestCharacterSnapshotData>
{
    struct SetPredictedDefault : IJobParallelFor
    {
        [ReadOnly] public NativeArray<TestCharacterSnapshotData> snapshots;
        public NativeArray<int> predictionMask;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<NetworkIdComponent> localPlayerId;
        public void Execute(int index)
        {
            if (localPlayerId.Length == 1 && snapshots[index].GetPlayerIdplayerId() == localPlayerId[0].Value)
                predictionMask[index] = 1;
        }
    }
    protected override JobHandle SetPredictedGhostDefaults(NativeArray<TestCharacterSnapshotData> snapshots, NativeArray<int> predictionMask, JobHandle inputDeps)
    {
        JobHandle playerHandle;
        var job = new SetPredictedDefault
        {
            snapshots = snapshots,
            predictionMask = predictionMask,
            localPlayerId = m_PlayerGroup.ToComponentDataArrayAsync<NetworkIdComponent>(Allocator.TempJob, out playerHandle),
        };
        return job.Schedule(predictionMask.Length, 8, JobHandle.CombineDependencies(playerHandle, inputDeps));
    }
}
