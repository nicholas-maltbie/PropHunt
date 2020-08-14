using Unity.Entities;
using Unity.NetCode;


namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Player view component data, holds the attitude of the player's
    /// view to match a camera to.
    /// </summary>
    [GhostDefaultComponent(
        GhostDefaultComponentAttribute.Type.Server |
        GhostDefaultComponentAttribute.Type.PredictedClient)]
    public struct PlayerView : IComponentData
    {
        /// <summary>
        /// Player view speed in degrees per second
        /// </summary>
        [GhostDefaultField(100, true)]
        public float viewRotationRate;

        /// <summary>
        /// Current pitch of the player's view in degrees
        /// </summary>
        [GhostDefaultField(100, true)]
        public float pitch;
        
        /// <summary>
        /// Current yaw of the player's view in degrees
        /// </summary>
        [GhostDefaultField(100, true)]
        public float yaw;
    }

}
