using Moq;
using NUnit.Framework;
using PropHunt.InputManagement;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Systems;
using PropHunt.Tests.Utils;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace PropHunt.EditMode.Tests.Mixed
{
    /// <summary>
    /// Tests for the Door Control System
    /// </summary>
    [TestFixture]
    public class DoorPositionUpdateSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Door Control System
        /// </summary>
        private DoorPositionUpdateSystem doorUpdateSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup kcc input system
            this.doorUpdateSystem = World.CreateSystem<DoorPositionUpdateSystem>();
        }

        [Test]
        public void VerifyDoorPositionOpened()
        {
            Entity door = base.m_Manager.CreateEntity();
            float3 openedPosition = new float3(0, 1, 0);
            float3 closedPosition = new float3(0, 3, 0);
            float3 openedRotation = new float3(0, 0, 3.14f);
            float3 closedRotation = new float3(0, 0, -3.14f);
            Door doorComponent = new Door
            {
                openedPosition = openedPosition,
                openedRotation = openedRotation,
                closedPosition = closedPosition,
                closedRotation = closedRotation,
                state = DoorState.Opened
            };
            base.m_Manager.AddComponentData<Door>(door, doorComponent);
            base.m_Manager.AddComponent<Translation>(door);
            base.m_Manager.AddComponent<Rotation>(door);

            this.doorUpdateSystem.Update();
            Assert.IsTrue(math.all(base.m_Manager.GetComponentData<Translation>(door).Value == openedPosition));
            Assert.IsTrue(math.all(base.m_Manager.GetComponentData<Rotation>(door).Value.value == quaternion.Euler(openedRotation).value));
        }

        [Test]
        public void VerifyDoorPositionClosed()
        {
            Entity door = base.m_Manager.CreateEntity();
            float3 openedPosition = new float3(0, 1, 0);
            float3 closedPosition = new float3(0, 3, 0);
            float3 openedRotation = new float3(0, 0, 3.14f);
            float3 closedRotation = new float3(0, 0, -3.14f);
            Door doorComponent = new Door
            {
                openedPosition = openedPosition,
                openedRotation = openedRotation,
                closedPosition = closedPosition,
                closedRotation = closedRotation,
                state = DoorState.Closed
            };
            base.m_Manager.AddComponentData<Door>(door, doorComponent);
            base.m_Manager.AddComponent<Translation>(door);
            base.m_Manager.AddComponent<Rotation>(door);

            this.doorUpdateSystem.Update();
            Assert.IsTrue(math.all(base.m_Manager.GetComponentData<Translation>(door).Value == closedPosition));
            Assert.IsTrue(math.all(base.m_Manager.GetComponentData<Rotation>(door).Value.value == quaternion.Euler(closedRotation).value));
        }

        [Test]
        public void VerifyDoorPositionOpening()
        {
            Entity door = base.m_Manager.CreateEntity();
            float3 openedPosition = new float3(0, 1, 0);
            float3 closedPosition = new float3(0, 3, 0);
            float3 openedRotation = new float3(0, 0, 3.14f);
            float3 closedRotation = new float3(0, 0, -3.14f);
            Door doorComponent = new Door
            {
                openedPosition = openedPosition,
                openedRotation = openedRotation,
                closedPosition = closedPosition,
                closedRotation = closedRotation,
                elapsedTransitionTime = 1.0f,
                transitionTime = 10.0f,
                state = DoorState.Opening
            };
            base.m_Manager.AddComponentData<Door>(door, doorComponent);
            base.m_Manager.AddComponent<Translation>(door);
            base.m_Manager.AddComponent<Rotation>(door);

            this.doorUpdateSystem.Update();
            // Should be 10% from closed to opened
            float3 targetPosition = doorComponent.closedPosition + (doorComponent.openedPosition - doorComponent.closedPosition) * 0.1f;
            float3 targetRotation = doorComponent.closedRotation + RotatingPlatformSystem.ShortestAngleBetween(doorComponent.openedRotation, doorComponent.closedRotation) * 0.1f;
            Assert.IsTrue(TestUtils.WithinErrorRange(base.m_Manager.GetComponentData<Translation>(door).Value, targetPosition));
        }

        [Test]
        public void VerifyDoorPositionClosing()
        {
            Entity door = base.m_Manager.CreateEntity();
            float3 openedPosition = new float3(0, 1, 0);
            float3 closedPosition = new float3(0, 3, 0);
            float3 openedRotation = new float3(0, 0, 3.14f);
            float3 closedRotation = new float3(0, 0, -3.14f);
            Door doorComponent = new Door
            {
                openedPosition = openedPosition,
                openedRotation = openedRotation,
                closedPosition = closedPosition,
                closedRotation = closedRotation,
                elapsedTransitionTime = 1.0f,
                transitionTime = 10.0f,
                state = DoorState.Closing
            };
            base.m_Manager.AddComponentData<Door>(door, doorComponent);
            base.m_Manager.AddComponent<Translation>(door);
            base.m_Manager.AddComponent<Rotation>(door);

            this.doorUpdateSystem.Update();
            // Should be 10% from closed to opened
            float3 targetPosition = doorComponent.openedPosition + (doorComponent.closedPosition - doorComponent.openedPosition) * 0.1f;
            Assert.IsTrue(TestUtils.WithinErrorRange(base.m_Manager.GetComponentData<Translation>(door).Value, targetPosition));
        }
    }

    /// <summary>
    /// Tests for the Door Control System
    /// </summary>
    [TestFixture]
    public class DoorControlSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Door Control System
        /// </summary>
        private DoorStateMachineUpdateSystem doorControlSystem;

        /// <summary>
        /// Unity service for measuring time delta
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup kcc input system
            this.doorControlSystem = World.CreateSystem<DoorStateMachineUpdateSystem>();

            // Setup mocks for system
            this.unityServiceMock = new Mock<IUnityService>();
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

            // Connect mocked variables to system
            this.doorControlSystem.unityService = this.unityServiceMock.Object;

            // Create server system
            base.World.CreateSystem<ServerSimulationSystemGroup>();
        }

        [Test]
        public void VerifyUpdateOnClient()
        {
            Entity door = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<Door>(door, new Door { });
            base.m_Manager.AddComponentData<Interactable>(door, new Interactable { });

            // Destory server system to emulate running on client
            base.World.DestroySystem(base.World.GetExistingSystem<ServerSimulationSystemGroup>());
            // Update the door control system
            this.doorControlSystem.Update();
        }

        [Test]
        public void VerifyElapsedTimeStateChanges()
        {
            Entity door = base.m_Manager.CreateEntity();
            Door doorComponent = new Door { transitionTime = 3.0f, elapsedTransitionTime = 0.0f, state = DoorState.Opening };
            base.m_Manager.AddComponentData<Door>(door, doorComponent);
            base.m_Manager.AddComponentData<Interactable>(door, new Interactable { previousInteracted = false, interacted = false });

            // Test test change to new state when the transition time is reached
            this.doorControlSystem.Update();
            UnityEngine.Debug.Log(base.m_Manager.GetComponentData<Door>(door).elapsedTransitionTime);
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).elapsedTransitionTime == 1.0f);
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).state == DoorState.Opening);
            this.doorControlSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).elapsedTransitionTime == 2.0f);
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).state == DoorState.Opening);
            this.doorControlSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).elapsedTransitionTime == 0.0f);
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).state == DoorState.Opened);
            // Repeat the process but with closing state
            base.m_Manager.AddComponentData<Door>(door, new Door { transitionTime = 3.0f, elapsedTransitionTime = 0.0f, state = DoorState.Closing });
            this.doorControlSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).elapsedTransitionTime == 1.0f);
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).state == DoorState.Closing);
            this.doorControlSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).elapsedTransitionTime == 2.0f);
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).state == DoorState.Closing);
            this.doorControlSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).state == DoorState.Closed);
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).elapsedTransitionTime == 0.0f);
        }

        [Test]
        public void VerifyInteractStateChagnes()
        {
            Entity door = base.m_Manager.CreateEntity();
            Door doorComponent = new Door { transitionTime = 10.0f, elapsedTransitionTime = 0.0f };
            base.m_Manager.AddComponentData<Door>(door, doorComponent);
            base.m_Manager.AddComponentData<Interactable>(door, new Interactable { previousInteracted = false, interacted = true });

            // Test interact while opened
            doorComponent.state = DoorState.Opened;
            base.m_Manager.SetComponentData<Door>(door, doorComponent);
            this.doorControlSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).state == DoorState.Closing);
            // Test interact while closed
            doorComponent.state = DoorState.Closed;
            base.m_Manager.SetComponentData<Door>(door, doorComponent);
            this.doorControlSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).state == DoorState.Opening);
            // Test interact while opening
            doorComponent.state = DoorState.Opening;
            doorComponent.elapsedTransitionTime = 2.0f;
            base.m_Manager.SetComponentData<Door>(door, doorComponent);
            this.doorControlSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).state == DoorState.Closing);
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).elapsedTransitionTime == (doorComponent.transitionTime - 1));
            // Test interact while closing
            doorComponent.state = DoorState.Closing;
            doorComponent.elapsedTransitionTime = 2.0f;
            base.m_Manager.SetComponentData<Door>(door, doorComponent);
            this.doorControlSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).state == DoorState.Opening);
            Assert.IsTrue(base.m_Manager.GetComponentData<Door>(door).elapsedTransitionTime == (doorComponent.transitionTime - 1));
        }
    }
}