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

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class RotatingPlatformSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for Moving Platform
        /// </summary>
        private RotatingPlatformSystem rotatingPlatformSystem;

        /// <summary>
        /// Mock for unity inputs like delta time and player input
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup camera follow system
            this.rotatingPlatformSystem = World.CreateSystem<RotatingPlatformSystem>();

            this.unityServiceMock = new Mock<IUnityService>();
            this.rotatingPlatformSystem.unityService = unityServiceMock.Object;
        }

        /// <summary>
        /// Test platform rotates between targets at correct speed
        /// </summary>
        [Test]
        public void RotatePlatform()
        {
            // Create a moving platform with one target
            Entity rotatingPlatform = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<Rotation>(rotatingPlatform, new Rotation { Value = quaternion.Euler(0, 0, 0) });
            base.m_Manager.AddComponentData<RotatingPlatform>(rotatingPlatform, new RotatingPlatform
            {
                speed = 15f,
                currentAngle = float3.zero,
                loopMethod = PlatformLooping.CYCLE,
                current = 0,
                direction = 1,
            });
            base.m_Manager.AddBuffer<RotatingPlatformTarget>(rotatingPlatform);
            DynamicBuffer<RotatingPlatformTarget> targets = base.m_Manager.GetBuffer<RotatingPlatformTarget>(rotatingPlatform);
            targets.Add(new RotatingPlatformTarget { target = new float3(30, 0, 0) } );

            // Setup mocked delta time of one second
            this.unityServiceMock.Setup(m => m.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

            // Update the moving platform to move the moving platform by speed * delta time
            this.rotatingPlatformSystem.Update();

            // Assert that the new rotation data is now (15, 0, 0)
            Assert.IsTrue(TestUtils.WithinErrorRange(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).currentAngle, new float3(15, 0, 0)));

            // Update the moving platform to move the moving platform by speed * delta time
            this.rotatingPlatformSystem.Update();

            // Assert that the new rotation data is now (30, 0, 0)
            Assert.IsTrue(TestUtils.WithinErrorRange(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).currentAngle, new float3(30, 0, 0)));
        }

        /// <summary>
        /// Test to ensure platform will update to the next platform with cycle mode
        /// </summary>
        [Test]
        public void UpdateRotationTargetReverse()
        {
            // Create a moving platform with one target
            Entity rotatingPlatform = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<Rotation>(rotatingPlatform, new Rotation { Value = quaternion.Euler(0, 30, 0) });
            base.m_Manager.AddComponentData<RotatingPlatform>(rotatingPlatform, new RotatingPlatform
            {
                speed = 30f,
                currentAngle = float3.zero,
                loopMethod = PlatformLooping.REVERSE,
                current = 1,
                direction = 1,
            });
            base.m_Manager.AddBuffer<RotatingPlatformTarget>(rotatingPlatform);
            DynamicBuffer<RotatingPlatformTarget> targets = base.m_Manager.GetBuffer<RotatingPlatformTarget>(rotatingPlatform);
            targets.Add(new RotatingPlatformTarget { target = new float3( 0, 0, 0) } ); // target [0]
            targets.Add(new RotatingPlatformTarget { target = new float3(30, 0, 0) } ); // target [1]
            targets.Add(new RotatingPlatformTarget { target = new float3(60, 0, 0) } ); // target [2]

            // Setup mocked delta time of one second
            this.unityServiceMock.Setup(m => m.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

            // The platform will pass beyond target [2] so when reverse is setup it shoudl loop back to having target
            // platform be target [1]

            // Update the moving platform to move the moving platform by speed * delta time
            // Ensure target changes to 1
            this.rotatingPlatformSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).current == 2);
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).direction == 1);

            // Ensure target changes back to 0
            this.rotatingPlatformSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).current == 1);
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).direction == -1);

            // Ensure target changes back to 1
            this.rotatingPlatformSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).current == 0);
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).direction == -1);
        }

        /// <summary>
        /// Test to ensure platform will update to the next platform with cycle mode
        /// </summary>
        [Test]
        public void UpdateRotationTargetCycle()
        {
            // Create a moving platform with one target
            Entity rotatingPlatform = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<Rotation>(rotatingPlatform, new Rotation { Value = quaternion.Euler(0, 30, 0) });
            base.m_Manager.AddComponentData<RotatingPlatform>(rotatingPlatform, new RotatingPlatform
            {
                speed = 30f,
                currentAngle = float3.zero,
                loopMethod = PlatformLooping.CYCLE,
                current = 1,
                direction = 1,
            });
            base.m_Manager.AddBuffer<RotatingPlatformTarget>(rotatingPlatform);
            DynamicBuffer<RotatingPlatformTarget> targets = base.m_Manager.GetBuffer<RotatingPlatformTarget>(rotatingPlatform);
            targets.Add(new RotatingPlatformTarget { target = new float3( 0, 0, 0) } ); // target [0]
            targets.Add(new RotatingPlatformTarget { target = new float3(30, 0, 0) } ); // target [1]
            targets.Add(new RotatingPlatformTarget { target = new float3(60, 0, 0) } ); // target [2]

            // Setup mocked delta time of one second
            this.unityServiceMock.Setup(m => m.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

            // The platform will pass beyond target [2] so when reverse is setup it shoudl loop back to having target
            // platform be target [1]

            // Update the moving platform to move the moving platform by speed * delta time
            // Ensure target changes to 2
            this.rotatingPlatformSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).current == 2);
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).direction == 1);

            // Ensure target changes back to 0
            this.rotatingPlatformSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).current == 0);
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).direction == 1);

            // Ensure target changes back to 1
            this.unityServiceMock.Setup(m => m.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(2.0f);
            this.rotatingPlatformSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).current == 1);
            Assert.IsTrue(base.m_Manager.GetComponentData<RotatingPlatform>(rotatingPlatform).direction == 1);
        }
    }
}