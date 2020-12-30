using NUnit.Framework;
using PropHunt.Utils;
using Unity.Entities;
using Unity.Entities.Tests;

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