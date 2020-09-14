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
    /// System to manage the available menu and
    /// switch between different menus.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    [UpdateAfter(typeof(ConnectionSystem))]
    public class MenuManagerSystem : ComponentSystem
    {
        /// <summary>
        /// Current movement input state of the player
        /// </summary>
        private static LockedInputState movementState = LockedInputState.ALLOW;

        /// <summary>
        /// Public get method for the current movement state of the player
        /// </summary>
        public static LockedInputState MovementState => MenuManagerSystem.movementState;

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

        /// <summary>
        /// Was this previously connected last frame
        /// </summary>
        private bool previouslyConnected;

        protected override void OnCreate()
        {
            // Add listener to screen change events
            UIManager.ScreenChangeOccur += this.HandleScreenChangeEvent;
        }

        protected override void OnDestroy()
        {
            UIManager.ScreenChangeOccur -= this.HandleScreenChangeEvent;
        }

        /// <summary>
        /// Handle events when the screen changes to ensure movement state
        /// is unlocked while the in game heads up display is shown.
        /// </summary>
        private void HandleScreenChangeEvent(object sender, ScreenChangeEventArgs eventArgs)
        {
            if (eventArgs.newScreen == MenuManagerSystem.HUDScreen)
            {
                MenuManagerSystem.movementState = LockedInputState.ALLOW;
            }
        }

        protected override void OnUpdate()
        {
            bool currentlyConnected = ConnectionSystem.IsConnected;

            // This should only act while in game
            if (!currentlyConnected)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Parse user input to check if the toggle menu button has been pressed
                if (Input.GetButtonDown("Cancel"))
                {
                    if (MenuManagerSystem.movementState == LockedInputState.ALLOW)
                    {
                        MenuManagerSystem.movementState = LockedInputState.DENY;
                        UIManager.RequestNewScreen(this, MenuManagerSystem.InGameMenuScreen);
                    }
                    else if (movementState == LockedInputState.DENY)
                    {
                        MenuManagerSystem.movementState = LockedInputState.ALLOW;
                        UIManager.RequestNewScreen(this, MenuManagerSystem.HUDScreen);
                    }
                }

                // Set cursor visibility based on current user input
                if (MenuManagerSystem.movementState == LockedInputState.ALLOW)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                }
                else if (MenuManagerSystem.movementState == LockedInputState.DENY)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            // If this changed from connected to not connected, will open main menu
            if (!currentlyConnected && this.previouslyConnected)
            {
                UIManager.RequestNewScreen(this, MenuManagerSystem.MainMenuScreen);
            }
            // if this changed from not connected to connected, open in game menu
            if (currentlyConnected && !this.previouslyConnected)
            {
                UIManager.RequestNewScreen(this, MenuManagerSystem.HUDScreen);
            }

            this.previouslyConnected = currentlyConnected;
        }
    }
}