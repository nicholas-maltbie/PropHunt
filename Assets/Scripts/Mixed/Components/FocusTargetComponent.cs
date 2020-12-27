using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Object that can act when a user 'focuses' (looks at) this object
    /// </summary>
    [GenerateAuthoringComponent]
    [GhostComponent]
    public struct FocusTarget : IComponentData
    {
        /// <summary>
        /// Is the player looking at the object
        /// </summary>
        public bool isFocused;
    }
}