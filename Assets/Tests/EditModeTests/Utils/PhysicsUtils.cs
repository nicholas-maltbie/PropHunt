using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PropHunt.EditMode.Tests.Utils
{
    public static class PhysicsTestUtils
    {
        public static unsafe Entity CreateBody(
            EntityManager entityManager,
            float3 position, quaternion orientation, BlobAssetReference<Collider> collider,
            float3 linearVelocity, float3 angularVelocity, float mass, bool isDynamic
        )
        {
            ComponentType[] componentTypes = new ComponentType[isDynamic ? 7 : 4];

            componentTypes[0] = typeof(Translation);
            componentTypes[1] = typeof(Rotation);
            componentTypes[2] = typeof(LocalToWorld);
            componentTypes[3] = typeof(PhysicsCollider);
            if (isDynamic)
            {
                componentTypes[4] = typeof(PhysicsVelocity);
                componentTypes[5] = typeof(PhysicsMass);
                componentTypes[6] = typeof(PhysicsDamping);
            }
            Entity entity = entityManager.CreateEntity(componentTypes);

            entityManager.SetComponentData(entity, new Translation { Value = position });
            entityManager.SetComponentData(entity, new Rotation { Value = orientation });

            entityManager.SetComponentData(entity, new PhysicsCollider { Value = collider });

            if (isDynamic)
            {
                Collider* colliderPtr = (Collider*)collider.GetUnsafePtr();
                entityManager.SetComponentData(entity, PhysicsMass.CreateDynamic(colliderPtr->MassProperties, mass));
                // Calculate the angular velocity in local space from rotation and world angular velocity
                float3 angularVelocityLocal = math.mul(math.inverse(colliderPtr->MassProperties.MassDistribution.Transform.rot), angularVelocity);
                entityManager.SetComponentData(entity, new PhysicsVelocity()
                {
                    Linear = linearVelocity,
                    Angular = angularVelocityLocal
                });
                entityManager.SetComponentData(entity, new PhysicsDamping()
                {
                    Linear = 0.01f,
                    Angular = 0.05f
                });
            }

            return entity;
        }

        public static Entity CreateSphere(EntityManager manager, float radius, float3 position, quaternion orientation, bool isDynamic)
        {
            // Sphere with default filter and material. Add to Create() call if you want non default:
            BlobAssetReference<Unity.Physics.Collider> spCollider = Unity.Physics.SphereCollider.Create(new SphereGeometry
            {
                Center = position,
                Radius = 1.0f,
            });
            return CreateBody(manager, position, orientation, spCollider, float3.zero, float3.zero, 1.0f, isDynamic);
        }

        public static Entity CreateBox(EntityManager manager, float3 size, float3 position, float3 center, float bevelRadius, quaternion orientation, bool isDynamic)
        {
            BlobAssetReference<Unity.Physics.Collider> boxCollider = Unity.Physics.BoxCollider.Create(new BoxGeometry
            {
                Center = center,
                Orientation = orientation,
                Size = size,
                BevelRadius = bevelRadius
            });
            return CreateBody(manager, position, orientation, boxCollider, float3.zero, float3.zero, 1.0f, isDynamic);
        }
    }
}