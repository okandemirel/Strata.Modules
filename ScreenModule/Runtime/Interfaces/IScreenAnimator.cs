using System;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Interface for custom screen animators.
    /// Implement this to provide custom animation logic for screens.
    /// </summary>
    public interface IScreenAnimator
    {
        /// <summary>
        /// Plays the show/entrance animation for a screen.
        /// </summary>
        /// <param name="screen">The screen to animate.</param>
        /// <param name="onComplete">Callback when animation completes.</param>
        void PlayShow(IScreenBody screen, Action onComplete);

        /// <summary>
        /// Plays the hide/exit animation for a screen.
        /// </summary>
        /// <param name="screen">The screen to animate.</param>
        /// <param name="onComplete">Callback when animation completes.</param>
        void PlayHide(IScreenBody screen, Action onComplete);
    }
}
