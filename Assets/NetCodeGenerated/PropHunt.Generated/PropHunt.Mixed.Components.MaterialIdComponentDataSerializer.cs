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
    public struct PropHuntMixedComponentsMaterialIdComponentDataGhostComponentSerializer
    {
        static PropHuntMixedComponentsMaterialIdComponentDataGhostComponentSerializer()
        {
            State = new GhostComponentSerializer.State
            {
<<<<<<< HEAD:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.MaterialIdComponentDataSerializer.cs
                GhostFieldsHash = 14767913548786401661,
=======
                GhostFieldsHash = 17884505960735121,
>>>>>>> Added an extra distance to ground and falling distance:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.KCCGroundedSerializer.cs
                ExcludeFromComponentCollectionHash = 0,
                ComponentType = ComponentType.ReadWrite<PropHunt.Mixed.Components.MaterialIdComponentData>(),
                ComponentSize = UnsafeUtility.SizeOf<PropHunt.Mixed.Components.MaterialIdComponentData>(),
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
<<<<<<< HEAD:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.MaterialIdComponentDataSerializer.cs
            public int materialId;
        }
        public const int ChangeMaskBits = 1;
=======
            public int maxWalkAngle;
            public int groundCheckDistance;
            public int groundFallingDistance;
        }
        public const int ChangeMaskBits = 3;
>>>>>>> Added an extra distance to ground and falling distance:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.KCCGroundedSerializer.cs
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.CopyToFromSnapshotDelegate))]
        private static void CopyToSnapshot(IntPtr stateData, IntPtr snapshotData, int snapshotOffset, int snapshotStride, IntPtr componentData, int componentStride, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData, snapshotOffset + snapshotStride*i);
                ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.MaterialIdComponentData>(componentData, componentStride*i);
                ref var serializerState = ref GhostComponentSerializer.TypeCast<GhostSerializerState>(stateData, 0);
<<<<<<< HEAD:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.MaterialIdComponentDataSerializer.cs
                snapshot.materialId = (int) component.materialId;
=======
                snapshot.maxWalkAngle = (int) math.round(component.maxWalkAngle * 100);
                snapshot.groundCheckDistance = (int) math.round(component.groundCheckDistance * 100);
                snapshot.groundFallingDistance = (int) math.round(component.groundFallingDistance * 100);
>>>>>>> Added an extra distance to ground and falling distance:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.KCCGroundedSerializer.cs
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
                ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.MaterialIdComponentData>(componentData, componentStride*i);
                var deserializerState = GhostComponentSerializer.TypeCast<GhostDeserializerState>(stateData, 0);
                deserializerState.SnapshotTick = snapshotInterpolationData.Tick;
<<<<<<< HEAD:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.MaterialIdComponentDataSerializer.cs
                component.materialId = (int) snapshotBefore.materialId;
=======
                component.maxWalkAngle =
                    math.lerp(snapshotBefore.maxWalkAngle * 0.01f,
                        snapshotAfter.maxWalkAngle * 0.01f, snapshotInterpolationFactor);
                component.groundCheckDistance =
                    math.lerp(snapshotBefore.groundCheckDistance * 0.01f,
                        snapshotAfter.groundCheckDistance * 0.01f, snapshotInterpolationFactor);
                component.groundFallingDistance =
                    math.lerp(snapshotBefore.groundFallingDistance * 0.01f,
                        snapshotAfter.groundFallingDistance * 0.01f, snapshotInterpolationFactor);
>>>>>>> Added an extra distance to ground and falling distance:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.KCCGroundedSerializer.cs
            }
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.RestoreFromBackupDelegate))]
        private static void RestoreFromBackup(IntPtr componentData, IntPtr backupData)
        {
<<<<<<< HEAD:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.MaterialIdComponentDataSerializer.cs
            ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.MaterialIdComponentData>(componentData, 0);
            ref var backup = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.MaterialIdComponentData>(backupData, 0);
            component.materialId = backup.materialId;
=======
            ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.KCCGrounded>(componentData, 0);
            ref var backup = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.KCCGrounded>(backupData, 0);
            component.maxWalkAngle = backup.maxWalkAngle;
            component.groundCheckDistance = backup.groundCheckDistance;
            component.groundFallingDistance = backup.groundFallingDistance;
>>>>>>> Added an extra distance to ground and falling distance:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.KCCGroundedSerializer.cs
        }

        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.PredictDeltaDelegate))]
        private static void PredictDelta(IntPtr snapshotData, IntPtr baseline1Data, IntPtr baseline2Data, ref GhostDeltaPredictor predictor)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline1 = ref GhostComponentSerializer.TypeCast<Snapshot>(baseline1Data);
            ref var baseline2 = ref GhostComponentSerializer.TypeCast<Snapshot>(baseline2Data);
<<<<<<< HEAD:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.MaterialIdComponentDataSerializer.cs
            snapshot.materialId = predictor.PredictInt(snapshot.materialId, baseline1.materialId, baseline2.materialId);
