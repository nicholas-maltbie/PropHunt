using System;
using System.Linq;
using Unity.Physics;

namespace PropHunt.Mixed.Utils
{
    public struct FilteringClosestHitCollector<T> : ICollector<T> where T : struct, IQueryResult
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
        public T ClosestHit { get; private set; }

        /// <summary>
        /// Entity index to avoid collisions with.
        /// </summary>
        private readonly int ignore;

        /// <summary>
        /// Entity index to allow collisions with
        /// </summary>
        private readonly int allow;

        private CollisionWorld collisionWorld;

        /// <summary>
        /// Creates a self filtering object collision detection
        /// </summary>
        /// <param name="ignore">index of entity to ignore hitting</param>
        /// <param name="allow">index of entity to allow to hit, leave empty to allow everything (except the ignroe)</param>
        /// <param name="maxFraction">Maximum fraction that an object can be encountered
        /// as a portion of the current raycast draw</param>
        /// <param name="collisionWorld">World of all colliable objects</param>
        public FilteringClosestHitCollector(int ignore, int allow, float maxFraction, CollisionWorld collisionWorld)
        {
            this.MaxFraction = maxFraction;
            this.ClosestHit = default;
            this.NumHits = 0;
            this.ignore = ignore;
            this.allow = allow;
            this.collisionWorld = collisionWorld;
        }

        #region ICollector

        /// <inheritdoc/>
        public bool AddHit(T hit)
        {
            int hitIndex = collisionWorld.Bodies[hit.RigidBodyIndex].Entity.Index;
            // Ignore if index is part of hit index or if
            // allowed is defined and does not contain this index
            if (ignore == hitIndex || (allow != AllowAll && allow != hitIndex))
            {
                return false;
            }
            this.MaxFraction = hit.Fraction;
            this.ClosestHit = hit;
            this.NumHits = 1;
            return true;
        }

        #endregion

        /// <summary>
        /// Empty set of integers
        /// </summary>
        public static readonly int AllowAll = -1;
    }
}