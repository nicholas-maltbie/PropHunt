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
    public struct PropHuntMixedComponentsKCCJumpingGhostComponentSerializer
    {
        static PropHuntMixedComponentsKCCJumpingGhostComponentSerializer()
        {
            State = new GhostComponentSerializer.State
            {
                GhostFieldsHash = 270396784890700208,
                ExcludeFromComponentCollectionHash = 0,
                ComponentType = ComponentType.ReadWrite<PropHunt.Mixed.Components.KCCJumping>(),
                ComponentSize = UnsafeUtility.SizeOf<PropHunt.Mixed.Components.KCCJumping>(),
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
            State.NumPredictionErrorNames = GetPredictionErrorNames(ref State.PredictionErrorNames);
            #endif
        }
        public static readonly GhostComponentSerializer.State State;
        public struct Snapshot
        {
            public int jumpForce;
            public uint attemptingJump;
        }
        public const int ChangeMaskBits = 2;
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.CopyToFromSnapshotDelegate))]
        private static void CopyToSnapshot(IntPtr stateData, IntPtr snapshotData, int snapshotOffset, int snapshotStride, IntPtr componentData, int componentStride, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData, snapshotOffset + snapshotStride*i);
                ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.KCCJumping>(componentData, componentStride*i);
                ref var serializerState = ref GhostComponentSerializer.TypeCast<GhostSerializerState>(stateData, 0);
                snapshot.jumpForce = (int) math.round(component.jumpForce * 100);
                snapshot.attemptingJump = component.attemptingJump?1u:0;
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
                ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.KCCJumping>(componentData, componentStride*i);
                var deserializerState = GhostComponentSerializer.TypeCast<GhostDeserializerState>(stateData, 0);
                deserializerState.SnapshotTick = snapshotInterpolationData.Tick;
                component.jumpForce =
                    math.lerp(snapshotBefore.jumpForce * 0.01f,
                        snapshotAfter.jumpForce * 0.01f, snapshotInterpolationFactor);
                component.attemptingJump = snapshotBefore.attemptingJump != 0;
            }
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.RestoreFromBackupDelegate))]
        private static void RestoreFromBackup(IntPtr componentData, IntPtr backupData)
        {
            ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.KCCJumping>(componentData, 0);
            ref var backup = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.KCCJumping>(backupData, 0);
            component.jumpForce = backup.jumpForce;
            component.attemptingJump = backup.attemptingJump;
        }

        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.PredictDeltaDelegate))]
        private static void PredictDelta(IntPtr snapshotData, IntPtr baseline1Data, IntPtr baseline2Data, ref GhostDeltaPredictor predictor)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline1 = ref GhostComponentSerializer.TypeCast<Snapshot>(baseline1Data);
            ref var baseline2 = ref GhostComponentSerializer.TypeCast<Snapshot>(baseline2Data);
            snapshot.jumpForce = predictor.PredictInt(snapshot.jumpForce, baseline1.jumpForce, baseline2.jumpForce);
            snapshot.attemptingJump = (uint)predictor.PredictInt((int)snapshot.attemptingJump, (int)baseline1.attemptingJump, (int)baseline2.attemptingJump);
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.CalculateChangeMaskDelegate))]
        private static void CalculateChangeMask(IntPtr snapshotData, IntPtr baselineData, IntPtr bits, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask;
            changeMask = (snapshot.jumpForce != baseline.jumpForce) ? 1u : 0;
            changeMask |= (snapshot.attemptingJump != baseline.attemptingJump) ? (1u<<1) : 0;
            GhostComponentSerializer.CopyToChangeMask(bits, changeMask, startOffset, 2);
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.SerializeDelegate))]
        private static void Serialize(IntPtr snapshotData, IntPtr baselineData, ref DataStreamWriter writer, ref NetworkCompressionModel compressionModel, IntPtr changeMaskData, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask = GhostComponentSerializer.CopyFromChangeMask(changeMaskData, startOffset, ChangeMaskBits);
            if ((changeMask & (1 << 0)) != 0)
                writer.WritePackedIntDelta(snapshot.jumpForce, baseline.jumpForce, compressionModel);
            if ((changeMask & (1 << 1)) != 0)
                writer.WritePackedUIntDelta(snapshot.attemptingJump, baseline.attemptingJump, compressionModel);
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.DeserializeDelegate))]
        private static void Deserialize(IntPtr snapshotData, IntPtr baselineData, ref DataStreamReader reader, ref NetworkCompressionModel compressionModel, IntPtr changeMaskData, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask = GhostComponentSerializer.CopyFromChangeMask(changeMaskData, startOffset, ChangeMaskBits);
            if ((changeMask & (1 << 0)) != 0)
                snapshot.jumpForce = reader.ReadPackedIntDelta(baseline.jumpForce, compressionModel);
            else
                snapshot.jumpForce = baseline.jumpForce;
            if ((changeMask & (1 << 1)) != 0)
                snapshot.attemptingJump = reader.ReadPackedUIntDelta(baseline.attemptingJump, compressionModel);
            else
                snapshot.attemptingJump = baseline.attemptingJump;
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.ReportPredictionErrorsDelegate))]
        private static void ReportPredictionErrors(IntPtr componentData, IntPtr backupData, ref UnsafeList<float> errors)
        {
            ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.KCCJumping>(componentData, 0);
            ref var backup = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.KCCJumping>(backupData, 0);
            int errorIndex = 0;
            errors[errorIndex] = math.max(errors[errorIndex], math.abs(component.jumpForce - backup.jumpForce));
            ++errorIndex;
            errors[errorIndex] = math.max(errors[errorIndex], (component.attemptingJump != backup.attemptingJump) ? 1 : 0);
            ++errorIndex;
        }
        private static int GetPredictionErrorNames(ref FixedString512 names)
        {
            int nameCount = 0;
            if (nameCount != 0)
                names.Append(new FixedString32(","));
            names.Append(new FixedString64("jumpForce"));
            ++nameCount;
            if (nameCount != 0)
                names.Append(new FixedString32(","));
            names.Append(new FixedString64("attemptingJump"));
            ++nameCount;
            return nameCount;
        }
        #endif
    }
}