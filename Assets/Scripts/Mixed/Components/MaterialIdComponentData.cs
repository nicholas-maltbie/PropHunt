using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Component for identifying a material id of a shared library of materials between the client and the server.
    /// </summary>
    [GenerateAuthoringComponent]
    public struct MaterialIdComponentData : IComponentData
    {
        /// <summary>
        /// Number identifying which material the entity has referenced.
        /// </summary>
        [GhostDefaultField]
        public int materialId;
    }
}