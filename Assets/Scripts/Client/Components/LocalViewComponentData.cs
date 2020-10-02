using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace PropHunt.Client.Components
{
    /// <summary>
    /// Player view component data, holds the attitude of the player's
    /// view to match a camera to.
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct LocalView : IComponentData
    {
        /// <summary>
        /// Current pitch of the player's view in degrees
        /// </summary>
        public float pitch;

        /// <summary>
        /// Current yaw of the player's view in degrees
        /// </summary>
        public float yaw;
    }

}