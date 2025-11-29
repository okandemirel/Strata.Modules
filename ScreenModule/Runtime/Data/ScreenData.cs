using System;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Runtime data for a screen instance. Created when a screen is opened
    /// and tracks its current state throughout its lifecycle.
    /// </summary>
    [Serializable]
    public class ScreenData
    {
        /// <summary>
        /// The type of the screen (IScreenBody implementation).
        /// </summary>
        public Type ScreenType;

        /// <summary>
        /// Current state flags of the screen.
        /// </summary>
        public ScreenState State;

        /// <summary>
        /// ID of the manager this screen belongs to.
        /// </summary>
        public int ManagerId;

        /// <summary>
        /// Index of the layer this screen is displayed on.
        /// </summary>
        public int LayerIndex;

        /// <summary>
        /// Category tag for this screen.
        /// </summary>
        public ScreenTag Tag;

        /// <summary>
        /// Whether this screen has a show/entrance animation.
        /// </summary>
        public bool HasShowAnimation;

        /// <summary>
        /// Whether this screen has a hide/exit animation.
        /// </summary>
        public bool HasHideAnimation;

        /// <summary>
        /// Custom parameters passed when opening the screen.
        /// </summary>
        public object[] Parameters;

        /// <summary>
        /// Whether this screen should be added to navigation history.
        /// </summary>
        public bool AddToHistory;

        /// <summary>
        /// Creates a new ScreenData with default values.
        /// </summary>
        public ScreenData()
        {
            State = ScreenState.None;
            ManagerId = 0;
            LayerIndex = 0;
            Tag = ScreenTag.Default;
            Parameters = Array.Empty<object>();
        }

        /// <summary>
        /// Creates a copy of this ScreenData.
        /// </summary>
        public ScreenData Clone()
        {
            return new ScreenData
            {
                ScreenType = ScreenType,
                State = State,
                ManagerId = ManagerId,
                LayerIndex = LayerIndex,
                Tag = Tag,
                HasShowAnimation = HasShowAnimation,
                HasHideAnimation = HasHideAnimation,
                Parameters = Parameters,
                AddToHistory = AddToHistory
            };
        }

        /// <summary>
        /// Resets the data to default values for reuse from pool.
        /// </summary>
        public void Reset()
        {
            ScreenType = null;
            State = ScreenState.None;
            ManagerId = 0;
            LayerIndex = 0;
            Tag = ScreenTag.Default;
            HasShowAnimation = false;
            HasHideAnimation = false;
            Parameters = Array.Empty<object>();
            AddToHistory = false;
        }
    }
}
