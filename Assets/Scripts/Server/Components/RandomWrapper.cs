using Unity.Entities;
using Unity.Mathematics;

namespace PropHunt.Server.Components
{
    /// <summary>
    /// Wrapper for random state of an object
    /// </summary>
    [GenerateAuthoringComponent]
    public struct RandomWrapper : IComponentData
    {
        public Random random;
    }
}