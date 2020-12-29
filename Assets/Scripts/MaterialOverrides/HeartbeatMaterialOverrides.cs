using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace PropHunt.MaterialOverrides
{
    /// <summary>
    /// Set the highlight on for a given object
    /// </summary>
    [MaterialProperty("_EmissionColor", MaterialPropertyFormat.Float4)]
    public struct EmissionColorFloatOverride : IComponentData
    {
        public float4 Value;
    }

    /// <summary>
    /// Set the highlight on for a given object
    /// </summary>
    [MaterialProperty("_EmissionActive", MaterialPropertyFormat.Float)]
    public struct EmissionActiveFloatOverride : IComponentData
    {
        public float Value;
    }

    /// <summary>
    /// Set the Fresnel value for a highlighted object
    /// </summary>
    [MaterialProperty("_FresnelValue", MaterialPropertyFormat.Float)]
    public struct FresnelValueFloatOverride : IComponentData
    {
        public float Value;
    }

    /// <summary>
    /// Set the blinking of a highlighted object on/off
    /// </summary>
    [MaterialProperty("_HasHeartbeat", MaterialPropertyFormat.Float)]
    public struct HasHeartbeatFloatOverride : IComponentData
    {
        public float Value;
    }

    /// <summary>
    /// Set the frequency of blinking of the object
    /// </summary>
    [MaterialProperty("_HeartbeatFrequency", MaterialPropertyFormat.Float)]
    public struct HeartbeatFrequencyFloatOverride : IComponentData
    {
        public float Value;
    }
}