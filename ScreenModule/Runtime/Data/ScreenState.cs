using System;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Flags representing the current state of a screen in its lifecycle.
    /// Multiple states can be combined (e.g., InUse | InShowAnimation).
    /// </summary>
    [Flags]
    public enum ScreenState
    {
        /// <summary>
        /// Default state - screen has no special state.
        /// </summary>
        None = 0,

        /// <summary>
        /// Screen is currently being loaded from its source.
        /// </summary>
        Loading = 1 << 0,

        /// <summary>
        /// Screen is being unloaded and will be destroyed.
        /// </summary>
        Unloading = 1 << 1,

        /// <summary>
        /// Screen is in the passive pool, available for reuse.
        /// </summary>
        InPool = 1 << 2,

        /// <summary>
        /// Screen is currently active and visible to the user.
        /// </summary>
        InUse = 1 << 3,

        /// <summary>
        /// Screen is playing its show/entrance animation.
        /// </summary>
        InShowAnimation = 1 << 4,

        /// <summary>
        /// Screen is playing its hide/exit animation.
        /// </summary>
        InHideAnimation = 1 << 5
    }
}
