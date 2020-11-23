using NUnit.Framework;
using PropHunt.Mixed.Systems;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Transforms;
using Unity.Mathematics;
using Moq;
using PropHunt.InputManagement;
using PropHunt.Tests.Utils;
using Unity.Physics;
using Unity.NetCode;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class PushForceCleanupSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for push cleanup system
        /// </summary>
        private PushForceCleanup pushForceCleanupSystem;

        /// <summary>
        /// Buffer system for updating objects
        /// </summary>
        private EndSimulationEntityCommandBufferSystem bufferSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.pushForceCleanupSystem = base.World.CreateSystem<PushForceCleanup>();
            this.bufferSystem = base.World.CreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        [Test]
        public void VerifyRemovalOfPushComponents()
        {
            // Verify that a buffer is removed from a pushed entity
            Entity pushed = base.m_Manager.CreateEntity();

            base.m_Manager.AddBuffer<PushForce>(pushed);
            DynamicBuffer<PushForce> pushBuffer = base.m_Manager.GetBuffer<PushForce>(pushed);
            pushBuffer.Add(new PushForce { point = new float3(0, 0, 0), force = new float3(1, 0, 0) });

            Assert.IsTrue(base.m_Manager.GetBuffer<PushForce>(pushed).Length > 0);

            this.pushForceCleanupSystem.Update();
            this.bufferSystem.Update();

            Assert.IsTrue(base.m_Manager.GetBuffer<PushForce>(pushed).Length == 0);

            // Assert that it still works when there is no buffer
            this.pushForceCleanupSystem.Update();
            this.bufferSystem.Update();
            Assert.IsTrue(base.m_Manager.GetBuffer<PushForce>(pushed).Length == 0);
        }
    }

    [TestFixture]
    public class PushForceApplySystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for push force apply system
        /// </summary>
        private PushForceApply pushForceApplySystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup camera follow system
            this.pushForceApplySystem = World.CreateSystem<PushForceApply>();
        }

        [Test]
        public void NoUpdateWithoutServer()
        {
            Entity pushed = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<PhysicsVelocity>(pushed);
            base.m_Manager.AddComponent<PhysicsMass>(pushed);
            base.m_Manager.AddComponentData<Translation>(pushed, new Translation { Value = float3.zero });
            base.m_Manager.AddComponent<Rotation>(pushed);
            base.m_Manager.AddBuffer<PushForce>(pushed);

            PhysicsVelocity startingPV = base.m_Manager.GetComponentData<PhysicsVelocity>(pushed);
            DynamicBuffer<PushForce> pushBuffer = base.m_Manager.GetBuffer<PushForce>(pushed);
            pushBuffer.Add(new PushForce { point = new float3(0, 0, 0), force = new float3(1, 0, 0) });

            this.pushForceApplySystem.Update();

            // Assert that the physics velocity is not changed
            PhysicsVelocity updatedPV = base.m_Manager.GetComponentData<PhysicsVelocity>(pushed);
            Assert.IsTrue(TestUtils.WithinErrorRange(startingPV.Linear, updatedPV.Linear));
            Assert.IsTrue(TestUtils.WithinErrorRange(startingPV.Angular, updatedPV.Angular));
        }

        private Entity CreatePushedEntity()
        {
            Entity pushed = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<PhysicsVelocity>(pushed);
            base.m_Manager.AddComponentData<PhysicsMass>(pushed, new PhysicsMass
            {
                Transform = new RigidTransform { rot = quaternion.Euler(float3.zero), pos = float3.zero },
                InverseMass = 1f,
                InverseInertia = new float3(1, 1, 1),
                AngularExpansionFactor = 1f,
                CenterOfMass = float3.zero,
                InertiaOrientation = quaternion.Euler(float3.zero)
            });
            base.m_Manager.AddComponentData<Translation>(pushed, new Translation { Value = float3.zero });
            base.m_Manager.AddComponentData<Rotation>(pushed, new Rotation { Value = quaternion.Euler(float3.zero) });
            base.m_Manager.AddBuffer<PushForce>(pushed);
            return pushed;
        }

        [Test]
        public void VerifyCorrectPushForce()
        {
            base.World.CreateSystem<ServerSimulationSystemGroup>();

            Entity pushed = CreatePushedEntity();

            DynamicBuffer<PushForce> pushBuffer = base.m_Manager.GetBuffer<PushForce>(pushed);
            pushBuffer.Add(new PushForce { point = new float3(1, 0, 0), force = new float3(1, 0, 0) });

            this.pushForceApplySystem.Update();

            // Assert that the physics velocity is not changed
            PhysicsVelocity pv = base.m_Manager.GetComponentData<PhysicsVelocity>(pushed);
            UnityEngine.Debug.Log(pv.Linear);
            Assert.IsTrue(TestUtils.WithinErrorRange(pv.Linear, new float3(1, 0, 0)));
            Assert.IsTrue(TestUtils.WithinErrorRange(pv.Angular, new float3(0, 0, 0)));
        }

        [Test]
        public void VerifyCorrectPushForceAngular()
        {
            base.World.CreateSystem<ServerSimulationSystemGroup>();

            Entity pushed = CreatePushedEntity();

            DynamicBuffer<PushForce> pushBuffer = base.m_Manager.GetBuffer<PushForce>(pushed);
            // Simulate push on the top of the object
            pushBuffer.Add(new PushForce { point = new float3(0, 1, 0), force = new float3(10, 0, 0) });

            this.pushForceApplySystem.Update();

            // Assert that there is some non zero angular and linear velocity
            PhysicsVelocity pv = base.m_Manager.GetComponentData<PhysicsVelocity>(pushed);
            Assert.IsFalse(TestUtils.WithinErrorRange(pv.Linear, float3.zero));
            Assert.IsFalse(TestUtils.WithinErrorRange(pv.Angular, float3.zero));
        }

    }
}