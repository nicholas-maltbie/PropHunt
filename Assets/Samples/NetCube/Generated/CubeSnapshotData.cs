using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;

public struct CubeSnapshotData : ISnapshotData<CubeSnapshotData>
{
    public uint tick;
    private int MovableCubeComponentPlayerId;
    private int PlayerViewpitch;
    private int PlayerViewyaw;
    private int RotationValueX;
    private int RotationValueY;
    private int RotationValueZ;
    private int RotationValueW;
    private int TranslationValueX;
    private int TranslationValueY;
    private int TranslationValueZ;
    uint changeMask0;

    public uint Tick => tick;
    public int GetMovableCubeComponentPlayerId(GhostDeserializerState deserializerState)
    {
        return (int)MovableCubeComponentPlayerId;
    }
    public int GetMovableCubeComponentPlayerId()
    {
        return (int)MovableCubeComponentPlayerId;
    }
    public void SetMovableCubeComponentPlayerId(int val, GhostSerializerState serializerState)
    {
        MovableCubeComponentPlayerId = (int)val;
    }
    public void SetMovableCubeComponentPlayerId(int val)
    {
        MovableCubeComponentPlayerId = (int)val;
    }
    public float GetPlayerViewpitch(GhostDeserializerState deserializerState)
    {
        return PlayerViewpitch * 0.01f;
    }
    public float GetPlayerViewpitch()
    {
        return PlayerViewpitch * 0.01f;
    }
    public void SetPlayerViewpitch(float val, GhostSerializerState serializerState)
    {
        PlayerViewpitch = (int)(val * 100);
    }
    public void SetPlayerViewpitch(float val)
    {
        PlayerViewpitch = (int)(val * 100);
    }
    public float GetPlayerViewyaw(GhostDeserializerState deserializerState)
    {
        return PlayerViewyaw * 0.01f;
    }
    public float GetPlayerViewyaw()
    {
        return PlayerViewyaw * 0.01f;
    }
    public void SetPlayerViewyaw(float val, GhostSerializerState serializerState)
    {
        PlayerViewyaw = (int)(val * 100);
    }
    public void SetPlayerViewyaw(float val)
    {
        PlayerViewyaw = (int)(val * 100);
    }
    public quaternion GetRotationValue(GhostDeserializerState deserializerState)
    {
        return GetRotationValue();
    }
    public quaternion GetRotationValue()
    {
        return new quaternion(RotationValueX * 0.001f, RotationValueY * 0.001f, RotationValueZ * 0.001f, RotationValueW * 0.001f);
    }
    public void SetRotationValue(quaternion q, GhostSerializerState serializerState)
    {
        SetRotationValue(q);
    }
    public void SetRotationValue(quaternion q)
    {
        RotationValueX = (int)(q.value.x * 1000);
        RotationValueY = (int)(q.value.y * 1000);
        RotationValueZ = (int)(q.value.z * 1000);
        RotationValueW = (int)(q.value.w * 1000);
    }
    public float3 GetTranslationValue(GhostDeserializerState deserializerState)
    {
        return GetTranslationValue();
    }
    public float3 GetTranslationValue()
    {
        return new float3(TranslationValueX * 0.01f, TranslationValueY * 0.01f, TranslationValueZ * 0.01f);
    }
    public void SetTranslationValue(float3 val, GhostSerializerState serializerState)
    {
        SetTranslationValue(val);
    }
    public void SetTranslationValue(float3 val)
    {
        TranslationValueX = (int)(val.x * 100);
        TranslationValueY = (int)(val.y * 100);
        TranslationValueZ = (int)(val.z * 100);
    }

    public void PredictDelta(uint tick, ref CubeSnapshotData baseline1, ref CubeSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        MovableCubeComponentPlayerId = predictor.PredictInt(MovableCubeComponentPlayerId, baseline1.MovableCubeComponentPlayerId, baseline2.MovableCubeComponentPlayerId);
        PlayerViewpitch = predictor.PredictInt(PlayerViewpitch, baseline1.PlayerViewpitch, baseline2.PlayerViewpitch);
        PlayerViewyaw = predictor.PredictInt(PlayerViewyaw, baseline1.PlayerViewyaw, baseline2.PlayerViewyaw);
        RotationValueX = predictor.PredictInt(RotationValueX, baseline1.RotationValueX, baseline2.RotationValueX);
        RotationValueY = predictor.PredictInt(RotationValueY, baseline1.RotationValueY, baseline2.RotationValueY);
        RotationValueZ = predictor.PredictInt(RotationValueZ, baseline1.RotationValueZ, baseline2.RotationValueZ);
        RotationValueW = predictor.PredictInt(RotationValueW, baseline1.RotationValueW, baseline2.RotationValueW);
        TranslationValueX = predictor.PredictInt(TranslationValueX, baseline1.TranslationValueX, baseline2.TranslationValueX);
        TranslationValueY = predictor.PredictInt(TranslationValueY, baseline1.TranslationValueY, baseline2.TranslationValueY);
        TranslationValueZ = predictor.PredictInt(TranslationValueZ, baseline1.TranslationValueZ, baseline2.TranslationValueZ);
    }