=======
            snapshot.maxWalkAngle = predictor.PredictInt(snapshot.maxWalkAngle, baseline1.maxWalkAngle, baseline2.maxWalkAngle);
            snapshot.groundCheckDistance = predictor.PredictInt(snapshot.groundCheckDistance, baseline1.groundCheckDistance, baseline2.groundCheckDistance);
            snapshot.groundFallingDistance = predictor.PredictInt(snapshot.groundFallingDistance, baseline1.groundFallingDistance, baseline2.groundFallingDistance);
>>>>>>> Added an extra distance to ground and falling distance:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.KCCGroundedSerializer.cs
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.CalculateChangeMaskDelegate))]
        private static void CalculateChangeMask(IntPtr snapshotData, IntPtr baselineData, IntPtr bits, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask;
<<<<<<< HEAD:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.MaterialIdComponentDataSerializer.cs
            changeMask = (snapshot.materialId != baseline.materialId) ? 1u : 0;
            GhostComponentSerializer.CopyToChangeMask(bits, changeMask, startOffset, 1);
=======
            changeMask = (snapshot.maxWalkAngle != baseline.maxWalkAngle) ? 1u : 0;
            changeMask |= (snapshot.groundCheckDistance != baseline.groundCheckDistance) ? (1u<<1) : 0;
            changeMask |= (snapshot.groundFallingDistance != baseline.groundFallingDistance) ? (1u<<2) : 0;
            GhostComponentSerializer.CopyToChangeMask(bits, changeMask, startOffset, 3);
>>>>>>> Added an extra distance to ground and falling distance:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.KCCGroundedSerializer.cs
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.SerializeDelegate))]
        private static void Serialize(IntPtr snapshotData, IntPtr baselineData, ref DataStreamWriter writer, ref NetworkCompressionModel compressionModel, IntPtr changeMaskData, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask = GhostComponentSerializer.CopyFromChangeMask(changeMaskData, startOffset, ChangeMaskBits);
            if ((changeMask & (1 << 0)) != 0)
<<<<<<< HEAD:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.MaterialIdComponentDataSerializer.cs
                writer.WritePackedIntDelta(snapshot.materialId, baseline.materialId, compressionModel);
=======
                writer.WritePackedIntDelta(snapshot.maxWalkAngle, baseline.maxWalkAngle, compressionModel);
            if ((changeMask & (1 << 1)) != 0)
                writer.WritePackedIntDelta(snapshot.groundCheckDistance, baseline.groundCheckDistance, compressionModel);
            if ((changeMask & (1 << 2)) != 0)
                writer.WritePackedIntDelta(snapshot.groundFallingDistance, baseline.groundFallingDistance, compressionModel);
>>>>>>> Added an extra distance to ground and falling distance:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.KCCGroundedSerializer.cs
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.DeserializeDelegate))]
        private static void Deserialize(IntPtr snapshotData, IntPtr baselineData, ref DataStreamReader reader, ref NetworkCompressionModel compressionModel, IntPtr changeMaskData, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask = GhostComponentSerializer.CopyFromChangeMask(changeMaskData, startOffset, ChangeMaskBits);
            if ((changeMask & (1 << 0)) != 0)
                snapshot.materialId = reader.ReadPackedIntDelta(baseline.materialId, compressionModel);
            else
<<<<<<< HEAD:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.MaterialIdComponentDataSerializer.cs
                snapshot.materialId = baseline.materialId;
=======
                snapshot.maxWalkAngle = baseline.maxWalkAngle;
            if ((changeMask & (1 << 1)) != 0)
                snapshot.groundCheckDistance = reader.ReadPackedIntDelta(baseline.groundCheckDistance, compressionModel);
            else
                snapshot.groundCheckDistance = baseline.groundCheckDistance;
            if ((changeMask & (1 << 2)) != 0)
                snapshot.groundFallingDistance = reader.ReadPackedIntDelta(baseline.groundFallingDistance, compressionModel);
            else
                snapshot.groundFallingDistance = baseline.groundFallingDistance;
>>>>>>> Added an extra distance to ground and falling distance:Assets/NetCodeGenerated/PropHunt.Generated/PropHunt.Mixed.Components.KCCGroundedSerializer.cs
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.ReportPredictionErrorsDelegate))]
        private static void ReportPredictionErrors(IntPtr componentData, IntPtr backupData, ref UnsafeList<float> errors)
        {
            ref var component = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.MaterialIdComponentData>(componentData, 0);
            ref var backup = ref GhostComponentSerializer.TypeCast<PropHunt.Mixed.Components.MaterialIdComponentData>(backupData, 0);
            int errorIndex = 0;
            errors[errorIndex] = math.max(errors[errorIndex], math.abs(component.materialId - backup.materialId));
            ++errorIndex;
            errors[errorIndex] = math.max(errors[errorIndex], math.abs(component.groundFallingDistance - backup.groundFallingDistance));
            ++errorIndex;
        }
        private static int GetPredictionErrorNames(ref FixedString512 names)
        {
            int nameCount = 0;
            if (nameCount != 0)
                names.Append(new FixedString32(","));
            names.Append(new FixedString64("materialId"));
            ++nameCount;
            if (nameCount != 0)
                names.Append(new FixedString32(","));
            names.Append(new FixedString64("groundFallingDistance"));
            ++nameCount;
            return nameCount;
        }
        #endif
    }
}