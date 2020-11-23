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
    public class MovingPlatformSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// System for Moving Platform
        /// </summary>
        private MovingPlatformSystem movingPlatformSystem;

        /// <summary>
        /// Mock for unity inputs like delta time and player input
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup camera follow system
            this.movingPlatformSystem = World.CreateSystem<MovingPlatformSystem>();

            this.unityServiceMock = new Mock<IUnityService>();
            this.movingPlatformSystem.unityService = unityServiceMock.Object;
        }

        /// <summary>
        /// Test platform moves between targets at correct speed
        /// </summary>
        [Test]
        public void MovePlatform()
        {
            // Create a moving platform with one target
            Entity movingPlatform = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<Translation>(movingPlatform, new Translation { Value = float3.zero });
            base.m_Manager.AddComponentData<MovingPlatform>(movingPlatform, new MovingPlatform
            {
                speed = 1.0f,
                loopMethod = PlatformLooping.CYCLE,
                current = 0,
                direction = 1,
            });
            base.m_Manager.AddBuffer<MovingPlatformTarget>(movingPlatform);
            DynamicBuffer<MovingPlatformTarget> targets = base.m_Manager.GetBuffer<MovingPlatformTarget>(movingPlatform);
            targets.Add(new MovingPlatformTarget { target = new float3(10, 0, 0) });

            // Setup mocked delta time of one second
            this.unityServiceMock.Setup(m => m.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

            // Update the moving platform to move the moving platform by speed * delta time
            this.movingPlatformSystem.Update();

            // Assert that the new position data is now (1, 0, 0)
            Assert.IsTrue(TestUtils.WithinErrorRange(base.m_Manager.GetComponentData<Translation>(movingPlatform).Value, new float3(1, 0, 0)));

            // Update the moving platform to move the moving platform by speed * delta time
            this.movingPlatformSystem.Update();

            // Assert that the new position data is now (2, 0, 0)
            Assert.IsTrue(TestUtils.WithinErrorRange(base.m_Manager.GetComponentData<Translation>(movingPlatform).Value, new float3(2, 0, 0)));
        }

        /// <summary>
        /// Test to ensure platform will update to the next platform with cycle mode
        /// </summary>
        [Test]
        public void UpdatePlatformTargetReverse()
        {
            // Create a moving platform with one target
            Entity movingPlatform = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<Translation>(movingPlatform, new Translation { Value = new float3(1, 0, 0) });
            base.m_Manager.AddComponentData<MovingPlatform>(movingPlatform, new MovingPlatform
            {
                speed = 1.0f,
                loopMethod = PlatformLooping.REVERSE,
                current = 2,
                direction = 1,
            });
            base.m_Manager.AddBuffer<MovingPlatformTarget>(movingPlatform);
            DynamicBuffer<MovingPlatformTarget> targets = base.m_Manager.GetBuffer<MovingPlatformTarget>(movingPlatform);
            targets.Add(new MovingPlatformTarget { target = new float3(0, 0, 0) }); // target [0]
            targets.Add(new MovingPlatformTarget { target = new float3(1, 0, 0) }); // target [1]
            targets.Add(new MovingPlatformTarget { target = new float3(2, 0, 0) }); // target [2]

            // Setup mocked delta time of one second
            this.unityServiceMock.Setup(m => m.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

            // The platform will pass beyond target [2] so when reverse is setup it shoudl loop back to having target
            // platform be target [1]

            // Update the moving platform to move the moving platform by speed * delta time
            // Ensure target changes to 1
            this.movingPlatformSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<MovingPlatform>(movingPlatform).current == 1);
            Assert.IsTrue(base.m_Manager.GetComponentData<MovingPlatform>(movingPlatform).direction == -1);

            // Ensure target changes back to 0
            this.movingPlatformSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<MovingPlatform>(movingPlatform).current == 0);
            Assert.IsTrue(base.m_Manager.GetComponentData<MovingPlatform>(movingPlatform).direction == -1);

            // Ensure target changes back to 1
            this.movingPlatformSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<MovingPlatform>(movingPlatform).current == 1);
            Assert.IsTrue(base.m_Manager.GetComponentData<MovingPlatform>(movingPlatform).direction == 1);
        }

        /// <summary>
        /// Test to ensure platform will update to the next platform with cycle mode
        /// </summary>
        [Test]
        public void UpdatePlatformTargetCycle()
        {
            // Create a moving platform with one target
            Entity movingPlatform = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<Translation>(movingPlatform, new Translation { Value = new float3(1, 0, 0) });
            base.m_Manager.AddComponentData<MovingPlatform>(movingPlatform, new MovingPlatform
            {
                speed = 1f,
                loopMethod = PlatformLooping.CYCLE,
                current = 2,
                direction = 1,
            });
            base.m_Manager.AddBuffer<MovingPlatformTarget>(movingPlatform);
            DynamicBuffer<MovingPlatformTarget> targets = base.m_Manager.GetBuffer<MovingPlatformTarget>(movingPlatform);
            targets.Add(new MovingPlatformTarget { target = new float3(0, 0, 0) }); // target [0]
            targets.Add(new MovingPlatformTarget { target = new float3(1, 0, 0) }); // target [1]
            targets.Add(new MovingPlatformTarget { target = new float3(2, 0, 0) }); // target [2]

            // Setup mocked delta time of one second
            this.unityServiceMock.Setup(m => m.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);

            // The platform will pass beyond target [2] so when cycle is setup it shoudl loop back to having target
            // platform be target [0]

            // Update the moving platform to move the moving platform by speed * delta time
            this.movingPlatformSystem.Update();
            Assert.IsTrue(base.m_Manager.GetComponentData<MovingPlatform>(movingPlatform).current == 0);

            // Ensure target changes to 1
            // Will take two seconds to get back to original location
            this.unityServiceMock.Setup(m => m.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(2.0f);
            this.movingPlatformSystem.Update();
            UnityEngine.Debug.Log(base.m_Manager.GetComponentData<MovingPlatform>(movingPlatform).current);
            UnityEngine.Debug.Log(base.m_Manager.GetComponentData<Translation>(movingPlatform).Value);
            Assert.IsTrue(base.m_Manager.GetComponentData<MovingPlatform>(movingPlatform).current == 1);


            // Ensure target updates back to 2
            // Will involve slighlty more movement
            this.unityServiceMock.Setup(m => m.GetDeltaTime(It.IsAny<Unity.Core.TimeData>())).Returns(1.0f);
            this.movingPlatformSystem.Update();
            UnityEngine.Debug.Log(base.m_Manager.GetComponentData<MovingPlatform>(movingPlatform).current);
            UnityEngine.Debug.Log(base.m_Manager.GetComponentData<Translation>(movingPlatform).Value);
            Assert.IsTrue(base.m_Manager.GetComponentData<MovingPlatform>(movingPlatform).current == 2);
        }
    }
}