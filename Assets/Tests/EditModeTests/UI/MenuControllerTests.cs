using NUnit.Framework;
using PropHunt.Client.Systems;
using PropHunt.UI;
using Unity.Entities.Tests;
using UnityEngine;

namespace PropHunt.EditMode.Tests.UI
{
    /// <summary>
    /// Tests for Menu Controller Tests
    /// </summary>
    [TestFixture]
    public class MenuControllerTests : ECSTestsFixture
    {
        /// <summary>
        /// Object to hold menu controller
        /// </summary>
        private GameObject menuControllerObject;

        /// <summary>
        /// Menu controller object
        /// </summary>
        private MenuController menuController;

        /// <summary>
        /// Mocked changing UIEvents
        /// </summary>
        private UIChangeEvents uiChangeEvents;

        /// <summary>
        /// Current screen
        /// </summary>
        private string currentScreen;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.menuControllerObject = new GameObject();
            this.menuController = this.menuControllerObject.AddComponent<MenuController>();

            this.uiChangeEvents = new UIChangeEvents();
            UIManager.UIEvents = this.uiChangeEvents;

            // Listen to requested screen change events
            UIManager.UIEvents.RequestScreenChange += (object source, RequestScreenChangeEventArgs args) =>
            {
                this.currentScreen = args.newScreen;
            };
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            // Cleanup game object
            GameObject.DestroyImmediate(this.menuControllerObject);
        }

        [Test]
        public void SetScreenTests()
        {
            GameObject holderObject = new GameObject();
            holderObject.name = "Helloworld";
            this.menuController.SetScreen(holderObject);
            Assert.IsTrue(this.currentScreen == "Helloworld");

            this.menuController.SetScreen("NewScreen");
            Assert.IsTrue(this.currentScreen == "NewScreen");

            GameObject.DestroyImmediate(holderObject);
        }
    }
}