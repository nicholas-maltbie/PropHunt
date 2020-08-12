
using Unity.Physics;

namespace PropHunt.Mixed.Utilities
{
    /// <summary>
    /// Collider processor to handle filtering out an object to stop
    /// hitting itself.
    /// </summary>
    public struct SelfFilteringClosestHitCollector<T> : ICollector<T> where T : struct,IQueryResult
    {
        /// <summary>
        /// Can this exit early on the current hit object
        /// </summary>
        public bool EarlyOutOnFirstHit => false;

        /// <summary>
        /// Maximum fraction away that the hit object was encountered
        /// along the path of the raycast
        /// </summary>
        /// <value></value>
        public float MaxFraction { get; private set; }

        /// <summary>
        /// Number of objects hit
        /// </summary>
        public int NumHits { get; private set; }

        /// <summary>
        /// Most recent (closest hit object)
        /// </summary>
        public T ClosestHit {get; private set; }

        /// <summary>
        /// Entity index to avoid collisions with.
        /// </summary>
        private readonly int selfEntityIndex;

        private CollisionWorld collisionWorld;

        /// <summary>
        /// Creates a self filtering object collision detection
        /// </summary>
        /// <param name="entityId">Index of this entity</param>
        /// <param name="maxFraction">Maximum fraction that an object can be encountered
        /// as a portion of the current raycast draw</param>
        /// <param name="collisionWorld">World of all colliable objects</param>
        public SelfFilteringClosestHitCollector(int entityIndex, float maxFraction, CollisionWorld collisionWorld)
        {
            this.MaxFraction = maxFraction;
            this.ClosestHit = default;
            this.NumHits = 0;
            this.selfEntityIndex = entityIndex;
            this.collisionWorld = collisionWorld;
        }

        #region ICollector

        /// <inheritdoc/>
        public bool AddHit(T hit)
        {
            if (collisionWorld.Bodies[hit.RigidBodyIndex].Entity.Index == this.selfEntityIndex)
            {
                return false;
            }
            this.MaxFraction = hit.Fraction;
            this.ClosestHit = hit;
            this.NumHits = 1;
            return true;
        }

        #endregion
    }

    
}
