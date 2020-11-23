using Moq;
using NUnit.Framework;
using PropHunt.Client.Systems;
using PropHunt.InputManagement;
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace PropHunt.EditMode.Tests.Client
{
    /// <summary>
    /// Sample player input system tests to verify player input and forward data
    /// </summary>
    [TestFixture]
    class SamplePlayerInputSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Sample player input system
        /// </summary>
        private SamplePlayerInput samplePlayerInputSystem;

        /// <summary>
        /// Mock for player input data
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        /// <summary>
        /// Entity representing the player
        /// </summary>
        private Entity playerEntity;

        /// <summary>
        /// Mocked input state manager
        /// </summary>
        private Mock<IInputStateController> inputStateControllerMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.samplePlayerInputSystem = base.World.CreateSystem<SamplePlayerInput>();
            this.unityServiceMock = new Mock<IUnityService>();
            this.samplePlayerInputSystem.unityService = this.unityServiceMock.Object;
            base.World.CreateSystem<ClientSimulationSystemGroup>();
            this.inputStateControllerMock = new Mock<IInputStateController>();
            this.inputStateControllerMock.Setup(m => m.GetCurrentState()).Returns(LockedInputState.ALLOW);
            MenuManagerSystem.Controller = this.inputStateControllerMock.Object;

            Entity networkId = base.m_Manager.CreateEntity(typeof(NetworkIdComponent));
            base.m_Manager.SetComponentData<NetworkIdComponent>(
                networkId,
                new NetworkIdComponent
                {
                    Value = 0
                });
            Entity commandTarget = base.m_Manager.CreateEntity(typeof(CommandTargetComponent));
            // Create a 'player' entity
            this.playerEntity = CreatePlayerEntity(0);
            // Update to 'initialize' the player input state
            this.samplePlayerInputSystem.Update();
        }

        /// <summary>
        /// Create a sample player entity for attaching input
        /// </summary>
        /// <param name="playerId">Integer to identify the network id of the player</param>
        /// <returns>Entity representing the player</returns>
        private Entity CreatePlayerEntity(int playerId)
        {
            Entity playerEntity = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<PlayerId>(
                playerEntity,
                new PlayerId
                {
                    playerId = playerId
                });
            base.m_Manager.AddComponentData<PlayerView>(
                playerEntity,
                new PlayerView
                {
                    pitch = 0,
                    yaw = 0,
                    viewRotationRate = 1,
                    maxPitch = 90,
                    minPitch = -90,
                });
            base.m_Manager.AddComponentData<Rotation>(
                playerEntity,
                new Rotation
                {
                    Value = quaternion.EulerXYZ(0, 0, 0)
                });
            return playerEntity;
        }

        /// <summary>
        /// Verify no input generation when there is not a network ID Singleton.
        /// </summary>
        [Test]
        public void VerifyNoInputWithoutPlayerId()
        {
            // Remove the command target and network id singletons
            // verify that the input manager is not invoked
            base.m_Manager.DestroyEntity(this.samplePlayerInputSystem.GetSingletonEntity<NetworkIdComponent>());
            base.m_Manager.DestroyEntity(this.samplePlayerInputSystem.GetSingletonEntity<CommandTargetComponent>());

            // Update the system and verify that the input manager is not invoked
            this.samplePlayerInputSystem.Update();
            Assert.IsTrue(this.unityServiceMock.Invocations.Count == 0);
        }

        /// <summary>
        /// Verify that input is created with correct values
        /// </summary>
        [Test]
        public void VerifyInputCreation()
        {
            // Setup default inputs
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);
            this.unityServiceMock.Setup(e => e.GetAxis("Mouse Y")).Returns(1);
            this.unityServiceMock.Setup(e => e.GetAxis("Mouse X")).Returns(1);
            this.unityServiceMock.Setup(e => e.GetAxis("Horizontal")).Returns(1);
            this.unityServiceMock.Setup(e => e.GetAxis("Vertical")).Returns(1);
            this.unityServiceMock.Setup(e => e.GetButtonDown("Interact")).Returns(true);
            this.unityServiceMock.Setup(e => e.GetButton("Jump")).Returns(true);
            this.unityServiceMock.Setup(e => e.GetButton("Sprint")).Returns(true);

            // Update the system and verify that the input manager is invoked
            this.samplePlayerInputSystem.Update();
            Assert.IsTrue(this.unityServiceMock.Invocations.Count > 0);
            // Verify that a buffer size is greater than zero and that the 
            var localInput = this.samplePlayerInputSystem.GetSingleton<CommandTargetComponent>().targetEntity;
            Assert.IsTrue(base.m_Manager.GetBuffer<PlayerInput>(localInput).Length > 0);
        }

        /// <summary>
        /// Verify that this can filter between the different entities
        /// </summary>
        [Test]
        public void VerifyMultipleEntities()
        {
            // Create a second player with ID 2
            CreatePlayerEntity(2);

            // Setup default inputs
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);
            this.unityServiceMock.Setup(e => e.GetAxis("Mouse Y")).Returns(1);
            this.unityServiceMock.Setup(e => e.GetAxis("Mouse X")).Returns(1);
            this.unityServiceMock.Setup(e => e.GetAxis("Horizontal")).Returns(1);
            this.unityServiceMock.Setup(e => e.GetAxis("Vertical")).Returns(1);
            this.unityServiceMock.Setup(e => e.GetButtonDown("Interact")).Returns(true);
            this.unityServiceMock.Setup(e => e.GetButton("Jump")).Returns(true);
            this.unityServiceMock.Setup(e => e.GetButton("Sprint")).Returns(true);

            // Update the system and verify that the input manager is invoked
            this.samplePlayerInputSystem.Update();
            Assert.IsTrue(this.unityServiceMock.Invocations.Count > 0);
            // Verify that a buffer size is greater than zero and that the 
            var localInput = this.samplePlayerInputSystem.GetSingleton<CommandTargetComponent>().targetEntity;
            Assert.IsTrue(base.m_Manager.GetBuffer<PlayerInput>(localInput).Length > 0);
        }

        /// <summary>
        /// Verify bounding of pitch between min and max values when out of bounds
        /// </summary>
        [Test]
        public void VerifyCorrectWithPitchOutOfBounds()
        {
            // Setup default inputs
            this.unityServiceMock.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);
            this.unityServiceMock.Setup(e => e.GetAxis("Mouse Y")).Returns(-100);
            this.unityServiceMock.Setup(e => e.GetAxis("Mouse X")).Returns(1);
            this.unityServiceMock.Setup(e => e.GetAxis("Horizontal")).Returns(1);
            this.unityServiceMock.Setup(e => e.GetAxis("Vertical")).Returns(1);
            this.unityServiceMock.Setup(e => e.GetButtonDown("Interact")).Returns(true);
            this.unityServiceMock.Setup(e => e.GetButton("Jump")).Returns(true);
            this.unityServiceMock.Setup(e => e.GetButton("Sprint")).Returns(true);

            // Update the system and verify that the input manager is not invoked
            this.samplePlayerInputSystem.Update();
            Assert.IsTrue(this.unityServiceMock.Invocations.Count > 0);
            // Verify that a buffer size is zero and that the 
            var localInput = this.samplePlayerInputSystem.GetSingleton<CommandTargetComponent>().targetEntity;
            Assert.IsTrue(base.m_Manager.GetBuffer<PlayerInput>(localInput).Length > 0);

            // Check to ensure the target pitch is not greater than the maximum pitch value
            PlayerView pvData = m_Manager.GetComponentData<PlayerView>(this.playerEntity);
            var playerInputSample = base.m_Manager.GetBuffer<PlayerInput>(localInput)[0];
            Assert.IsTrue(playerInputSample.targetPitch == pvData.maxPitch);

            // Repeat for a large negative pitch value
            this.unityServiceMock.Setup(e => e.GetAxis("Mouse Y")).Returns(500);

            // Update the system and verify that the input manager is not invoked
            this.samplePlayerInputSystem.Update();
            Assert.IsTrue(this.unityServiceMock.Invocations.Count > 0);
            // Verify that a buffer size is zero and that the 
            localInput = this.samplePlayerInputSystem.GetSingleton<CommandTargetComponent>().targetEntity;
            Assert.IsTrue(base.m_Manager.GetBuffer<PlayerInput>(localInput).Length > 0);

            // Check to ensure the target pitch is not greater than the maximum pitch value
            playerInputSample = base.m_Manager.GetBuffer<PlayerInput>(localInput)[0];
            Assert.IsTrue(playerInputSample.targetPitch == pvData.minPitch);
        }

        /// <summary>
        /// Verify performance when updating with locked movement state
        /// </summary>
        [Test]
        public void UpdateWithLockedMovementState()
        {
            this.inputStateControllerMock.Setup(m => m.GetCurrentState()).Returns(LockedInputState.DENY);

            // Update the system and verify that the input manager is not invoked
            this.samplePlayerInputSystem.Update();
            // Only delta time should have been invoked
            Assert.IsTrue(this.unityServiceMock.Invocations.Count == 1);
            // Verify that a buffer size is greater than zero and that the 
            var localInput = this.samplePlayerInputSystem.GetSingleton<CommandTargetComponent>().targetEntity;
            Assert.IsTrue(base.m_Manager.GetBuffer<PlayerInput>(localInput).Length > 0);

            // Check to ensure the target pitch is not greater than the maximum pitch value
            var playerInputSample = base.m_Manager.GetBuffer<PlayerInput>(localInput)[0];
            Assert.IsTrue(playerInputSample.horizMove == 0);
            Assert.IsTrue(playerInputSample.vertMove == 0);
            Assert.IsTrue(playerInputSample.interact == 0);
            Assert.IsTrue(playerInputSample.jump == 0);
            Assert.IsTrue(playerInputSample.sprint == 0);
        }
    }
}