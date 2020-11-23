using Unity.Entities;
using Unity.NetCode;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using PropHunt.Mixed.Components;
using Unity.Physics.Extensions;

namespace PropHunt.Mixed.Systems
{
    /// <summary>
    /// System group for resolving push forces applied to dynamic objects in the scene
    /// </summary>
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    public class PushForceGroup : ComponentSystemGroup { }

    /// <summary>
    /// System to remove Push Components the frame after they are created
    /// </summary>
    [UpdateInGroup(typeof(PushForceGroup))]
    public class PushForceCleanup : SystemBase
    {
        /// <summary>
        /// Command buffer for removing push forces after they are applied
        /// </summary>
        EndSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            this.commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = this.commandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities.ForEach((
                Entity entity,
                ref DynamicBuffer<PushForce> pushForce) =>
                {
                    pushForce.Clear();
                }
            ).ScheduleParallel();

            this.Dependency.Complete();
            this.commandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }

    /// <summary>
    /// Apply pushes to objects with push component
    /// </summary>
    [UpdateInGroup(typeof(PushForceGroup))]
    [UpdateBefore(typeof(PushForceCleanup))]
    public class PushForceApply : SystemBase
    {

        protected override void OnUpdate()
        {
            // Only apply force if this is the server
            bool isServer = World.GetExistingSystem<ServerSimulationSystemGroup>() != null;

            if (isServer)
            {
                Entities.WithChangeFilter<PushForce>().ForEach((
                    ref PhysicsVelocity physicsVelocity,
                    in PhysicsMass physicsMass,
                    in Translation translation,
                    in Rotation rotation,
                    in DynamicBuffer<PushForce> pushForce) =>
                    {
                        for (int push = 0; push < pushForce.Length; push++)
                        {
                            physicsVelocity.ApplyImpulse(physicsMass, translation, rotation, pushForce[push].force, pushForce[push].point);
                        }
                    }
                ).ScheduleParallel();
            }
        }
    }
}