    public void Serialize(int networkId, ref CubeSnapshotData baseline, ref DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        changeMask0 = (MovableCubeComponentPlayerId != baseline.MovableCubeComponentPlayerId) ? 1u : 0;
        changeMask0 |= (PlayerViewpitch != baseline.PlayerViewpitch) ? (1u<<1) : 0;
        changeMask0 |= (PlayerViewyaw != baseline.PlayerViewyaw) ? (1u<<2) : 0;
        changeMask0 |= (RotationValueX != baseline.RotationValueX ||
                                           RotationValueY != baseline.RotationValueY ||
                                           RotationValueZ != baseline.RotationValueZ ||
                                           RotationValueW != baseline.RotationValueW) ? (1u<<3) : 0;
        changeMask0 |= (TranslationValueX != baseline.TranslationValueX ||
                                           TranslationValueY != baseline.TranslationValueY ||
                                           TranslationValueZ != baseline.TranslationValueZ) ? (1u<<4) : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        bool isPredicted = GetMovableCubeComponentPlayerId() == networkId;
        writer.WritePackedUInt(isPredicted?1u:0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            writer.WritePackedIntDelta(MovableCubeComponentPlayerId, baseline.MovableCubeComponentPlayerId, compressionModel);
        if ((changeMask0 & (1 << 3)) != 0)
        {
            writer.WritePackedIntDelta(RotationValueX, baseline.RotationValueX, compressionModel);
            writer.WritePackedIntDelta(RotationValueY, baseline.RotationValueY, compressionModel);
            writer.WritePackedIntDelta(RotationValueZ, baseline.RotationValueZ, compressionModel);
            writer.WritePackedIntDelta(RotationValueW, baseline.RotationValueW, compressionModel);
        }
        if ((changeMask0 & (1 << 4)) != 0)
        {
            writer.WritePackedIntDelta(TranslationValueX, baseline.TranslationValueX, compressionModel);
            writer.WritePackedIntDelta(TranslationValueY, baseline.TranslationValueY, compressionModel);
            writer.WritePackedIntDelta(TranslationValueZ, baseline.TranslationValueZ, compressionModel);
        }
        if (isPredicted)
        {
            if ((changeMask0 & (1 << 1)) != 0)
                writer.WritePackedIntDelta(PlayerViewpitch, baseline.PlayerViewpitch, compressionModel);
            if ((changeMask0 & (1 << 2)) != 0)
                writer.WritePackedIntDelta(PlayerViewyaw, baseline.PlayerViewyaw, compressionModel);
        }
    }

    public void Deserialize(uint tick, ref CubeSnapshotData baseline, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        changeMask0 = reader.ReadPackedUIntDelta(baseline.changeMask0, compressionModel);
        bool isPredicted = reader.ReadPackedUInt(compressionModel)!=0;
        if ((changeMask0 & (1 << 0)) != 0)
            MovableCubeComponentPlayerId = reader.ReadPackedIntDelta(baseline.MovableCubeComponentPlayerId, compressionModel);
        else
            MovableCubeComponentPlayerId = baseline.MovableCubeComponentPlayerId;
        if ((changeMask0 & (1 << 3)) != 0)
        {
            RotationValueX = reader.ReadPackedIntDelta(baseline.RotationValueX, compressionModel);
            RotationValueY = reader.ReadPackedIntDelta(baseline.RotationValueY, compressionModel);
            RotationValueZ = reader.ReadPackedIntDelta(baseline.RotationValueZ, compressionModel);
            RotationValueW = reader.ReadPackedIntDelta(baseline.RotationValueW, compressionModel);
        }
        else
        {
            RotationValueX = baseline.RotationValueX;
            RotationValueY = baseline.RotationValueY;
            RotationValueZ = baseline.RotationValueZ;
            RotationValueW = baseline.RotationValueW;
        }
        if ((changeMask0 & (1 << 4)) != 0)
        {
            TranslationValueX = reader.ReadPackedIntDelta(baseline.TranslationValueX, compressionModel);
            TranslationValueY = reader.ReadPackedIntDelta(baseline.TranslationValueY, compressionModel);
            TranslationValueZ = reader.ReadPackedIntDelta(baseline.TranslationValueZ, compressionModel);
        }
        else
        {
            TranslationValueX = baseline.TranslationValueX;
            TranslationValueY = baseline.TranslationValueY;
            TranslationValueZ = baseline.TranslationValueZ;
        }
        if (isPredicted)
        {
            if ((changeMask0 & (1 << 1)) != 0)
                PlayerViewpitch = reader.ReadPackedIntDelta(baseline.PlayerViewpitch, compressionModel);
            else
                PlayerViewpitch = baseline.PlayerViewpitch;
            if ((changeMask0 & (1 << 2)) != 0)
                PlayerViewyaw = reader.ReadPackedIntDelta(baseline.PlayerViewyaw, compressionModel);
            else
                PlayerViewyaw = baseline.PlayerViewyaw;
        }
    }
    public void Interpolate(ref CubeSnapshotData target, float factor)
    {
        SetPlayerViewpitch(math.lerp(GetPlayerViewpitch(), target.GetPlayerViewpitch(), factor));
        SetPlayerViewyaw(math.lerp(GetPlayerViewyaw(), target.GetPlayerViewyaw(), factor));
        SetRotationValue(math.slerp(GetRotationValue(), target.GetRotationValue(), factor));
        SetTranslationValue(math.lerp(GetTranslationValue(), target.GetTranslationValue(), factor));
    }
}
