
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
    /// a given player's input.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    [UpdateAfter(typeof(PlayerRotationSystem))]
    public class PlayerPhysicsBasedMovement : ComponentSystem
    {
        private bool ISGrounded(Translation translation, float groundCheckDistance = 0.2f)
        {
            BuildPhysicsWorld physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
            CollisionWorld collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

            var filter = new CollisionFilter()
            {
                BelongsTo = ((uint) (1)),
                CollidesWith = ~((uint) (1)),
                GroupIndex = 1
            };

            var from = translation.Value;
            var to = translation.Value - new float3(0f, groundCheckDistance, 0f);
            var input = new RaycastInput()
            {
                End = to,
                Filter = filter,
                Start = from
            };

            var hit = collisionWorld.CastRay(input);
            return hit;
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

                float speedMultiplier = input.IsSprinting ? settings.moveSpeed : settings.SprintSpeed;

                Quaternion horizPlaneView = rot.Value;

                float3 movementVelocity = math.mul(horizPlaneView, direction) * speedMultiplier;

                pv.Angular = float3.zero;

                bool grounded = ISGrounded(trans);

                Debug.Log(grounded);
                if (!grounded)
                {
                    settings.velocity += new float3(0, -settings.gravityForce, 0) * Time.DeltaTime;
                    movementVelocity += settings.velocity;
                }
                else
                {
                    settings.velocity = float3.zero;
                }

                pv.Linear = movementVelocity;
            });
        }

    }

}
