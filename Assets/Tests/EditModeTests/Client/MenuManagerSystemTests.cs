using Moq;
using NUnit.Framework;
using PropHunt.Client.Components;
using PropHunt.Client.Systems;
using PropHunt.InputManagement;
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using PropHunt.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.TestTools;

namespace PropHunt.EditMode.Tests.Client
{
    /// <summary>
    /// Tests for the menu manager
    /// </summary>
    [TestFixture]
    public class MenuManagerSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Menu manager system
        /// </summary>
        private MenuManagerSystem menuManagerSystem;

        /// <summary>
        /// Mocked changing UIEvents
        /// </summary>
        private UIChangeEvents uiChangeEvents;

        /// <summary>
        /// Currently selected screen
        /// </summary>
        private string currentScreen;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Create connection singleton
            base.m_Manager.CreateEntity(typeof(ConnectionComponent));

            this.uiChangeEvents = new UIChangeEvents();
            UIManager.UIEvents = this.uiChangeEvents;

            this.menuManagerSystem = base.World.CreateSystem<MenuManagerSystem>();

            // Listen to requested screen change events
            UIManager.UIEvents.RequestScreenChange += (object source, RequestScreenChangeEventArgs args) =>
            {
                this.currentScreen = args.newScreen;
            };
        }

        /// <summary>
        /// Verify the internal satte of the menu manager
        /// </summary>
        [Test]
        public void VerifyInitialState()
        {
            this.menuManagerSystem.Update();

            // Assert curosor is setup correctely
            Assert.IsTrue(Cursor.lockState == CursorLockMode.None);
            Assert.IsTrue(Cursor.visible);
        }

        /// <summary>
        /// Verify change in menu state on connecting to server
        /// </summary>
        [Test]
        public void VerifyChangeOnConnect()
        {
            this.menuManagerSystem.Update();
            this.menuManagerSystem.SetSingleton<ConnectionComponent>(
                new ConnectionComponent
                {
                    isConnected = true,
                });

            this.menuManagerSystem.Update();

            // Assert selected screen is HUDScreen
            Assert.IsTrue(this.currentScreen == MenuScreenNames.HUDScreen);

            // Simulate disconnect
            this.menuManagerSystem.SetSingleton<ConnectionComponent>(
                new ConnectionComponent
                {
                    isConnected = false,
                });
            this.menuManagerSystem.Update();

            // Assert selected screen is MainMenuScreen
            Assert.IsTrue(this.currentScreen == MenuScreenNames.MainMenuScreen);

            // Verify does not change when connection state does not change
            this.menuManagerSystem.Update();
            Assert.IsTrue(this.currentScreen == MenuScreenNames.MainMenuScreen);
        }

        /// <summary>
        /// Verify changing state of UIManager and MenuManager interactions
        /// </summary>
        [Test]
        public void VerifyChangeStateOnCancel()
        {
            this.menuManagerSystem.Update();
            this.menuManagerSystem.SetSingleton<ConnectionComponent>(
                new ConnectionComponent
                {
                    isConnected = true,
                });

            // Initial update to set state
            this.menuManagerSystem.Update();

            // When connected, verify toggling input state and menu screen when hitting 'cancel' button
            var previousState = MenuManagerSystem.MovementState;
            var previousScreen = this.currentScreen;

            // Mock player input
            var mockService = new Moq.Mock<IUnityService>();
            // Simulate pressing 'cance' button
            mockService.Setup(p => p.GetButtonDown("Cancel")).Returns(true);
            // Attach mock to system
            this.menuManagerSystem.unityService = mockService.Object;

            // Verify change in state
            this.menuManagerSystem.Update();

            Assert.IsFalse(previousState == MenuManagerSystem.MovementState);
            previousScreen = this.currentScreen;
            previousState = MenuManagerSystem.MovementState;

            // Verify change in state when hitting 'cancel' again with another update
            this.menuManagerSystem.Update();

            Assert.IsFalse(previousState == MenuManagerSystem.MovementState);
        }

        /// <summary>
        /// Verify that the movement state is locked when the HUD screen is used
        /// </summary>
        [Test]
        public void VerifyLockOnHUDScreen()
        {
            this.menuManagerSystem.Update();
            this.menuManagerSystem.SetSingleton<ConnectionComponent>(
                new ConnectionComponent
                {
                    isConnected = true,
                });

            // Mock player input
            var mockService = new Moq.Mock<IUnityService>();
            // Simulate pressing 'cance' button
            mockService.Setup(p => p.GetButtonDown("Cancel")).Returns(true);
            // Attach mock to system
            this.menuManagerSystem.unityService = mockService.Object;

            // Initial update to set state
            this.menuManagerSystem.Update();

            var args = new ScreenChangeEventArgs
            {
                newScreen = MenuScreenNames.MainMenuScreen,
                oldScreen = MenuScreenNames.HUDScreen
            };
            this.uiChangeEvents.ChangeScreen(null, args);

            // Verify cursor is locked
            Assert.IsTrue(MenuManagerSystem.MovementState == LockedInputState.DENY);

            // Initial update to set state
            this.menuManagerSystem.Update();

            // raise a mocked event
            // Simulate switch from main menu screen to HUD screen
            args = new ScreenChangeEventArgs
            {
                newScreen = MenuScreenNames.HUDScreen,
                oldScreen = MenuScreenNames.MainMenuScreen
            };
            this.uiChangeEvents.ChangeScreen(null, args);

            // Verify cursor is locked
            Assert.IsTrue(MenuManagerSystem.MovementState == LockedInputState.ALLOW);
        }

        [Test]
        public void VerifyStaticConstants()
        {
            Assert.IsFalse(string.IsNullOrEmpty(MenuScreenNames.MainMenuScreen));
            Assert.IsFalse(string.IsNullOrEmpty(MenuScreenNames.InGameMenuScreen));
            Assert.IsFalse(string.IsNullOrEmpty(MenuScreenNames.HUDScreen));
        }
    }
}