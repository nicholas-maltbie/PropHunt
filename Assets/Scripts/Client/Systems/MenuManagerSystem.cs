using PropHunt.Client.Components;
using PropHunt.Constants;
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
    /// Interface for getting the current status of the input state
    /// </summary>
    public interface IInputStateController
    {
        /// <summary>
        /// Get the current input state
        /// </summary>
        /// <returns></returns>
        LockedInputState GetCurrentState();

        /// <summary>
        /// Set the state of the input controller
        /// </summary>
        /// <param name="newState">New input state for controller</param>
        void SetCurrentState(LockedInputState newState);
    }

    /// <summary>
    /// Implementation of input state controller for filtering player input
    /// </summary>
    public class InputStateController : IInputStateController
    {
        /// <summary>
        /// Current movement state of the player
        /// </summary>
        public LockedInputState MovementState { get; set; }

        /// <inheritdoc/>
        public LockedInputState GetCurrentState() => MovementState;

        /// <inheritdoc/>
        public void SetCurrentState(LockedInputState newState) => MovementState = newState;
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
        /// Current input state controller
        /// </summary>
        public static IInputStateController Controller;

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

            // Setup default unity service if none is provided
            if (this.unityService == null)
            {
                this.unityService = new UnityService();
            }
            // Setup default controller if none is provided
            if (MenuManagerSystem.Controller == null)
            {
                MenuManagerSystem.Controller = new InputStateController()
                {
                    MovementState = LockedInputState.ALLOW
                };
            }
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
                MenuManagerSystem.Controller.SetCurrentState(LockedInputState.ALLOW);
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
                    if (MenuManagerSystem.Controller.GetCurrentState() == LockedInputState.ALLOW)
                    {
                        MenuManagerSystem.Controller.SetCurrentState(LockedInputState.DENY);
                        UIManager.RequestNewScreen(this, MenuScreenNames.InGameMenuScreen);
                    }
                    else if (MenuManagerSystem.Controller.GetCurrentState() == LockedInputState.DENY)
                    {
                        MenuManagerSystem.Controller.SetCurrentState(LockedInputState.ALLOW);
                        UIManager.RequestNewScreen(this, MenuScreenNames.HUDScreen);
                    }
                }

                // Set cursor visibility based on current user input
                if (MenuManagerSystem.Controller.GetCurrentState() == LockedInputState.ALLOW)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                }
                else if (MenuManagerSystem.Controller.GetCurrentState() == LockedInputState.DENY)
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