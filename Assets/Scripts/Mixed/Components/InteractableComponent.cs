using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Object that can be 'interacted' with by players or other agents
    /// </summary>
    [GenerateAuthoringComponent]
    [GhostComponent]
    public struct Interactable : IComponentData
    {
        /// <summary>
        /// Is the object interacted with this frame
        /// </summary>
        public bool interacted;

        /// <summary>
        /// Was this object interacted with the previous frame
        /// </summary>
        public bool previousInteracted;

        /// <summary>
        /// Did an entity just start interacting with this object this frame
        /// </summary>
        public bool InteractStart => this.interacted && !this.previousInteracted;

        /// <summary>
        /// Did an entity just stop interacting with this object this frame
        /// </summary>
        public bool InteractStop => !this.interacted && this.previousInteracted;
    }
}