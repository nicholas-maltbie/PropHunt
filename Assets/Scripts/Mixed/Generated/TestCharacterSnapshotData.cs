using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;

public struct TestCharacterSnapshotData : ISnapshotData<TestCharacterSnapshotData>
{
    public uint tick;
    private int KCCGravitygravityAccelerationX;
    private int KCCGravitygravityAccelerationY;
    private int KCCGravitygravityAccelerationZ;
    private int KCCGroundedmaxWalkAngle;
    private int KCCGroundedgroundCheckDistance;
    private int KCCJumpingjumpForce;
    private uint KCCJumpingattemptingJump;
    private int KCCMovementSettingsmoveSpeed;
    private int KCCMovementSettingssprintMultiplier;
    private int KCCMovementSettingsmoveMaxBounces;
    private int KCCMovementSettingsmoveAnglePower;
    private int KCCMovementSettingsmovePushPower;
    private int KCCMovementSettingsmovePushDecay;
    private int KCCMovementSettingsfallMaxBounces;
    private int KCCMovementSettingsfallPushPower;
    private int KCCMovementSettingsfallAnglePower;
    private int KCCMovementSettingsfallPushDecay;
    private int KCCVelocityplayerVelocityX;
    private int KCCVelocityplayerVelocityY;
    private int KCCVelocityplayerVelocityZ;
    private int KCCVelocityworldVelocityX;
    private int KCCVelocityworldVelocityY;
    private int KCCVelocityworldVelocityZ;
    private int PlayerIdplayerId;
    private int PlayerViewviewRotationRate;
    private int PlayerViewpitch;
    private int PlayerViewyaw;
    private int RotationValueX;
    private int RotationValueY;
    private int RotationValueZ;
    private int RotationValueW;
    private int TranslationValueX;
    private int TranslationValueY;
    private int TranslationValueZ;
    private int Child0RotationValueX;
    private int Child0RotationValueY;
    private int Child0RotationValueZ;
    private int Child0RotationValueW;
    private int Child0TranslationValueX;
    private int Child0TranslationValueY;
    private int Child0TranslationValueZ;
    private int Child1RotationValueX;
    private int Child1RotationValueY;
    private int Child1RotationValueZ;
    private int Child1RotationValueW;
    private int Child1TranslationValueX;
    private int Child1TranslationValueY;
    private int Child1TranslationValueZ;
    uint changeMask0;

