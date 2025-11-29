namespace Strada.Modules.Screen
{
    /// <summary>
    /// Extension methods for screen-related types.
    /// </summary>
    public static class ScreenExtensions
    {
        #region ScreenState Extensions

        /// <summary>
        /// Adds a state flag to the screen data.
        /// </summary>
        /// <param name="data">The screen data.</param>
        /// <param name="state">The state to add.</param>
        public static void AddState(this ScreenData data, ScreenState state)
        {
            if (data != null)
            {
                data.State |= state;
            }
        }

        /// <summary>
        /// Removes a state flag from the screen data.
        /// </summary>
        /// <param name="data">The screen data.</param>
        /// <param name="state">The state to remove.</param>
        public static void RemoveState(this ScreenData data, ScreenState state)
        {
            if (data != null)
            {
                data.State &= ~state;
            }
        }

        /// <summary>
        /// Checks if the screen data has a specific state.
        /// </summary>
        /// <param name="data">The screen data.</param>
        /// <param name="state">The state to check.</param>
        /// <returns>True if the state is set.</returns>
        public static bool HasState(this ScreenData data, ScreenState state)
        {
            return data != null && (data.State & state) == state;
        }

        /// <summary>
        /// Toggles a state flag on the screen data.
        /// </summary>
        /// <param name="data">The screen data.</param>
        /// <param name="state">The state to toggle.</param>
        public static void ToggleState(this ScreenData data, ScreenState state)
        {
            if (data != null)
            {
                data.State ^= state;
            }
        }

        /// <summary>
        /// Sets a state flag based on a condition.
        /// </summary>
        /// <param name="data">The screen data.</param>
        /// <param name="state">The state to set.</param>
        /// <param name="value">True to add, false to remove.</param>
        public static void SetState(this ScreenData data, ScreenState state, bool value)
        {
            if (value)
            {
                data.AddState(state);
            }
            else
            {
                data.RemoveState(state);
            }
        }

        #endregion

        #region IScreenBody Extensions

        /// <summary>
        /// Checks if the screen is currently loading.
        /// </summary>
        public static bool IsLoading(this IScreenBody screen)
        {
            return screen?.HasState(ScreenState.Loading) ?? false;
        }

        /// <summary>
        /// Checks if the screen is currently unloading.
        /// </summary>
        public static bool IsUnloading(this IScreenBody screen)
        {
            return screen?.HasState(ScreenState.Unloading) ?? false;
        }

        /// <summary>
        /// Checks if the screen is in the passive pool.
        /// </summary>
        public static bool IsPooled(this IScreenBody screen)
        {
            return screen?.HasState(ScreenState.InPool) ?? false;
        }

        /// <summary>
        /// Checks if the screen is currently active/in use.
        /// </summary>
        public static bool IsActive(this IScreenBody screen)
        {
            return screen?.HasState(ScreenState.InUse) ?? false;
        }

        /// <summary>
        /// Checks if the screen is playing show animation.
        /// </summary>
        public static bool IsShowingAnimation(this IScreenBody screen)
        {
            return screen?.HasState(ScreenState.InShowAnimation) ?? false;
        }

        /// <summary>
        /// Checks if the screen is playing hide animation.
        /// </summary>
        public static bool IsHidingAnimation(this IScreenBody screen)
        {
            return screen?.HasState(ScreenState.InHideAnimation) ?? false;
        }

        /// <summary>
        /// Checks if the screen is in any animation state.
        /// </summary>
        public static bool IsAnimating(this IScreenBody screen)
        {
            return screen.IsShowingAnimation() || screen.IsHidingAnimation();
        }

        /// <summary>
        /// Gets the screen's manager ID.
        /// </summary>
        public static int GetManagerId(this IScreenBody screen)
        {
            return screen?.Data?.ManagerId ?? 0;
        }

        /// <summary>
        /// Gets the screen's layer index.
        /// </summary>
        public static int GetLayerIndex(this IScreenBody screen)
        {
            return screen?.Data?.LayerIndex ?? 0;
        }

        /// <summary>
        /// Gets the screen's tag.
        /// </summary>
        public static ScreenTag GetTag(this IScreenBody screen)
        {
            return screen?.Data?.Tag ?? ScreenTag.Default;
        }

        #endregion
    }
}
