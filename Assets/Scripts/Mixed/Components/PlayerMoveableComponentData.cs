using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Mixed.Components 
{

    /// <summary>
    /// An object that can be moved by a player.
    /// </summary>
    /// </summary>
    [GhostDefaultComponent(GhostDefaultComponentAttribute.Type.Server)]
    public class PlayerMoveable : IComponentData
    {
        /// <summary>
        /// Speed of movement in units/second.
        /// </summary>
        [GhostDefaultField]
        public float speed = 5.0f;
    }

}

