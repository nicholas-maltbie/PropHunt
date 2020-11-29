using Moq;
using NUnit.Framework;
using PropHunt.InputManagement;
using PropHunt.Mixed.Components;
using PropHunt.Server.Components;
using PropHunt.Server.Systems;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.Transforms;

namespace PropHunt.EditMode.Tests.Server
{
    [TestFixture]
    public class AutonomousKCCSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// AutonomousKCCSystem for controlling characters
        /// </summary>
        private AutonomousKCCSystem autonomousKCCSystem;

        private Mock<IUnityService> mockUnityService;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.mockUnityService = new Mock<IUnityService>();
            this.autonomousKCCSystem = base.World.CreateSystem<AutonomousKCCSystem>();
            this.autonomousKCCSystem.unityService = this.mockUnityService.Object;
        }

        public Entity SpawnAutonomousCharacter()
        {
            Entity autonomousKCC = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<KCCVelocity>(autonomousKCC);
            base.m_Manager.AddComponent<KCCJumping>(autonomousKCC);
            base.m_Manager.AddComponentData<AutonomousKCCAgent>(autonomousKCC, new AutonomousKCCAgent
            {
                minTimeDirectionChange = 3.0f,
                maxTimeDirectionChange = 5.0f,
                moveSpeed = 2.0f,
                minTimeJump = 5.0f,
                maxTimeJump = 7.0f,
                remainingTimeDirectionChange = 3.0f,
                remainingTimeJump = 3.0f,
            });
            base.m_Manager.AddComponentData<Rotation>(autonomousKCC, new Rotation { Value = quaternion.Euler(float3.zero) });
            base.m_Manager.AddComponentData<RandomWrapper>(autonomousKCC, new RandomWrapper { random = new Unity.Mathematics.Random(123) });
            return autonomousKCC;
        }

        [Test]
        public void TestNoAdjustment()
        {
            Entity autonomousCharacter = this.SpawnAutonomousCharacter();

            // Setup time to be no time passing
            float deltaTime = 1.0f;
            this.mockUnityService.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(deltaTime);

            AutonomousKCCAgent agent = base.m_Manager.GetComponentData<AutonomousKCCAgent>(autonomousCharacter);
            float startRemainingChangeDirectionTime = agent.remainingTimeDirectionChange;
            float startRemainingJumpTime = agent.remainingTimeJump;
            float3 startVelocity = base.m_Manager.GetComponentData<KCCVelocity>(autonomousCharacter).playerVelocity;

            this.autonomousKCCSystem.Update();
            // Assert that velocity has not changed and that the character is not jumping
            Assert.IsTrue(math.all(startVelocity == base.m_Manager.GetComponentData<KCCVelocity>(autonomousCharacter).playerVelocity));
            // Assert that the character is not jumping
            Assert.IsTrue(base.m_Manager.GetComponentData<KCCJumping>(autonomousCharacter).attemptingJump == false);

            // Assert that remaining jump time and change direction time have decreased by delta time
            agent = base.m_Manager.GetComponentData<AutonomousKCCAgent>(autonomousCharacter);
            Assert.IsTrue(startRemainingChangeDirectionTime - deltaTime == agent.remainingTimeDirectionChange);
            Assert.IsTrue(startRemainingJumpTime - deltaTime == agent.remainingTimeJump);
        }

        [Test]
        public void TestAutoKCCChangeDirection()
        {
            Entity autonomousCharacter = this.SpawnAutonomousCharacter();
            AutonomousKCCAgent agent = base.m_Manager.GetComponentData<AutonomousKCCAgent>(autonomousCharacter);
            agent.remainingTimeDirectionChange = 0;
            agent.remainingTimeJump = 10;
            base.m_Manager.SetComponentData<AutonomousKCCAgent>(autonomousCharacter, agent);

            // Setup time to be 1 second passing
            this.mockUnityService.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);

            float3 startVelocity = base.m_Manager.GetComponentData<KCCVelocity>(autonomousCharacter).playerVelocity;

            this.autonomousKCCSystem.Update();
            // Assert that velocity has changed and that the character is moving
            float3 endingVelocity = base.m_Manager.GetComponentData<KCCVelocity>(autonomousCharacter).playerVelocity;
            Assert.IsTrue(math.any(startVelocity != endingVelocity));
            // Assert that the ending velocity is the correct speed
            Assert.IsTrue(math.abs(math.length(endingVelocity) - agent.moveSpeed) < 0.001f);
            // Assert that the character is not jumping
            Assert.IsTrue(base.m_Manager.GetComponentData<KCCJumping>(autonomousCharacter).attemptingJump == false);
            // Assert that the new change direction time is within the correct bounds
            agent = base.m_Manager.GetComponentData<AutonomousKCCAgent>(autonomousCharacter);
            float changeDirectionTime = agent.remainingTimeDirectionChange;
            Assert.IsTrue(changeDirectionTime <= agent.maxTimeDirectionChange);
            Assert.IsTrue(changeDirectionTime >= agent.minTimeDirectionChange);
        }

        [Test]
        public void TestAutoKCCAttemptingJump()
        {
            Entity autonomousCharacter = this.SpawnAutonomousCharacter();
            AutonomousKCCAgent agent = base.m_Manager.GetComponentData<AutonomousKCCAgent>(autonomousCharacter);
            agent.remainingTimeDirectionChange = 10;
            agent.remainingTimeJump = 0;
            base.m_Manager.SetComponentData<AutonomousKCCAgent>(autonomousCharacter, agent);

            // Setup time to be 1 second passing
            this.mockUnityService.Setup(e => e.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1);

            float3 startVelocity = base.m_Manager.GetComponentData<KCCVelocity>(autonomousCharacter).playerVelocity;

            this.autonomousKCCSystem.Update();
            // Assert that velocity has not changed and that the character is not jumping
            Assert.IsTrue(math.all(startVelocity == base.m_Manager.GetComponentData<KCCVelocity>(autonomousCharacter).playerVelocity));
            // Assert that the character is jumping
            Assert.IsTrue(base.m_Manager.GetComponentData<KCCJumping>(autonomousCharacter).attemptingJump);
            agent = base.m_Manager.GetComponentData<AutonomousKCCAgent>(autonomousCharacter);
            float jumpTime = agent.remainingTimeJump;
            UnityEngine.Debug.Log(jumpTime + " | " + agent.maxTimeJump);
            Assert.IsTrue(jumpTime <= agent.maxTimeJump);
            Assert.IsTrue(jumpTime >= agent.minTimeJump);
        }
    }
}