//THIS FILE IS AUTOGENERATED BY GHOSTCOMPILER. DON'T MODIFY OR ALTER.
using System;
using AOT;
using Unity.Burst;
using Unity.Networking.Transport;
using Unity.NetCode.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Mathematics;
using PropHunt.Mixed.Components;

namespace PropHunt.Generated
{
    [BurstCompile]
    public struct PropHuntMixedComponentsPlayerIdGhostComponentSerializer
    {
        static GhostComponentSerializer.State GetState()
        {
            // This needs to be lazy initialized because otherwise there is a depenency on the static initialization order which breaks il2cpp builds due to TYpeManager not being initialized yet
            if (!s_StateInitialized)
            {
                s_State = new GhostComponentSerializer.State
                {
                    GhostFieldsHash = 14767913548786401661,
                    ExcludeFromComponentCollectionHash = 0,
                    ComponentType = ComponentType.ReadWrite<PropHunt.Mixed.Components.PlayerId>(),
                    ComponentSize = UnsafeUtility.SizeOf<PropHunt.Mixed.Components.PlayerId>(),
                    SnapshotSize = UnsafeUtility.SizeOf<Snapshot>(),
                    ChangeMaskBits = ChangeMaskBits,
                    SendMask = GhostComponentSerializer.SendMask.Interpolated | GhostComponentSerializer.SendMask.Predicted,
                    SendForChildEntities = 1,
                    CopyToSnapshot =
                        new PortableFunctionPointer<GhostComponentSerializer.CopyToFromSnapshotDelegate>(CopyToSnapshot),
                    CopyFromSnapshot =
                        new PortableFunctionPointer<GhostComponentSerializer.CopyToFromSnapshotDelegate>(CopyFromSnapshot),
                    RestoreFromBackup =
                        new PortableFunctionPointer<GhostComponentSerializer.RestoreFromBackupDelegate>(RestoreFromBackup),
                    PredictDelta = new PortableFunctionPointer<GhostComponentSerializer.PredictDeltaDelegate>(PredictDelta),
                    CalculateChangeMask =
                        new PortableFunctionPointer<GhostComponentSerializer.CalculateChangeMaskDelegate>(
                            CalculateChangeMask),
                    Serialize = new PortableFunctionPointer<GhostComponentSerializer.SerializeDelegate>(Serialize),
                    Deserialize = new PortableFunctionPointer<GhostComponentSerializer.DeserializeDelegate>(Deserialize),
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    ReportPredictionErrors = new PortableFunctionPointer<GhostComponentSerializer.ReportPredictionErrorsDelegate>(ReportPredictionErrors),
                    #endif
                };
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                s_State.NumPredictionErrorNames = GetPredictionErrorNames(ref s_State.PredictionErrorNames);
                #endif
                s_StateInitialized = true;
            }
            return s_State;
        }
        private static bool s_StateInitialized;
        private static GhostComponentSerializer.State s_State;
        public static GhostComponentSerializer.State State => GetState();
        public struct Snapshot
        {
            public int playerId;
        }
        public const int ChangeMaskBits = 1;
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.CopyToFromSnapshotDelegate))]
        private static void CopyToSnapshot(IntPtr stateData, IntPtr snapshotData, int snapshotOffset, int snapshotStride, IntPtr componentData, int componentStride, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData, snapshotOffset + snapshotStride*i);
                ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.PlayerId>(componentData, componentStride*i);
                ref var serializerState = ref GhostComponentSerializer.TypeCast<GhostSerializerState>(stateData, 0);
                snapshot.playerId = (int) component.playerId;
            }
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.CopyToFromSnapshotDelegate))]
        private static void CopyFromSnapshot(IntPtr stateData, IntPtr snapshotData, int snapshotOffset, int snapshotStride, IntPtr componentData, int componentStride, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                ref var snapshotInterpolationData = ref GhostComponentSerializer.TypeCast<SnapshotData.DataAtTick>(snapshotData, snapshotStride*i);
                ref var snapshotBefore = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotInterpolationData.SnapshotBefore, snapshotOffset);
                ref var snapshotAfter = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotInterpolationData.SnapshotAfter, snapshotOffset);
                float snapshotInterpolationFactor = snapshotInterpolationData.InterpolationFactor;
                ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.PlayerId>(componentData, componentStride*i);
                var deserializerState = GhostComponentSerializer.TypeCast<GhostDeserializerState>(stateData, 0);
                deserializerState.SnapshotTick = snapshotInterpolationData.Tick;
                component.playerId = (int) snapshotBefore.playerId;
            }
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.RestoreFromBackupDelegate))]
        private static void RestoreFromBackup(IntPtr componentData, IntPtr backupData)
        {
            ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.PlayerId>(componentData, 0);
            ref var backup = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.PlayerId>(backupData, 0);
            component.playerId = backup.playerId;
        }

        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.PredictDeltaDelegate))]
        private static void PredictDelta(IntPtr snapshotData, IntPtr baseline1Data, IntPtr baseline2Data, ref GhostDeltaPredictor predictor)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline1 = ref GhostComponentSerializer.TypeCast<Snapshot>(baseline1Data);
            ref var baseline2 = ref GhostComponentSerializer.TypeCast<Snapshot>(baseline2Data);
            snapshot.playerId = predictor.PredictInt(snapshot.playerId, baseline1.playerId, baseline2.playerId);
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.CalculateChangeMaskDelegate))]
        private static void CalculateChangeMask(IntPtr snapshotData, IntPtr baselineData, IntPtr bits, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask;
            changeMask = (snapshot.playerId != baseline.playerId) ? 1u : 0;
            GhostComponentSerializer.CopyToChangeMask(bits, changeMask, startOffset, 1);
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.SerializeDelegate))]
        private static void Serialize(IntPtr snapshotData, IntPtr baselineData, ref DataStreamWriter writer, ref NetworkCompressionModel compressionModel, IntPtr changeMaskData, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask = GhostComponentSerializer.CopyFromChangeMask(changeMaskData, startOffset, ChangeMaskBits);
            if ((changeMask & (1 << 0)) != 0)
                writer.WritePackedIntDelta(snapshot.playerId, baseline.playerId, compressionModel);
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.DeserializeDelegate))]
        private static void Deserialize(IntPtr snapshotData, IntPtr baselineData, ref DataStreamReader reader, ref NetworkCompressionModel compressionModel, IntPtr changeMaskData, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask = GhostComponentSerializer.CopyFromChangeMask(changeMaskData, startOffset, ChangeMaskBits);
            if ((changeMask & (1 << 0)) != 0)
                snapshot.playerId = reader.ReadPackedIntDelta(baseline.playerId, compressionModel);
            else
                snapshot.playerId = baseline.playerId;
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.ReportPredictionErrorsDelegate))]
        private static void ReportPredictionErrors(IntPtr componentData, IntPtr backupData, ref UnsafeList<float> errors)
        {
            ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.PlayerId>(componentData, 0);
            ref var backup = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.PlayerId>(backupData, 0);
            int errorIndex = 0;
            errors[errorIndex] = math.max(errors[errorIndex], math.abs(component.playerId - backup.playerId));
            ++errorIndex;
        }
        private static int GetPredictionErrorNames(ref FixedString512 names)
        {
            int nameCount = 0;
            if (nameCount != 0)
                names.Append(new FixedString32(","));
            names.Append(new FixedString64("playerId"));
            ++nameCount;
            return nameCount;
        }
        #endif
    }
}