    public uint Tick => tick;
    public float3 GetKCCGravitygravityAcceleration(GhostDeserializerState deserializerState)
    {
        return GetKCCGravitygravityAcceleration();
    }
    public float3 GetKCCGravitygravityAcceleration()
    {
        return new float3(KCCGravitygravityAccelerationX * 0.01f, KCCGravitygravityAccelerationY * 0.01f, KCCGravitygravityAccelerationZ * 0.01f);
    }
    public void SetKCCGravitygravityAcceleration(float3 val, GhostSerializerState serializerState)
    {
        SetKCCGravitygravityAcceleration(val);
    }
    public void SetKCCGravitygravityAcceleration(float3 val)
    {
        KCCGravitygravityAccelerationX = (int)(val.x * 100);
        KCCGravitygravityAccelerationY = (int)(val.y * 100);
        KCCGravitygravityAccelerationZ = (int)(val.z * 100);
    }
    public float GetKCCGroundedmaxWalkAngle(GhostDeserializerState deserializerState)
    {
        return KCCGroundedmaxWalkAngle * 0.01f;
    }
    public float GetKCCGroundedmaxWalkAngle()
    {
        return KCCGroundedmaxWalkAngle * 0.01f;
    }
    public void SetKCCGroundedmaxWalkAngle(float val, GhostSerializerState serializerState)
    {
        KCCGroundedmaxWalkAngle = (int)(val * 100);
    }
    public void SetKCCGroundedmaxWalkAngle(float val)
    {
        KCCGroundedmaxWalkAngle = (int)(val * 100);
    }
    public float GetKCCGroundedgroundCheckDistance(GhostDeserializerState deserializerState)
    {
        return KCCGroundedgroundCheckDistance * 0.01f;
    }
    public float GetKCCGroundedgroundCheckDistance()
    {
        return KCCGroundedgroundCheckDistance * 0.01f;
    }
    public void SetKCCGroundedgroundCheckDistance(float val, GhostSerializerState serializerState)
    {
        KCCGroundedgroundCheckDistance = (int)(val * 100);
    }
    public void SetKCCGroundedgroundCheckDistance(float val)
    {
        KCCGroundedgroundCheckDistance = (int)(val * 100);
    }
    public float GetKCCJumpingjumpForce(GhostDeserializerState deserializerState)
    {
        return KCCJumpingjumpForce * 0.01f;
    }
    public float GetKCCJumpingjumpForce()
    {
        return KCCJumpingjumpForce * 0.01f;
    }
    public void SetKCCJumpingjumpForce(float val, GhostSerializerState serializerState)
    {
        KCCJumpingjumpForce = (int)(val * 100);
    }
    public void SetKCCJumpingjumpForce(float val)
    {
        KCCJumpingjumpForce = (int)(val * 100);
    }
    public bool GetKCCJumpingattemptingJump(GhostDeserializerState deserializerState)
    {
        return KCCJumpingattemptingJump!=0;
    }
    public bool GetKCCJumpingattemptingJump()
    {
        return KCCJumpingattemptingJump!=0;
    }
    public void SetKCCJumpingattemptingJump(bool val, GhostSerializerState serializerState)
    {
        KCCJumpingattemptingJump = val?1u:0;
    }
    public void SetKCCJumpingattemptingJump(bool val)
    {
        KCCJumpingattemptingJump = val?1u:0;
    }
    public float GetKCCMovementSettingsmoveSpeed(GhostDeserializerState deserializerState)
    {
        return KCCMovementSettingsmoveSpeed * 0.01f;
    }
    public float GetKCCMovementSettingsmoveSpeed()
    {
        return KCCMovementSettingsmoveSpeed * 0.01f;
    }
    public void SetKCCMovementSettingsmoveSpeed(float val, GhostSerializerState serializerState)
    {
        KCCMovementSettingsmoveSpeed = (int)(val * 100);
    }
    public void SetKCCMovementSettingsmoveSpeed(float val)
    {
        KCCMovementSettingsmoveSpeed = (int)(val * 100);
    }
    public float GetKCCMovementSettingssprintMultiplier(GhostDeserializerState deserializerState)
    {
        return KCCMovementSettingssprintMultiplier * 0.01f;
    }
    public float GetKCCMovementSettingssprintMultiplier()
    {
        return KCCMovementSettingssprintMultiplier * 0.01f;
    }
    public void SetKCCMovementSettingssprintMultiplier(float val, GhostSerializerState serializerState)
    {
        KCCMovementSettingssprintMultiplier = (int)(val * 100);
    }
    public void SetKCCMovementSettingssprintMultiplier(float val)
    {
        KCCMovementSettingssprintMultiplier = (int)(val * 100);
    }
    public int GetKCCMovementSettingsmoveMaxBounces(GhostDeserializerState deserializerState)
    {
        return (int)KCCMovementSettingsmoveMaxBounces;
    }
    public int GetKCCMovementSettingsmoveMaxBounces()
    {
        return (int)KCCMovementSettingsmoveMaxBounces;
    }
    public void SetKCCMovementSettingsmoveMaxBounces(int val, GhostSerializerState serializerState)
    {
        KCCMovementSettingsmoveMaxBounces = (int)val;
    }
    public void SetKCCMovementSettingsmoveMaxBounces(int val)
    {
        KCCMovementSettingsmoveMaxBounces = (int)val;
    }
    public float GetKCCMovementSettingsmoveAnglePower(GhostDeserializerState deserializerState)
    {
        return KCCMovementSettingsmoveAnglePower * 0.01f;
    }
    public float GetKCCMovementSettingsmoveAnglePower()
    {
        return KCCMovementSettingsmoveAnglePower * 0.01f;
    }
    public void SetKCCMovementSettingsmoveAnglePower(float val, GhostSerializerState serializerState)
    {
        KCCMovementSettingsmoveAnglePower = (int)(val * 100);
    }
    public void SetKCCMovementSettingsmoveAnglePower(float val)
    {
        KCCMovementSettingsmoveAnglePower = (int)(val * 100);
    }
    public float GetKCCMovementSettingsmovePushPower(GhostDeserializerState deserializerState)
    {
        return KCCMovementSettingsmovePushPower * 0.01f;
    }
    public float GetKCCMovementSettingsmovePushPower()
    {
        return KCCMovementSettingsmovePushPower * 0.01f;
    }
    public void SetKCCMovementSettingsmovePushPower(float val, GhostSerializerState serializerState)
    {
        KCCMovementSettingsmovePushPower = (int)(val * 100);
    }
    public void SetKCCMovementSettingsmovePushPower(float val)
    {
        KCCMovementSettingsmovePushPower = (int)(val * 100);
    }
    public float GetKCCMovementSettingsmovePushDecay(GhostDeserializerState deserializerState)
    {
        return KCCMovementSettingsmovePushDecay * 0.01f;
    }
    public float GetKCCMovementSettingsmovePushDecay()
    {
        return KCCMovementSettingsmovePushDecay * 0.01f;
    }
    public void SetKCCMovementSettingsmovePushDecay(float val, GhostSerializerState serializerState)
    {
        KCCMovementSettingsmovePushDecay = (int)(val * 100);
    }
    public void SetKCCMovementSettingsmovePushDecay(float val)
    {
        KCCMovementSettingsmovePushDecay = (int)(val * 100);
    }
    public int GetKCCMovementSettingsfallMaxBounces(GhostDeserializerState deserializerState)
    {
        return (int)KCCMovementSettingsfallMaxBounces;
    }
    public int GetKCCMovementSettingsfallMaxBounces()
    {
        return (int)KCCMovementSettingsfallMaxBounces;
    }
    public void SetKCCMovementSettingsfallMaxBounces(int val, GhostSerializerState serializerState)
    {
        KCCMovementSettingsfallMaxBounces = (int)val;
    }
    public void SetKCCMovementSettingsfallMaxBounces(int val)
    {
        KCCMovementSettingsfallMaxBounces = (int)val;
    }
    public float GetKCCMovementSettingsfallPushPower(GhostDeserializerState deserializerState)
    {
        return KCCMovementSettingsfallPushPower * 0.01f;
    }
    public float GetKCCMovementSettingsfallPushPower()
    {
        return KCCMovementSettingsfallPushPower * 0.01f;
    }
    public void SetKCCMovementSettingsfallPushPower(float val, GhostSerializerState serializerState)
    {
        KCCMovementSettingsfallPushPower = (int)(val * 100);
    }
    public void SetKCCMovementSettingsfallPushPower(float val)
    {
        KCCMovementSettingsfallPushPower = (int)(val * 100);
    }
    public float GetKCCMovementSettingsfallAnglePower(GhostDeserializerState deserializerState)
    {
        return KCCMovementSettingsfallAnglePower * 0.01f;
    }
    public float GetKCCMovementSettingsfallAnglePower()
    {
        return KCCMovementSettingsfallAnglePower * 0.01f;
    }
    public void SetKCCMovementSettingsfallAnglePower(float val, GhostSerializerState serializerState)
    {
        KCCMovementSettingsfallAnglePower = (int)(val * 100);
    }
    public void SetKCCMovementSettingsfallAnglePower(float val)
    {
        KCCMovementSettingsfallAnglePower = (int)(val * 100);
    }
    public float GetKCCMovementSettingsfallPushDecay(GhostDeserializerState deserializerState)
    {
        return KCCMovementSettingsfallPushDecay * 0.01f;
    }
    public float GetKCCMovementSettingsfallPushDecay()
    {
        return KCCMovementSettingsfallPushDecay * 0.01f;
    }
    public void SetKCCMovementSettingsfallPushDecay(float val, GhostSerializerState serializerState)
    {
        KCCMovementSettingsfallPushDecay = (int)(val * 100);
    }
    public void SetKCCMovementSettingsfallPushDecay(float val)
    {
        KCCMovementSettingsfallPushDecay = (int)(val * 100);
    }
    public float3 GetKCCVelocityplayerVelocity(GhostDeserializerState deserializerState)
    {
        return GetKCCVelocityplayerVelocity();
    }
    public float3 GetKCCVelocityplayerVelocity()
    {
        return new float3(KCCVelocityplayerVelocityX * 0.01f, KCCVelocityplayerVelocityY * 0.01f, KCCVelocityplayerVelocityZ * 0.01f);
    }
    public void SetKCCVelocityplayerVelocity(float3 val, GhostSerializerState serializerState)
    {
        SetKCCVelocityplayerVelocity(val);
    }
    public void SetKCCVelocityplayerVelocity(float3 val)
    {
        KCCVelocityplayerVelocityX = (int)(val.x * 100);
        KCCVelocityplayerVelocityY = (int)(val.y * 100);
        KCCVelocityplayerVelocityZ = (int)(val.z * 100);
    }
    public float3 GetKCCVelocityworldVelocity(GhostDeserializerState deserializerState)
    {
        return GetKCCVelocityworldVelocity();
    }
    public float3 GetKCCVelocityworldVelocity()
    {
        return new float3(KCCVelocityworldVelocityX * 0.01f, KCCVelocityworldVelocityY * 0.01f, KCCVelocityworldVelocityZ * 0.01f);
    }
    public void SetKCCVelocityworldVelocity(float3 val, GhostSerializerState serializerState)
    {
        SetKCCVelocityworldVelocity(val);
    }
    public void SetKCCVelocityworldVelocity(float3 val)
    {
        KCCVelocityworldVelocityX = (int)(val.x * 100);
        KCCVelocityworldVelocityY = (int)(val.y * 100);
        KCCVelocityworldVelocityZ = (int)(val.z * 100);
    }
    public int GetPlayerIdplayerId(GhostDeserializerState deserializerState)
    {
        return (int)PlayerIdplayerId;
    }
    public int GetPlayerIdplayerId()
    {
        return (int)PlayerIdplayerId;
    }
    public void SetPlayerIdplayerId(int val, GhostSerializerState serializerState)
    {
        PlayerIdplayerId = (int)val;
    }
    public void SetPlayerIdplayerId(int val)
    {
        PlayerIdplayerId = (int)val;
    }
    public float GetPlayerViewviewRotationRate(GhostDeserializerState deserializerState)
    {
        return PlayerViewviewRotationRate * 0.01f;
    }
    public float GetPlayerViewviewRotationRate()
    {
        return PlayerViewviewRotationRate * 0.01f;
    }
    public void SetPlayerViewviewRotationRate(float val, GhostSerializerState serializerState)
    {
        PlayerViewviewRotationRate = (int)(val * 100);
    }
    public void SetPlayerViewviewRotationRate(float val)
    {
        PlayerViewviewRotationRate = (int)(val * 100);
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
    public quaternion GetChild0RotationValue(GhostDeserializerState deserializerState)
    {
        return GetChild0RotationValue();
    }
    public quaternion GetChild0RotationValue()
    {
        return new quaternion(Child0RotationValueX * 0.001f, Child0RotationValueY * 0.001f, Child0RotationValueZ * 0.001f, Child0RotationValueW * 0.001f);
    }
    public void SetChild0RotationValue(quaternion q, GhostSerializerState serializerState)
    {
        SetChild0RotationValue(q);
    }
    public void SetChild0RotationValue(quaternion q)
    {
        Child0RotationValueX = (int)(q.value.x * 1000);
        Child0RotationValueY = (int)(q.value.y * 1000);
        Child0RotationValueZ = (int)(q.value.z * 1000);
        Child0RotationValueW = (int)(q.value.w * 1000);
    }
    public float3 GetChild0TranslationValue(GhostDeserializerState deserializerState)
    {
        return GetChild0TranslationValue();
    }
    public float3 GetChild0TranslationValue()
    {
        return new float3(Child0TranslationValueX * 0.01f, Child0TranslationValueY * 0.01f, Child0TranslationValueZ * 0.01f);
    }
    public void SetChild0TranslationValue(float3 val, GhostSerializerState serializerState)
    {
        SetChild0TranslationValue(val);
    }
    public void SetChild0TranslationValue(float3 val)
    {
        Child0TranslationValueX = (int)(val.x * 100);
        Child0TranslationValueY = (int)(val.y * 100);
        Child0TranslationValueZ = (int)(val.z * 100);
    }
    public quaternion GetChild1RotationValue(GhostDeserializerState deserializerState)
    {
        return GetChild1RotationValue();
    }
    public quaternion GetChild1RotationValue()
    {
        return new quaternion(Child1RotationValueX * 0.001f, Child1RotationValueY * 0.001f, Child1RotationValueZ * 0.001f, Child1RotationValueW * 0.001f);
    }
    public void SetChild1RotationValue(quaternion q, GhostSerializerState serializerState)
    {
        SetChild1RotationValue(q);
    }
    public void SetChild1RotationValue(quaternion q)
    {
        Child1RotationValueX = (int)(q.value.x * 1000);
        Child1RotationValueY = (int)(q.value.y * 1000);
        Child1RotationValueZ = (int)(q.value.z * 1000);
        Child1RotationValueW = (int)(q.value.w * 1000);
    }
    public float3 GetChild1TranslationValue(GhostDeserializerState deserializerState)
    {
        return GetChild1TranslationValue();
    }
    public float3 GetChild1TranslationValue()
    {
        return new float3(Child1TranslationValueX * 0.01f, Child1TranslationValueY * 0.01f, Child1TranslationValueZ * 0.01f);
    }
    public void SetChild1TranslationValue(float3 val, GhostSerializerState serializerState)
    {
        SetChild1TranslationValue(val);
    }
    public void SetChild1TranslationValue(float3 val)
    {
        Child1TranslationValueX = (int)(val.x * 100);
        Child1TranslationValueY = (int)(val.y * 100);
        Child1TranslationValueZ = (int)(val.z * 100);
    }

    public void PredictDelta(uint tick, ref TestCharacterSnapshotData baseline1, ref TestCharacterSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        KCCGravitygravityAccelerationX = predictor.PredictInt(KCCGravitygravityAccelerationX, baseline1.KCCGravitygravityAccelerationX, baseline2.KCCGravitygravityAccelerationX);
        KCCGravitygravityAccelerationY = predictor.PredictInt(KCCGravitygravityAccelerationY, baseline1.KCCGravitygravityAccelerationY, baseline2.KCCGravitygravityAccelerationY);
        KCCGravitygravityAccelerationZ = predictor.PredictInt(KCCGravitygravityAccelerationZ, baseline1.KCCGravitygravityAccelerationZ, baseline2.KCCGravitygravityAccelerationZ);
        KCCGroundedmaxWalkAngle = predictor.PredictInt(KCCGroundedmaxWalkAngle, baseline1.KCCGroundedmaxWalkAngle, baseline2.KCCGroundedmaxWalkAngle);
        KCCGroundedgroundCheckDistance = predictor.PredictInt(KCCGroundedgroundCheckDistance, baseline1.KCCGroundedgroundCheckDistance, baseline2.KCCGroundedgroundCheckDistance);
        KCCJumpingjumpForce = predictor.PredictInt(KCCJumpingjumpForce, baseline1.KCCJumpingjumpForce, baseline2.KCCJumpingjumpForce);
        KCCJumpingattemptingJump = (uint)predictor.PredictInt((int)KCCJumpingattemptingJump, (int)baseline1.KCCJumpingattemptingJump, (int)baseline2.KCCJumpingattemptingJump);
        KCCMovementSettingsmoveSpeed = predictor.PredictInt(KCCMovementSettingsmoveSpeed, baseline1.KCCMovementSettingsmoveSpeed, baseline2.KCCMovementSettingsmoveSpeed);
        KCCMovementSettingssprintMultiplier = predictor.PredictInt(KCCMovementSettingssprintMultiplier, baseline1.KCCMovementSettingssprintMultiplier, baseline2.KCCMovementSettingssprintMultiplier);
        KCCMovementSettingsmoveMaxBounces = predictor.PredictInt(KCCMovementSettingsmoveMaxBounces, baseline1.KCCMovementSettingsmoveMaxBounces, baseline2.KCCMovementSettingsmoveMaxBounces);
        KCCMovementSettingsmoveAnglePower = predictor.PredictInt(KCCMovementSettingsmoveAnglePower, baseline1.KCCMovementSettingsmoveAnglePower, baseline2.KCCMovementSettingsmoveAnglePower);
        KCCMovementSettingsmovePushPower = predictor.PredictInt(KCCMovementSettingsmovePushPower, baseline1.KCCMovementSettingsmovePushPower, baseline2.KCCMovementSettingsmovePushPower);
        KCCMovementSettingsmovePushDecay = predictor.PredictInt(KCCMovementSettingsmovePushDecay, baseline1.KCCMovementSettingsmovePushDecay, baseline2.KCCMovementSettingsmovePushDecay);
        KCCMovementSettingsfallMaxBounces = predictor.PredictInt(KCCMovementSettingsfallMaxBounces, baseline1.KCCMovementSettingsfallMaxBounces, baseline2.KCCMovementSettingsfallMaxBounces);
        KCCMovementSettingsfallPushPower = predictor.PredictInt(KCCMovementSettingsfallPushPower, baseline1.KCCMovementSettingsfallPushPower, baseline2.KCCMovementSettingsfallPushPower);
        KCCMovementSettingsfallAnglePower = predictor.PredictInt(KCCMovementSettingsfallAnglePower, baseline1.KCCMovementSettingsfallAnglePower, baseline2.KCCMovementSettingsfallAnglePower);
        KCCMovementSettingsfallPushDecay = predictor.PredictInt(KCCMovementSettingsfallPushDecay, baseline1.KCCMovementSettingsfallPushDecay, baseline2.KCCMovementSettingsfallPushDecay);
        KCCVelocityplayerVelocityX = predictor.PredictInt(KCCVelocityplayerVelocityX, baseline1.KCCVelocityplayerVelocityX, baseline2.KCCVelocityplayerVelocityX);
        KCCVelocityplayerVelocityY = predictor.PredictInt(KCCVelocityplayerVelocityY, baseline1.KCCVelocityplayerVelocityY, baseline2.KCCVelocityplayerVelocityY);
        KCCVelocityplayerVelocityZ = predictor.PredictInt(KCCVelocityplayerVelocityZ, baseline1.KCCVelocityplayerVelocityZ, baseline2.KCCVelocityplayerVelocityZ);
        KCCVelocityworldVelocityX = predictor.PredictInt(KCCVelocityworldVelocityX, baseline1.KCCVelocityworldVelocityX, baseline2.KCCVelocityworldVelocityX);
        KCCVelocityworldVelocityY = predictor.PredictInt(KCCVelocityworldVelocityY, baseline1.KCCVelocityworldVelocityY, baseline2.KCCVelocityworldVelocityY);
        KCCVelocityworldVelocityZ = predictor.PredictInt(KCCVelocityworldVelocityZ, baseline1.KCCVelocityworldVelocityZ, baseline2.KCCVelocityworldVelocityZ);
        PlayerIdplayerId = predictor.PredictInt(PlayerIdplayerId, baseline1.PlayerIdplayerId, baseline2.PlayerIdplayerId);
        PlayerViewviewRotationRate = predictor.PredictInt(PlayerViewviewRotationRate, baseline1.PlayerViewviewRotationRate, baseline2.PlayerViewviewRotationRate);
        PlayerViewpitch = predictor.PredictInt(PlayerViewpitch, baseline1.PlayerViewpitch, baseline2.PlayerViewpitch);
        PlayerViewyaw = predictor.PredictInt(PlayerViewyaw, baseline1.PlayerViewyaw, baseline2.PlayerViewyaw);
        RotationValueX = predictor.PredictInt(RotationValueX, baseline1.RotationValueX, baseline2.RotationValueX);
        RotationValueY = predictor.PredictInt(RotationValueY, baseline1.RotationValueY, baseline2.RotationValueY);
        RotationValueZ = predictor.PredictInt(RotationValueZ, baseline1.RotationValueZ, baseline2.RotationValueZ);
        RotationValueW = predictor.PredictInt(RotationValueW, baseline1.RotationValueW, baseline2.RotationValueW);
        TranslationValueX = predictor.PredictInt(TranslationValueX, baseline1.TranslationValueX, baseline2.TranslationValueX);
        TranslationValueY = predictor.PredictInt(TranslationValueY, baseline1.TranslationValueY, baseline2.TranslationValueY);
        TranslationValueZ = predictor.PredictInt(TranslationValueZ, baseline1.TranslationValueZ, baseline2.TranslationValueZ);
        Child0RotationValueX = predictor.PredictInt(Child0RotationValueX, baseline1.Child0RotationValueX, baseline2.Child0RotationValueX);
        Child0RotationValueY = predictor.PredictInt(Child0RotationValueY, baseline1.Child0RotationValueY, baseline2.Child0RotationValueY);
        Child0RotationValueZ = predictor.PredictInt(Child0RotationValueZ, baseline1.Child0RotationValueZ, baseline2.Child0RotationValueZ);
        Child0RotationValueW = predictor.PredictInt(Child0RotationValueW, baseline1.Child0RotationValueW, baseline2.Child0RotationValueW);
        Child0TranslationValueX = predictor.PredictInt(Child0TranslationValueX, baseline1.Child0TranslationValueX, baseline2.Child0TranslationValueX);
        Child0TranslationValueY = predictor.PredictInt(Child0TranslationValueY, baseline1.Child0TranslationValueY, baseline2.Child0TranslationValueY);
        Child0TranslationValueZ = predictor.PredictInt(Child0TranslationValueZ, baseline1.Child0TranslationValueZ, baseline2.Child0TranslationValueZ);
        Child1RotationValueX = predictor.PredictInt(Child1RotationValueX, baseline1.Child1RotationValueX, baseline2.Child1RotationValueX);
        Child1RotationValueY = predictor.PredictInt(Child1RotationValueY, baseline1.Child1RotationValueY, baseline2.Child1RotationValueY);
        Child1RotationValueZ = predictor.PredictInt(Child1RotationValueZ, baseline1.Child1RotationValueZ, baseline2.Child1RotationValueZ);
        Child1RotationValueW = predictor.PredictInt(Child1RotationValueW, baseline1.Child1RotationValueW, baseline2.Child1RotationValueW);
        Child1TranslationValueX = predictor.PredictInt(Child1TranslationValueX, baseline1.Child1TranslationValueX, baseline2.Child1TranslationValueX);
        Child1TranslationValueY = predictor.PredictInt(Child1TranslationValueY, baseline1.Child1TranslationValueY, baseline2.Child1TranslationValueY);
        Child1TranslationValueZ = predictor.PredictInt(Child1TranslationValueZ, baseline1.Child1TranslationValueZ, baseline2.Child1TranslationValueZ);
    }

    public void Serialize(int networkId, ref TestCharacterSnapshotData baseline, ref DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        changeMask0 = (KCCGravitygravityAccelerationX != baseline.KCCGravitygravityAccelerationX ||
                                          KCCGravitygravityAccelerationY != baseline.KCCGravitygravityAccelerationY ||
                                          KCCGravitygravityAccelerationZ != baseline.KCCGravitygravityAccelerationZ) ? 1u : 0;
        changeMask0 |= (KCCGroundedmaxWalkAngle != baseline.KCCGroundedmaxWalkAngle) ? (1u<<1) : 0;
        changeMask0 |= (KCCGroundedgroundCheckDistance != baseline.KCCGroundedgroundCheckDistance) ? (1u<<2) : 0;
        changeMask0 |= (KCCJumpingjumpForce != baseline.KCCJumpingjumpForce) ? (1u<<3) : 0;
        changeMask0 |= (KCCJumpingattemptingJump != baseline.KCCJumpingattemptingJump) ? (1u<<4) : 0;
        changeMask0 |= (KCCMovementSettingsmoveSpeed != baseline.KCCMovementSettingsmoveSpeed) ? (1u<<5) : 0;
        changeMask0 |= (KCCMovementSettingssprintMultiplier != baseline.KCCMovementSettingssprintMultiplier) ? (1u<<6) : 0;
        changeMask0 |= (KCCMovementSettingsmoveMaxBounces != baseline.KCCMovementSettingsmoveMaxBounces) ? (1u<<7) : 0;
        changeMask0 |= (KCCMovementSettingsmoveAnglePower != baseline.KCCMovementSettingsmoveAnglePower) ? (1u<<8) : 0;
        changeMask0 |= (KCCMovementSettingsmovePushPower != baseline.KCCMovementSettingsmovePushPower) ? (1u<<9) : 0;
        changeMask0 |= (KCCMovementSettingsmovePushDecay != baseline.KCCMovementSettingsmovePushDecay) ? (1u<<10) : 0;
        changeMask0 |= (KCCMovementSettingsfallMaxBounces != baseline.KCCMovementSettingsfallMaxBounces) ? (1u<<11) : 0;
        changeMask0 |= (KCCMovementSettingsfallPushPower != baseline.KCCMovementSettingsfallPushPower) ? (1u<<12) : 0;
        changeMask0 |= (KCCMovementSettingsfallAnglePower != baseline.KCCMovementSettingsfallAnglePower) ? (1u<<13) : 0;
        changeMask0 |= (KCCMovementSettingsfallPushDecay != baseline.KCCMovementSettingsfallPushDecay) ? (1u<<14) : 0;
        changeMask0 |= (KCCVelocityplayerVelocityX != baseline.KCCVelocityplayerVelocityX ||
                                           KCCVelocityplayerVelocityY != baseline.KCCVelocityplayerVelocityY ||
                                           KCCVelocityplayerVelocityZ != baseline.KCCVelocityplayerVelocityZ) ? (1u<<15) : 0;
        changeMask0 |= (KCCVelocityworldVelocityX != baseline.KCCVelocityworldVelocityX ||
                                           KCCVelocityworldVelocityY != baseline.KCCVelocityworldVelocityY ||
                                           KCCVelocityworldVelocityZ != baseline.KCCVelocityworldVelocityZ) ? (1u<<16) : 0;
        changeMask0 |= (PlayerIdplayerId != baseline.PlayerIdplayerId) ? (1u<<17) : 0;
        changeMask0 |= (PlayerViewviewRotationRate != baseline.PlayerViewviewRotationRate) ? (1u<<18) : 0;
        changeMask0 |= (PlayerViewpitch != baseline.PlayerViewpitch) ? (1u<<19) : 0;
        changeMask0 |= (PlayerViewyaw != baseline.PlayerViewyaw) ? (1u<<20) : 0;
        changeMask0 |= (RotationValueX != baseline.RotationValueX ||
                                           RotationValueY != baseline.RotationValueY ||
                                           RotationValueZ != baseline.RotationValueZ ||
                                           RotationValueW != baseline.RotationValueW) ? (1u<<21) : 0;
        changeMask0 |= (TranslationValueX != baseline.TranslationValueX ||
                                           TranslationValueY != baseline.TranslationValueY ||
                                           TranslationValueZ != baseline.TranslationValueZ) ? (1u<<22) : 0;
        changeMask0 |= (Child0RotationValueX != baseline.Child0RotationValueX ||
                                           Child0RotationValueY != baseline.Child0RotationValueY ||
                                           Child0RotationValueZ != baseline.Child0RotationValueZ ||
                                           Child0RotationValueW != baseline.Child0RotationValueW) ? (1u<<23) : 0;
        changeMask0 |= (Child0TranslationValueX != baseline.Child0TranslationValueX ||
                                           Child0TranslationValueY != baseline.Child0TranslationValueY ||
                                           Child0TranslationValueZ != baseline.Child0TranslationValueZ) ? (1u<<24) : 0;
        changeMask0 |= (Child1RotationValueX != baseline.Child1RotationValueX ||
                                           Child1RotationValueY != baseline.Child1RotationValueY ||
                                           Child1RotationValueZ != baseline.Child1RotationValueZ ||
                                           Child1RotationValueW != baseline.Child1RotationValueW) ? (1u<<25) : 0;
        changeMask0 |= (Child1TranslationValueX != baseline.Child1TranslationValueX ||
                                           Child1TranslationValueY != baseline.Child1TranslationValueY ||
                                           Child1TranslationValueZ != baseline.Child1TranslationValueZ) ? (1u<<26) : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
        {
            writer.WritePackedIntDelta(KCCGravitygravityAccelerationX, baseline.KCCGravitygravityAccelerationX, compressionModel);
            writer.WritePackedIntDelta(KCCGravitygravityAccelerationY, baseline.KCCGravitygravityAccelerationY, compressionModel);
            writer.WritePackedIntDelta(KCCGravitygravityAccelerationZ, baseline.KCCGravitygravityAccelerationZ, compressionModel);
        }
        if ((changeMask0 & (1 << 1)) != 0)
            writer.WritePackedIntDelta(KCCGroundedmaxWalkAngle, baseline.KCCGroundedmaxWalkAngle, compressionModel);
        if ((changeMask0 & (1 << 2)) != 0)
            writer.WritePackedIntDelta(KCCGroundedgroundCheckDistance, baseline.KCCGroundedgroundCheckDistance, compressionModel);
        if ((changeMask0 & (1 << 3)) != 0)
            writer.WritePackedIntDelta(KCCJumpingjumpForce, baseline.KCCJumpingjumpForce, compressionModel);
        if ((changeMask0 & (1 << 4)) != 0)
            writer.WritePackedUIntDelta(KCCJumpingattemptingJump, baseline.KCCJumpingattemptingJump, compressionModel);
        if ((changeMask0 & (1 << 5)) != 0)
            writer.WritePackedIntDelta(KCCMovementSettingsmoveSpeed, baseline.KCCMovementSettingsmoveSpeed, compressionModel);
        if ((changeMask0 & (1 << 6)) != 0)
            writer.WritePackedIntDelta(KCCMovementSettingssprintMultiplier, baseline.KCCMovementSettingssprintMultiplier, compressionModel);
        if ((changeMask0 & (1 << 7)) != 0)
            writer.WritePackedIntDelta(KCCMovementSettingsmoveMaxBounces, baseline.KCCMovementSettingsmoveMaxBounces, compressionModel);
        if ((changeMask0 & (1 << 8)) != 0)
            writer.WritePackedIntDelta(KCCMovementSettingsmoveAnglePower, baseline.KCCMovementSettingsmoveAnglePower, compressionModel);
        if ((changeMask0 & (1 << 9)) != 0)
            writer.WritePackedIntDelta(KCCMovementSettingsmovePushPower, baseline.KCCMovementSettingsmovePushPower, compressionModel);
        if ((changeMask0 & (1 << 10)) != 0)
            writer.WritePackedIntDelta(KCCMovementSettingsmovePushDecay, baseline.KCCMovementSettingsmovePushDecay, compressionModel);
        if ((changeMask0 & (1 << 11)) != 0)
            writer.WritePackedIntDelta(KCCMovementSettingsfallMaxBounces, baseline.KCCMovementSettingsfallMaxBounces, compressionModel);
        if ((changeMask0 & (1 << 12)) != 0)
            writer.WritePackedIntDelta(KCCMovementSettingsfallPushPower, baseline.KCCMovementSettingsfallPushPower, compressionModel);
        if ((changeMask0 & (1 << 13)) != 0)
            writer.WritePackedIntDelta(KCCMovementSettingsfallAnglePower, baseline.KCCMovementSettingsfallAnglePower, compressionModel);
        if ((changeMask0 & (1 << 14)) != 0)
            writer.WritePackedIntDelta(KCCMovementSettingsfallPushDecay, baseline.KCCMovementSettingsfallPushDecay, compressionModel);
        if ((changeMask0 & (1 << 15)) != 0)
        {
            writer.WritePackedIntDelta(KCCVelocityplayerVelocityX, baseline.KCCVelocityplayerVelocityX, compressionModel);
            writer.WritePackedIntDelta(KCCVelocityplayerVelocityY, baseline.KCCVelocityplayerVelocityY, compressionModel);
            writer.WritePackedIntDelta(KCCVelocityplayerVelocityZ, baseline.KCCVelocityplayerVelocityZ, compressionModel);
        }
        if ((changeMask0 & (1 << 16)) != 0)
        {
            writer.WritePackedIntDelta(KCCVelocityworldVelocityX, baseline.KCCVelocityworldVelocityX, compressionModel);
            writer.WritePackedIntDelta(KCCVelocityworldVelocityY, baseline.KCCVelocityworldVelocityY, compressionModel);
            writer.WritePackedIntDelta(KCCVelocityworldVelocityZ, baseline.KCCVelocityworldVelocityZ, compressionModel);
        }
        if ((changeMask0 & (1 << 17)) != 0)
            writer.WritePackedIntDelta(PlayerIdplayerId, baseline.PlayerIdplayerId, compressionModel);
        if ((changeMask0 & (1 << 18)) != 0)
            writer.WritePackedIntDelta(PlayerViewviewRotationRate, baseline.PlayerViewviewRotationRate, compressionModel);
        if ((changeMask0 & (1 << 19)) != 0)
            writer.WritePackedIntDelta(PlayerViewpitch, baseline.PlayerViewpitch, compressionModel);
        if ((changeMask0 & (1 << 20)) != 0)
            writer.WritePackedIntDelta(PlayerViewyaw, baseline.PlayerViewyaw, compressionModel);
        if ((changeMask0 & (1 << 21)) != 0)
        {
            writer.WritePackedIntDelta(RotationValueX, baseline.RotationValueX, compressionModel);
            writer.WritePackedIntDelta(RotationValueY, baseline.RotationValueY, compressionModel);
            writer.WritePackedIntDelta(RotationValueZ, baseline.RotationValueZ, compressionModel);
            writer.WritePackedIntDelta(RotationValueW, baseline.RotationValueW, compressionModel);
        }
        if ((changeMask0 & (1 << 22)) != 0)
        {
            writer.WritePackedIntDelta(TranslationValueX, baseline.TranslationValueX, compressionModel);
            writer.WritePackedIntDelta(TranslationValueY, baseline.TranslationValueY, compressionModel);
            writer.WritePackedIntDelta(TranslationValueZ, baseline.TranslationValueZ, compressionModel);
        }
        if ((changeMask0 & (1 << 23)) != 0)
        {
            writer.WritePackedIntDelta(Child0RotationValueX, baseline.Child0RotationValueX, compressionModel);
            writer.WritePackedIntDelta(Child0RotationValueY, baseline.Child0RotationValueY, compressionModel);
            writer.WritePackedIntDelta(Child0RotationValueZ, baseline.Child0RotationValueZ, compressionModel);
            writer.WritePackedIntDelta(Child0RotationValueW, baseline.Child0RotationValueW, compressionModel);
        }
        if ((changeMask0 & (1 << 24)) != 0)
        {
            writer.WritePackedIntDelta(Child0TranslationValueX, baseline.Child0TranslationValueX, compressionModel);
            writer.WritePackedIntDelta(Child0TranslationValueY, baseline.Child0TranslationValueY, compressionModel);
            writer.WritePackedIntDelta(Child0TranslationValueZ, baseline.Child0TranslationValueZ, compressionModel);
        }
        if ((changeMask0 & (1 << 25)) != 0)
        {
            writer.WritePackedIntDelta(Child1RotationValueX, baseline.Child1RotationValueX, compressionModel);
            writer.WritePackedIntDelta(Child1RotationValueY, baseline.Child1RotationValueY, compressionModel);
            writer.WritePackedIntDelta(Child1RotationValueZ, baseline.Child1RotationValueZ, compressionModel);
            writer.WritePackedIntDelta(Child1RotationValueW, baseline.Child1RotationValueW, compressionModel);
        }
        if ((changeMask0 & (1 << 26)) != 0)
        {
            writer.WritePackedIntDelta(Child1TranslationValueX, baseline.Child1TranslationValueX, compressionModel);
            writer.WritePackedIntDelta(Child1TranslationValueY, baseline.Child1TranslationValueY, compressionModel);
            writer.WritePackedIntDelta(Child1TranslationValueZ, baseline.Child1TranslationValueZ, compressionModel);
        }
    }

    public void Deserialize(uint tick, ref TestCharacterSnapshotData baseline, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        changeMask0 = reader.ReadPackedUIntDelta(baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
        {
            KCCGravitygravityAccelerationX = reader.ReadPackedIntDelta(baseline.KCCGravitygravityAccelerationX, compressionModel);
            KCCGravitygravityAccelerationY = reader.ReadPackedIntDelta(baseline.KCCGravitygravityAccelerationY, compressionModel);
            KCCGravitygravityAccelerationZ = reader.ReadPackedIntDelta(baseline.KCCGravitygravityAccelerationZ, compressionModel);
        }
        else
        {
            KCCGravitygravityAccelerationX = baseline.KCCGravitygravityAccelerationX;
            KCCGravitygravityAccelerationY = baseline.KCCGravitygravityAccelerationY;
            KCCGravitygravityAccelerationZ = baseline.KCCGravitygravityAccelerationZ;
        }
        if ((changeMask0 & (1 << 1)) != 0)
            KCCGroundedmaxWalkAngle = reader.ReadPackedIntDelta(baseline.KCCGroundedmaxWalkAngle, compressionModel);
        else
            KCCGroundedmaxWalkAngle = baseline.KCCGroundedmaxWalkAngle;
        if ((changeMask0 & (1 << 2)) != 0)
            KCCGroundedgroundCheckDistance = reader.ReadPackedIntDelta(baseline.KCCGroundedgroundCheckDistance, compressionModel);
        else
            KCCGroundedgroundCheckDistance = baseline.KCCGroundedgroundCheckDistance;
        if ((changeMask0 & (1 << 3)) != 0)
            KCCJumpingjumpForce = reader.ReadPackedIntDelta(baseline.KCCJumpingjumpForce, compressionModel);
        else
            KCCJumpingjumpForce = baseline.KCCJumpingjumpForce;
        if ((changeMask0 & (1 << 4)) != 0)
            KCCJumpingattemptingJump = reader.ReadPackedUIntDelta(baseline.KCCJumpingattemptingJump, compressionModel);
        else
            KCCJumpingattemptingJump = baseline.KCCJumpingattemptingJump;
        if ((changeMask0 & (1 << 5)) != 0)
            KCCMovementSettingsmoveSpeed = reader.ReadPackedIntDelta(baseline.KCCMovementSettingsmoveSpeed, compressionModel);
        else
            KCCMovementSettingsmoveSpeed = baseline.KCCMovementSettingsmoveSpeed;
        if ((changeMask0 & (1 << 6)) != 0)
            KCCMovementSettingssprintMultiplier = reader.ReadPackedIntDelta(baseline.KCCMovementSettingssprintMultiplier, compressionModel);
        else
            KCCMovementSettingssprintMultiplier = baseline.KCCMovementSettingssprintMultiplier;
        if ((changeMask0 & (1 << 7)) != 0)
            KCCMovementSettingsmoveMaxBounces = reader.ReadPackedIntDelta(baseline.KCCMovementSettingsmoveMaxBounces, compressionModel);
        else
            KCCMovementSettingsmoveMaxBounces = baseline.KCCMovementSettingsmoveMaxBounces;
        if ((changeMask0 & (1 << 8)) != 0)
            KCCMovementSettingsmoveAnglePower = reader.ReadPackedIntDelta(baseline.KCCMovementSettingsmoveAnglePower, compressionModel);
        else
            KCCMovementSettingsmoveAnglePower = baseline.KCCMovementSettingsmoveAnglePower;
        if ((changeMask0 & (1 << 9)) != 0)
            KCCMovementSettingsmovePushPower = reader.ReadPackedIntDelta(baseline.KCCMovementSettingsmovePushPower, compressionModel);
        else
            KCCMovementSettingsmovePushPower = baseline.KCCMovementSettingsmovePushPower;
        if ((changeMask0 & (1 << 10)) != 0)
            KCCMovementSettingsmovePushDecay = reader.ReadPackedIntDelta(baseline.KCCMovementSettingsmovePushDecay, compressionModel);
        else
            KCCMovementSettingsmovePushDecay = baseline.KCCMovementSettingsmovePushDecay;
        if ((changeMask0 & (1 << 11)) != 0)
            KCCMovementSettingsfallMaxBounces = reader.ReadPackedIntDelta(baseline.KCCMovementSettingsfallMaxBounces, compressionModel);
        else
            KCCMovementSettingsfallMaxBounces = baseline.KCCMovementSettingsfallMaxBounces;
        if ((changeMask0 & (1 << 12)) != 0)
            KCCMovementSettingsfallPushPower = reader.ReadPackedIntDelta(baseline.KCCMovementSettingsfallPushPower, compressionModel);
        else
            KCCMovementSettingsfallPushPower = baseline.KCCMovementSettingsfallPushPower;
        if ((changeMask0 & (1 << 13)) != 0)
            KCCMovementSettingsfallAnglePower = reader.ReadPackedIntDelta(baseline.KCCMovementSettingsfallAnglePower, compressionModel);
        else
            KCCMovementSettingsfallAnglePower = baseline.KCCMovementSettingsfallAnglePower;
        if ((changeMask0 & (1 << 14)) != 0)
            KCCMovementSettingsfallPushDecay = reader.ReadPackedIntDelta(baseline.KCCMovementSettingsfallPushDecay, compressionModel);
        else
            KCCMovementSettingsfallPushDecay = baseline.KCCMovementSettingsfallPushDecay;
        if ((changeMask0 & (1 << 15)) != 0)
        {
            KCCVelocityplayerVelocityX = reader.ReadPackedIntDelta(baseline.KCCVelocityplayerVelocityX, compressionModel);
            KCCVelocityplayerVelocityY = reader.ReadPackedIntDelta(baseline.KCCVelocityplayerVelocityY, compressionModel);
            KCCVelocityplayerVelocityZ = reader.ReadPackedIntDelta(baseline.KCCVelocityplayerVelocityZ, compressionModel);
        }
        else
        {
            KCCVelocityplayerVelocityX = baseline.KCCVelocityplayerVelocityX;
            KCCVelocityplayerVelocityY = baseline.KCCVelocityplayerVelocityY;
            KCCVelocityplayerVelocityZ = baseline.KCCVelocityplayerVelocityZ;
        }
        if ((changeMask0 & (1 << 16)) != 0)
        {
            KCCVelocityworldVelocityX = reader.ReadPackedIntDelta(baseline.KCCVelocityworldVelocityX, compressionModel);
            KCCVelocityworldVelocityY = reader.ReadPackedIntDelta(baseline.KCCVelocityworldVelocityY, compressionModel);
            KCCVelocityworldVelocityZ = reader.ReadPackedIntDelta(baseline.KCCVelocityworldVelocityZ, compressionModel);
        }
        else
        {
            KCCVelocityworldVelocityX = baseline.KCCVelocityworldVelocityX;
            KCCVelocityworldVelocityY = baseline.KCCVelocityworldVelocityY;
            KCCVelocityworldVelocityZ = baseline.KCCVelocityworldVelocityZ;
        }
        if ((changeMask0 & (1 << 17)) != 0)
            PlayerIdplayerId = reader.ReadPackedIntDelta(baseline.PlayerIdplayerId, compressionModel);
        else
            PlayerIdplayerId = baseline.PlayerIdplayerId;
        if ((changeMask0 & (1 << 18)) != 0)
            PlayerViewviewRotationRate = reader.ReadPackedIntDelta(baseline.PlayerViewviewRotationRate, compressionModel);
        else
            PlayerViewviewRotationRate = baseline.PlayerViewviewRotationRate;
        if ((changeMask0 & (1 << 19)) != 0)
            PlayerViewpitch = reader.ReadPackedIntDelta(baseline.PlayerViewpitch, compressionModel);
        else
            PlayerViewpitch = baseline.PlayerViewpitch;
        if ((changeMask0 & (1 << 20)) != 0)
            PlayerViewyaw = reader.ReadPackedIntDelta(baseline.PlayerViewyaw, compressionModel);
        else
            PlayerViewyaw = baseline.PlayerViewyaw;
        if ((changeMask0 & (1 << 21)) != 0)
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
        if ((changeMask0 & (1 << 22)) != 0)
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
        if ((changeMask0 & (1 << 23)) != 0)
        {
            Child0RotationValueX = reader.ReadPackedIntDelta(baseline.Child0RotationValueX, compressionModel);
            Child0RotationValueY = reader.ReadPackedIntDelta(baseline.Child0RotationValueY, compressionModel);
            Child0RotationValueZ = reader.ReadPackedIntDelta(baseline.Child0RotationValueZ, compressionModel);
            Child0RotationValueW = reader.ReadPackedIntDelta(baseline.Child0RotationValueW, compressionModel);
        }
        else
        {
            Child0RotationValueX = baseline.Child0RotationValueX;
            Child0RotationValueY = baseline.Child0RotationValueY;
            Child0RotationValueZ = baseline.Child0RotationValueZ;
            Child0RotationValueW = baseline.Child0RotationValueW;
        }
        if ((changeMask0 & (1 << 24)) != 0)
        {
            Child0TranslationValueX = reader.ReadPackedIntDelta(baseline.Child0TranslationValueX, compressionModel);
            Child0TranslationValueY = reader.ReadPackedIntDelta(baseline.Child0TranslationValueY, compressionModel);
            Child0TranslationValueZ = reader.ReadPackedIntDelta(baseline.Child0TranslationValueZ, compressionModel);
        }
        else
        {
            Child0TranslationValueX = baseline.Child0TranslationValueX;
            Child0TranslationValueY = baseline.Child0TranslationValueY;
            Child0TranslationValueZ = baseline.Child0TranslationValueZ;
        }
        if ((changeMask0 & (1 << 25)) != 0)
        {
            Child1RotationValueX = reader.ReadPackedIntDelta(baseline.Child1RotationValueX, compressionModel);
            Child1RotationValueY = reader.ReadPackedIntDelta(baseline.Child1RotationValueY, compressionModel);
            Child1RotationValueZ = reader.ReadPackedIntDelta(baseline.Child1RotationValueZ, compressionModel);
            Child1RotationValueW = reader.ReadPackedIntDelta(baseline.Child1RotationValueW, compressionModel);
        }
        else
        {
            Child1RotationValueX = baseline.Child1RotationValueX;
            Child1RotationValueY = baseline.Child1RotationValueY;
            Child1RotationValueZ = baseline.Child1RotationValueZ;
            Child1RotationValueW = baseline.Child1RotationValueW;
        }
        if ((changeMask0 & (1 << 26)) != 0)
        {
            Child1TranslationValueX = reader.ReadPackedIntDelta(baseline.Child1TranslationValueX, compressionModel);
            Child1TranslationValueY = reader.ReadPackedIntDelta(baseline.Child1TranslationValueY, compressionModel);
            Child1TranslationValueZ = reader.ReadPackedIntDelta(baseline.Child1TranslationValueZ, compressionModel);
        }
        else
        {
            Child1TranslationValueX = baseline.Child1TranslationValueX;
            Child1TranslationValueY = baseline.Child1TranslationValueY;
            Child1TranslationValueZ = baseline.Child1TranslationValueZ;
        }
    }
    public void Interpolate(ref TestCharacterSnapshotData target, float factor)
    {
        SetKCCGravitygravityAcceleration(math.lerp(GetKCCGravitygravityAcceleration(), target.GetKCCGravitygravityAcceleration(), factor));
        SetKCCGroundedmaxWalkAngle(math.lerp(GetKCCGroundedmaxWalkAngle(), target.GetKCCGroundedmaxWalkAngle(), factor));
        SetKCCGroundedgroundCheckDistance(math.lerp(GetKCCGroundedgroundCheckDistance(), target.GetKCCGroundedgroundCheckDistance(), factor));
        SetKCCJumpingjumpForce(math.lerp(GetKCCJumpingjumpForce(), target.GetKCCJumpingjumpForce(), factor));
        SetKCCMovementSettingsmoveSpeed(math.lerp(GetKCCMovementSettingsmoveSpeed(), target.GetKCCMovementSettingsmoveSpeed(), factor));
        SetKCCMovementSettingssprintMultiplier(math.lerp(GetKCCMovementSettingssprintMultiplier(), target.GetKCCMovementSettingssprintMultiplier(), factor));
        SetKCCMovementSettingsmoveAnglePower(math.lerp(GetKCCMovementSettingsmoveAnglePower(), target.GetKCCMovementSettingsmoveAnglePower(), factor));
        SetKCCMovementSettingsmovePushPower(math.lerp(GetKCCMovementSettingsmovePushPower(), target.GetKCCMovementSettingsmovePushPower(), factor));
        SetKCCMovementSettingsmovePushDecay(math.lerp(GetKCCMovementSettingsmovePushDecay(), target.GetKCCMovementSettingsmovePushDecay(), factor));
        SetKCCMovementSettingsfallPushPower(math.lerp(GetKCCMovementSettingsfallPushPower(), target.GetKCCMovementSettingsfallPushPower(), factor));
        SetKCCMovementSettingsfallAnglePower(math.lerp(GetKCCMovementSettingsfallAnglePower(), target.GetKCCMovementSettingsfallAnglePower(), factor));
        SetKCCMovementSettingsfallPushDecay(math.lerp(GetKCCMovementSettingsfallPushDecay(), target.GetKCCMovementSettingsfallPushDecay(), factor));
        SetKCCVelocityplayerVelocity(math.lerp(GetKCCVelocityplayerVelocity(), target.GetKCCVelocityplayerVelocity(), factor));
        SetKCCVelocityworldVelocity(math.lerp(GetKCCVelocityworldVelocity(), target.GetKCCVelocityworldVelocity(), factor));
        SetPlayerViewviewRotationRate(math.lerp(GetPlayerViewviewRotationRate(), target.GetPlayerViewviewRotationRate(), factor));
        SetPlayerViewpitch(math.lerp(GetPlayerViewpitch(), target.GetPlayerViewpitch(), factor));
        SetPlayerViewyaw(math.lerp(GetPlayerViewyaw(), target.GetPlayerViewyaw(), factor));
        SetRotationValue(math.slerp(GetRotationValue(), target.GetRotationValue(), factor));
        SetTranslationValue(math.lerp(GetTranslationValue(), target.GetTranslationValue(), factor));
        SetChild0RotationValue(math.slerp(GetChild0RotationValue(), target.GetChild0RotationValue(), factor));
        SetChild0TranslationValue(math.lerp(GetChild0TranslationValue(), target.GetChild0TranslationValue(), factor));
        SetChild1RotationValue(math.slerp(GetChild1RotationValue(), target.GetChild1RotationValue(), factor));
        SetChild1TranslationValue(math.lerp(GetChild1TranslationValue(), target.GetChild1TranslationValue(), factor));
    }
}
