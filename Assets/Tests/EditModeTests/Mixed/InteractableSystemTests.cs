using NUnit.Framework;
using PropHunt.Mixed.Components;
using PropHunt.Mixed.Systems;
using Unity.Entities;
using Unity.Entities.Tests;

namespace PropHunt.EditMode.Tests.Mixed
{
    /// <summary>
    /// Tests for the interactable reset system
    /// </summary>
    [TestFixture]
    public class InteractableResetStateTests : ECSTestsFixture
    {
        public InteractableResetState interactableResetSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup kcc input system
            this.interactableResetSystem = World.CreateSystem<InteractableResetState>();
        }

        [Test]
        public void VerifyInteractableStateReset()
        {
            Entity notInteractedWith = base.m_Manager.CreateEntity();
            Entity interactedWith = base.m_Manager.CreateEntity();
            Entity manyInteractedWith = base.m_Manager.CreateEntity();

            base.m_Manager.AddComponentData<Interactable>(notInteractedWith, new Interactable { 
                previousInteracted = false, interacted = false
            });
            base.m_Manager.AddComponentData<Interactable>(interactedWith, new Interactable { 
                previousInteracted = false, interacted = true
            });
            base.m_Manager.AddComponentData<Interactable>(manyInteractedWith, new Interactable { 
                previousInteracted = true, interacted = true
            });

            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(notInteractedWith).InteractStart == false);
            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(notInteractedWith).InteractStop == false);

            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(interactedWith).InteractStart == true);
            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(interactedWith).InteractStop == false);

            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(manyInteractedWith).InteractStart == false);
            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(manyInteractedWith).InteractStop == false);

            this.interactableResetSystem.Update();

            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(notInteractedWith).InteractStart == false);
            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(notInteractedWith).InteractStop == false);

            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(interactedWith).InteractStart == false);
            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(interactedWith).InteractStop == true);

            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(manyInteractedWith).InteractStart == false);
            Assert.IsTrue(base.m_Manager.GetComponentData<Interactable>(manyInteractedWith).InteractStop == true);
        }
    }
}