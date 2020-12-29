using NUnit.Framework;
using PropHunt.Client.Systems;
using PropHunt.MaterialOverrides;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;

namespace PropHunt.EditMode.Tests.Mixed
{
    /// <summary>
    /// Tests for the KCC Components
    /// </summary>
    [TestFixture]
    public class HighlightObjectsSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Highlight object system for highlighting focused objects
        /// </summary>
        private HighlightObjectsSystem highlightObjectSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.highlightObjectSystem = World.CreateSystem<HighlightObjectsSystem>();
        }

        /// <summary>
        /// Test adding and removing a highlight to an object
        /// </summary>
        [Test]
        public void TestAddHighlightToObjects()
        {
            Entity entity = base.m_Manager.CreateEntity();
            HighlightableComponent baseHighlightable = new HighlightableComponent{
                emissionColor = UnityEngine.Color.blue,
                hasHeartbeat = true,
                fresnelValue = 3.0f,
                heartbeatSpeed = 3.0f
            };

            base.m_Manager.AddComponentData<HighlightableComponent>(entity, baseHighlightable);
            base.m_Manager.AddComponentData<FocusTarget>(entity, new FocusTarget { isFocused = true });

            this.highlightObjectSystem.Update();

            Assert.IsTrue(base.m_Manager.HasComponent<EmissionActiveFloatOverride>(entity));
            Assert.IsTrue(base.m_Manager.HasComponent<EmissionColorFloatOverride>(entity));
            Assert.IsTrue(base.m_Manager.HasComponent<HasHeartbeatFloatOverride>(entity));
            Assert.IsTrue(base.m_Manager.HasComponent<HeartbeatFrequencyFloatOverride>(entity));
            Assert.IsTrue(base.m_Manager.HasComponent<FresnelValueFloatOverride>(entity));

            Assert.IsTrue(base.m_Manager.GetComponentData<EmissionActiveFloatOverride>(entity).Value == 1.0f);
            Assert.IsTrue(math.all(base.m_Manager.GetComponentData<EmissionColorFloatOverride>(entity).Value == baseHighlightable.EmissionColor));
            Assert.IsTrue(base.m_Manager.GetComponentData<HasHeartbeatFloatOverride>(entity).Value == (baseHighlightable.hasHeartbeat ? 1.0f : 0.0f));
            Assert.IsTrue(base.m_Manager.GetComponentData<HeartbeatFrequencyFloatOverride>(entity).Value == baseHighlightable.heartbeatSpeed);
            Assert.IsTrue(base.m_Manager.GetComponentData<FresnelValueFloatOverride>(entity).Value == baseHighlightable.fresnelValue);

            base.m_Manager.AddComponentData<FocusTarget>(entity, new FocusTarget { isFocused = false });

            this.highlightObjectSystem.Update();

            Assert.IsTrue(base.m_Manager.GetComponentData<EmissionActiveFloatOverride>(entity).Value == 0.0f);
        }
    }
}