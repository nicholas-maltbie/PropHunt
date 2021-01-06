using NUnit.Framework;
using PropHunt.Utils;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Transforms;

namespace PropHunt.EditMode.Tests.Utils
{
    /// <summary>
    /// Tests for the modify entity utils tests
    /// </summary>
    [TestFixture]
    public class ModifyEntityUtilitiesTests : ECSTestsFixture
    {
        private struct TestSingleton : IComponentData { }

        public struct TestComponent : IComponentData
        {
            public float Value;
        }

        public class TestSystem : SystemBase
        {
            protected override void OnCreate()
            {
                base.RequireSingletonForUpdate<TestSingleton>();
            }

            protected override void OnUpdate()
            {
                var testComponentGetter = GetComponentDataFromEntity<TestComponent>(true);
                EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
                var parallelWriter = ecb.AsParallelWriter();

                Entities.WithReadOnly(testComponentGetter).ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ModifyEntityUtilities.AddOrSetData(entityInQueryIndex, entity, new TestComponent { Value = 10.0f }, testComponentGetter, parallelWriter);
                }).ScheduleParallel();
                this.Dependency.Complete();

                ecb.Playback(base.EntityManager);
                ecb.Dispose();
            }
        }

        public class TestSystemRecursive : SystemBase
        {
            protected override void OnCreate()
            {
                base.RequireSingletonForUpdate<TestSingleton>();
            }

            protected override void OnUpdate()
            {
                var testComponentGetter = GetComponentDataFromEntity<TestComponent>(true);
                var childBufferGetter = GetBufferFromEntity<Child>(true);
                EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
                var parallelWriter = ecb.AsParallelWriter();

                Entities.WithReadOnly(testComponentGetter).WithReadOnly(childBufferGetter).ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ModifyEntityUtilities.AddOrSetDataRecursive(entityInQueryIndex, entity, new TestComponent { Value = 10.0f },
                        testComponentGetter, childBufferGetter, parallelWriter);
                }).ScheduleParallel();
                this.Dependency.Complete();

                ecb.Playback(base.EntityManager);
                ecb.Dispose();
            }
        }

        [Test]
        public void TestAddOrSetRecursiveComponent()
        {
            Entity parentEntity = base.m_Manager.CreateEntity(typeof(TestComponent));
            Entity childEntity1 = base.m_Manager.CreateEntity();
            Entity childEntity2 = base.m_Manager.CreateEntity();

            base.m_Manager.CreateEntity(typeof(TestSingleton));

            base.m_Manager.AddBuffer<Child>(parentEntity);
            var childBuffer = base.m_Manager.GetBuffer<Child>(parentEntity);
            childBuffer.Add(new Child { Value = childEntity1 });
            childBuffer.Add(new Child { Value = childEntity2 });

            Assert.IsTrue(base.m_Manager.HasComponent<TestComponent>(parentEntity));
            Assert.IsFalse(base.m_Manager.HasComponent<TestComponent>(childEntity1));
            Assert.IsFalse(base.m_Manager.HasComponent<TestComponent>(childEntity2));

            TestSystemRecursive testSystemRecursive = base.World.CreateSystem<TestSystemRecursive>();
            testSystemRecursive.Update();

            Assert.IsTrue(base.m_Manager.HasComponent<TestComponent>(parentEntity));
            Assert.IsTrue(base.m_Manager.HasComponent<TestComponent>(childEntity1));
            Assert.IsTrue(base.m_Manager.HasComponent<TestComponent>(childEntity2));

            Assert.IsTrue(base.m_Manager.GetComponentData<TestComponent>(parentEntity).Value == 10.0f);
            Assert.IsTrue(base.m_Manager.GetComponentData<TestComponent>(childEntity1).Value == 10.0f);
            Assert.IsTrue(base.m_Manager.GetComponentData<TestComponent>(childEntity2).Value == 10.0f);
        }

        [Test]
        public void TestAddOrSetComponent()
        {
            Entity withComponent = base.m_Manager.CreateEntity(typeof(TestComponent));
            Entity withoutComponent = base.m_Manager.CreateEntity();

            base.m_Manager.CreateEntity(typeof(TestSingleton));

            TestSystem testSystem = base.World.CreateSystem<TestSystem>();
            testSystem.Update();

            Assert.IsTrue(base.m_Manager.HasComponent<TestComponent>(withComponent));
            Assert.IsTrue(base.m_Manager.HasComponent<TestComponent>(withoutComponent));

            Assert.IsTrue(base.m_Manager.GetComponentData<TestComponent>(withComponent).Value == 10.0f);
            Assert.IsTrue(base.m_Manager.GetComponentData<TestComponent>(withoutComponent).Value == 10.0f);
        }
    }
}