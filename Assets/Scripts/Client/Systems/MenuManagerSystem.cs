using PropHunt.Client.Components;
using PropHunt.InputManagement;
using PropHunt.UI;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace PropHunt.Client.Systems
{
    /// <summary>
    /// Enum to control locking the current player input. This could be for things such
    /// as a pause menu or other options.
    /// </summary>
    public enum LockedInputState { ALLOW, DENY };

    /// <summary>
    /// Database for menu screen names
    /// </summary>
    public static class MenuScreenNames
    {
        /// <summary>
        /// Main menu default screen for when leaving the game
        /// </summary>
        public static readonly string MainMenuScreen = "MainMenuScreen";

        /// <summary>
        /// Screen with in game menu options
        /// </summary>
        public static readonly string InGameMenuScreen = "InGameMenu";

        /// <summary>
        /// Screen with in game heads up display name
        /// </summary>
        public static readonly string HUDScreen = "InGameHUD";
    }

    /// <summary>
    /// System to manage the available menu and
    /// switch between different menus.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    [UpdateAfter(typeof(ConnectionSystem))]
    public class MenuManagerSystem : ComponentSystem
    {
        /// <summary>
        /// Public get method for the current movement state of the player
        /// </summary>
        public static LockedInputState MovementState { get; private set; }

        /// <summary>
        /// Was this previously connected last frame
        /// </summary>
        private bool previouslyConnected;

        /// <summary>
        /// Unity service for getting inputs
        /// </summary>
        public IUnityService unityService;

        protected override void OnCreate()
        {
            UIManager.SetupUIEvents();
            // Add listener to screen change events
            UIManager.UIEvents.ScreenChangeOccur += this.HandleScreenChangeEvent;
            RequireSingletonForUpdate<ConnectionComponent>();

            // Setup default unity service if non is provided
            if (this.unityService == null)
            {
                this.unityService = new UnityService();
            }

            MenuManagerSystem.MovementState = LockedInputState.ALLOW;
        }

        protected override void OnDestroy()
        {
            UIManager.UIEvents.ScreenChangeOccur -= this.HandleScreenChangeEvent;
        }

        /// <summary>
        /// Handle events when the screen changes to ensure movement state
        /// is unlocked while the in game heads up display is shown.
        /// </summary>
        private void HandleScreenChangeEvent(object sender, ScreenChangeEventArgs eventArgs)
        {
            if (eventArgs.newScreen == MenuScreenNames.HUDScreen)
            {
                MenuManagerSystem.MovementState = LockedInputState.ALLOW;
            }
        }

        protected override void OnUpdate()
        {
            var connectionSingleton = GetSingleton<ConnectionComponent>();
            bool currentlyConnected = connectionSingleton.isConnected;

            // This should only act while in game
            if (!currentlyConnected)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Parse user input to check if the toggle menu button has been pressed
                if (unityService.GetButtonDown("Cancel"))
                {
                    if (MenuManagerSystem.MovementState == LockedInputState.ALLOW)
                    {
                        MenuManagerSystem.MovementState = LockedInputState.DENY;
                        UIManager.RequestNewScreen(this, MenuScreenNames.InGameMenuScreen);
                    }
                    else if (MovementState == LockedInputState.DENY)
                    {
                        MenuManagerSystem.MovementState = LockedInputState.ALLOW;
                        UIManager.RequestNewScreen(this, MenuScreenNames.HUDScreen);
                    }
                }

                // Set cursor visibility based on current user input
                if (MenuManagerSystem.MovementState == LockedInputState.ALLOW)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                }
                else if (MenuManagerSystem.MovementState == LockedInputState.DENY)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            // If this changed from connected to not connected, will open main menu
            if (!currentlyConnected && this.previouslyConnected)
            {
                UIManager.RequestNewScreen(this, MenuScreenNames.MainMenuScreen);
            }
            // if this changed from not connected to connected, open in game menu
            if (currentlyConnected && !this.previouslyConnected)
            {
                UIManager.RequestNewScreen(this, MenuScreenNames.HUDScreen);
            }

            this.previouslyConnected = currentlyConnected;
        }
    }
}