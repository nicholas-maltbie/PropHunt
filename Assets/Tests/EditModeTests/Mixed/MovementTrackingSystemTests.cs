using NUnit.Framework;
using PropHunt.Mixed.Systems;
using PropHunt.Mixed.Components;
using PropHunt.Tests.Utils;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Transforms;
using Unity.Mathematics;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class MovementTrackingSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for movement tracking
        /// </summary>
        private MovementTrackingSystem movementTracking;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup camera follow system
            this.movementTracking = World.CreateSystem<MovementTrackingSystem>();
        }

        /// <summary>
        /// Verify the updated state of a movement tracking object
        /// </summary>
        [Test]
        public void TestMovementChanges()
        {
            // Create a test object
            Entity tracked = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<MovementTracking>(tracked);
            base.m_Manager.AddComponentData<Translation>(tracked, new Translation { Value = new float3(0, 0, 0) });
            base.m_Manager.AddComponentData<Rotation>(tracked, new Rotation { Value = quaternion.EulerXYZ(new float3(0, 0, 0)) });
            // Initial update for initializing state
            // Update state using the system
            this.movementTracking.Update();
            // Verify initial object state
            MovementTracking trackComponent;
            trackComponent = base.m_Manager.GetComponentData<MovementTracking>(tracked);
            Assert.IsTrue(TestUtils.WithinErrorRange(MovementTracking.GetDisplacementAtPoint(trackComponent, float3.zero), float3.zero));

            // Verify change in state when object moves
            base.m_Manager.SetComponentData<Translation>(tracked, new Translation { Value = new float3(0, 1, 0) });
            base.m_Manager.SetComponentData<Rotation>(tracked, new Rotation { Value = quaternion.EulerXYZ(new float3(0, 0, 0)) });

            // Update state using the system
            this.movementTracking.Update();

            // Verify new displacement values
            trackComponent = base.m_Manager.GetComponentData<MovementTracking>(tracked);
            UnityEngine.Debug.Log(trackComponent.Displacement + " " + trackComponent.position + " " + trackComponent.previousPosition);
            Assert.IsTrue(TestUtils.WithinErrorRange(MovementTracking.GetDisplacementAtPoint(trackComponent, float3.zero), new float3(0, 1, 0)));

            // Verify change in state when object rotates
            base.m_Manager.SetComponentData<Translation>(tracked, new Translation { Value = new float3(0, 1, 0) });
            base.m_Manager.SetComponentData<Rotation>(tracked, new Rotation { Value = quaternion.EulerXYZ(new float3(0, -math.PI / 2, 0)) });

            // Update state using the system
            this.movementTracking.Update();

            // Verify new displacement values
            trackComponent = base.m_Manager.GetComponentData<MovementTracking>(tracked);
            Assert.IsTrue(TestUtils.WithinErrorRange(MovementTracking.GetDisplacementAtPoint(trackComponent, float3.zero), new float3(0, 0, 0)));
            Assert.IsTrue(TestUtils.WithinErrorRange(MovementTracking.GetDisplacementAtPoint(trackComponent, new float3(1, 0, 0)), new float3(-1, 0, 1)));

            // Verify change in state when object rotates and is displaced
            base.m_Manager.SetComponentData<Translation>(tracked, new Translation { Value = new float3(0, 0, 0) });
            base.m_Manager.SetComponentData<Rotation>(tracked, new Rotation { Value = quaternion.EulerXYZ(new float3(0, 0, 0)) });

            // Update state using the system
            this.movementTracking.Update();

            // Verify new displacement values
            trackComponent = base.m_Manager.GetComponentData<MovementTracking>(tracked);
            Assert.IsTrue(TestUtils.WithinErrorRange(MovementTracking.GetDisplacementAtPoint(trackComponent, float3.zero), new float3(0, -1, 0)));
            Assert.IsTrue(TestUtils.WithinErrorRange(MovementTracking.GetDisplacementAtPoint(trackComponent, new float3(1, 0, 0)), new float3(-1, -1, -1)));

            // Update state using the system
            // Verify no change with no movement
            this.movementTracking.Update();
            trackComponent = base.m_Manager.GetComponentData<MovementTracking>(tracked);
            Assert.IsTrue(TestUtils.WithinErrorRange(MovementTracking.GetDisplacementAtPoint(trackComponent, float3.zero), float3.zero));
            Assert.IsTrue(TestUtils.WithinErrorRange(MovementTracking.GetDisplacementAtPoint(trackComponent, new float3(1, 0, 0)), float3.zero));
        }
    }
}