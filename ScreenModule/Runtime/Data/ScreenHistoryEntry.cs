using System;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Represents an entry in the screen navigation history.
    /// Used for back navigation functionality.
    /// </summary>
    [Serializable]
    public class ScreenHistoryEntry
    {
        /// <summary>
        /// The type of screen that was shown.
        /// </summary>
        public Type ScreenType;

        /// <summary>
        /// The manager ID the screen was shown on.
        /// </summary>
        public int ManagerId;

        /// <summary>
        /// The layer index the screen was displayed on.
        /// </summary>
        public int LayerIndex;

        /// <summary>
        /// Parameters that were passed when opening the screen.
        /// </summary>
        public object[] Parameters;

        /// <summary>
        /// When this screen was shown.
        /// </summary>
        public DateTime Timestamp;

        /// <summary>
        /// Creates a new empty history entry.
        /// </summary>
        public ScreenHistoryEntry()
        {
            Parameters = Array.Empty<object>();
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Creates a history entry from a screen body.
        /// </summary>
        /// <param name="screen">The screen to create an entry for.</param>
        public ScreenHistoryEntry(IScreenBody screen)
        {
            if (screen?.Data == null)
            {
                Parameters = Array.Empty<object>();
                Timestamp = DateTime.Now;
                return;
            }

            ScreenType = screen.Data.ScreenType;
            ManagerId = screen.Data.ManagerId;
            LayerIndex = screen.Data.LayerIndex;
            Parameters = screen.Data.Parameters ?? Array.Empty<object>();
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Creates a history entry with explicit values.
        /// </summary>
        public ScreenHistoryEntry(Type screenType, int managerId, int layerIndex, object[] parameters)
        {
            ScreenType = screenType;
            ManagerId = managerId;
            LayerIndex = layerIndex;
            Parameters = parameters ?? Array.Empty<object>();
            Timestamp = DateTime.Now;
        }
    }
}
