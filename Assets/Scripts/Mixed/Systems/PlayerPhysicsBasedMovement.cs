
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace PropHunt.Mixed.Systems
{

    /// <summary>
    /// Player movement system that moves player controlled objects from
    /// a given player's input. Moves a physics body attached to a
    /// character based on input with some respect for physics.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateAfter(typeof(PlayerRotationSystem))]
    public class PlayerPhysicsBasedMovement : ComponentSystem
    {
        /// <summary>
        /// Checks if a character is grounded. Uses a sphere cast to
        /// draw a line and check if the character is touching the ground.
        /// </summary>
        /// <param name="translation">Location to start cast</param>
        /// <param name="maxAngle">maximum angle that the character can walk at</param>
        /// <param name="groundCheckDistance">Distance to cast sphere</param>
        /// <returns></returns>
        private unsafe bool ISGrounded(Translation translation, float maxAngle, float groundCheckDistance = 0.1f)
        {
            BuildPhysicsWorld physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
            CollisionWorld collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

            var filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = ~((uint) (1)),
                GroupIndex = 1
            };

            BlobAssetReference<Unity.Physics.Collider> sphereCollider = Unity.Physics.SphereCollider.Create(
                new SphereGeometry() {
                    Center = new float3(0, 0.5f, 0),
                    Radius = 0.5f
                }, filter);

            var from = translation.Value;
            var to = translation.Value - new float3(0f, groundCheckDistance, 0f);
            var input = new ColliderCastInput()
            {
                End = to,
                Start = from,
                Collider = (Unity.Physics.Collider*)sphereCollider.GetUnsafePtr()
            };

            Unity.Physics.ColliderCastHit hit = new Unity.Physics.ColliderCastHit();

            if(collisionWorld.CastCollider(input, out hit)) {
                float angleBetween = math.abs(math.acos(math.dot(math.normalizesafe(hit.SurfaceNormal), new float3(0, 1, 0))));
                angleBetween = math.degrees(angleBetween);
                return angleBetween <= maxAngle;
            }
            
            return false;
        }

        protected override void OnUpdate()
        {
            var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
            var tick = group.PredictingTick;
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((DynamicBuffer<PlayerInput> inputBuffer,
                ref PredictedGhostComponent prediction,
                ref PlayerMovement settings, ref PhysicsVelocity pv,
                ref Translation trans, ref Rotation rot) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                {
                    return;
                }

                PlayerInput input;
                inputBuffer.GetDataAtTick(tick, out input);

                // Rotate movement vector around current attitude (only care about horizontal)
                float3 inputVector = new float3(input.horizMove, 0, input.vertMove);
                // Don't allow the total movement to be more than the 1x max move speed
                float3 direction = inputVector / math.max(math.length(inputVector), 1);

                float speedMultiplier = input.IsSprinting ? settings.SprintSpeed : settings.moveSpeed;

                quaternion horizPlaneView = rot.Value;

                // Make movement vector based on player input
                float3 movementVelocity = math.mul(horizPlaneView, direction) * speedMultiplier;

                // Preserve current vertical movement
                movementVelocity.y = pv.Linear.y;

                // Check if is grounded for jumping
                bool grounded = ISGrounded(trans, settings.maxWalkAngle, settings.groundCheckDistance);
                if (grounded && input.IsJumping)
                {
                    movementVelocity.y = settings.jumpForce;
                }

                pv.Angular = float3.zero;
                pv.Linear = movementVelocity;
            });
        }

    }

}
