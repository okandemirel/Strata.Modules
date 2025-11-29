using System;
using UnityEngine;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Interface for screen view components. All screens must implement this interface.
    /// </summary>
    public interface IScreenBody
    {
        /// <summary>
        /// Runtime data for this screen instance.
        /// </summary>
        ScreenData Data { get; set; }

        /// <summary>
        /// Callback invoked when show animation completes.
        /// </summary>
        Action<IScreenBody> OnShowAnimationComplete { get; set; }

        /// <summary>
        /// Callback invoked when hide animation completes.
        /// </summary>
        Action<IScreenBody> OnHideAnimationComplete { get; set; }

        /// <summary>
        /// The GameObject this screen is attached to.
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// The RectTransform of this screen.
        /// </summary>
        RectTransform RectTransform { get; }

        /// <summary>
        /// Shows the screen, triggering show animation if configured.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the screen, triggering hide animation if configured.
        /// </summary>
        void Hide();

        /// <summary>
        /// Called after the screen has been hidden (animation complete, before pooling).
        /// </summary>
        void OnScreenHidden();

        /// <summary>
        /// Called before the screen is set up (parented to layer).
        /// </summary>
        void BeforeSetup();

        /// <summary>
        /// Called after the screen is set up (parented to layer, RectTransform configured).
        /// </summary>
        void AfterSetup();

        /// <summary>
        /// Called when parameters are passed to the screen.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        void SetParameters(object[] parameters);

        /// <summary>
        /// Checks if the screen has a specific state flag.
        /// </summary>
        /// <param name="state">The state to check.</param>
        /// <returns>True if the screen has the state.</returns>
        bool HasState(ScreenState state);

        /// <summary>
        /// Adds a state flag to the screen.
        /// </summary>
        /// <param name="state">The state to add.</param>
        void AddState(ScreenState state);

        /// <summary>
        /// Removes a state flag from the screen.
        /// </summary>
        /// <param name="state">The state to remove.</param>
        void RemoveState(ScreenState state);
    }
}
