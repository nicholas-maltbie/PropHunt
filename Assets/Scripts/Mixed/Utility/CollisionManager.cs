using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;

namespace PropHunt.Mixed.Utilities
{
    /// <summary>
    /// Structure for containg the collsion manager behaviour and making it testable
    /// </summary>
    public interface ICollisionManager
    {
        void SetCollisionManager(CollisionWorld collisionWorld);


        bool CastCollider<T>(ColliderCastInput input, ref T collector) where T : struct, ICollector<ColliderCastHit>;
        bool CastCollider(ColliderCastInput input, out ColliderCastHit closestHit);
        bool CastCollider(ColliderCastInput input, ref NativeList<ColliderCastHit> allHits);
        bool CastCollider(ColliderCastInput input);
    }

    public class CollisionManager : ICollisionManager
    {
        public CollisionWorld collsionWorld;

        public void SetCollisionManager(CollisionWorld collisionWorld)
        {
            this.collsionWorld = collisionWorld;
        }

        public bool CastCollider<T>(ColliderCastInput input, ref T collector) where T : struct, ICollector<ColliderCastHit>
        {
            return collsionWorld.CastCollider<T>(input, ref collector);
        }

        public bool CastCollider(ColliderCastInput input, out ColliderCastHit closestHit)
        {
            return collsionWorld.CastCollider(input, out closestHit);
        }

        public bool CastCollider(ColliderCastInput input, ref NativeList<ColliderCastHit> allHits)
        {
            return collsionWorld.CastCollider(input, ref allHits);
        }

        public bool CastCollider(ColliderCastInput input)
        {
            return collsionWorld.CastCollider(input);
        }
    }
}
