using System;
using System.Collections.Generic;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Interface for the screen runtime model.
    /// Manages active screens and the passive pool.
    /// </summary>
    public interface IScreenRuntimeModel
    {
        /// <summary>
        /// Adds a screen to the passive pool for reuse.
        /// </summary>
        /// <param name="screen">The screen to pool.</param>
        void AddToPassivePool(IScreenBody screen);

        /// <summary>
        /// Removes a screen from the passive pool.
        /// </summary>
        /// <param name="screen">The screen to remove.</param>
        void RemoveFromPassivePool(IScreenBody screen);

        /// <summary>
        /// Tries to get a screen from the passive pool.
        /// </summary>
        /// <typeparam name="T">The screen type.</typeparam>
        /// <param name="screen">The retrieved screen if found.</param>
        /// <returns>True if a screen was found in the pool.</returns>
        bool TryGetFromPool<T>(out T screen) where T : class, IScreenBody;

        /// <summary>
        /// Tries to get a screen from the passive pool by type.
        /// </summary>
        /// <param name="screenType">The screen type.</param>
        /// <param name="screen">The retrieved screen if found.</param>
        /// <returns>True if a screen was found.</returns>
        bool TryGetFromPool(Type screenType, out IScreenBody screen);

        /// <summary>
        /// Adds a screen to the active pools.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        void AddToActive(IScreenBody screen);

        /// <summary>
        /// Removes a screen from the active pools.
        /// </summary>
        /// <param name="screen">The screen to remove.</param>
        void RemoveFromActive(IScreenBody screen);

        /// <summary>
        /// Checks if a layer is occupied by an active screen.
        /// </summary>
        /// <param name="layerIndex">The layer index.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="occupant">The occupying screen if found.</param>
        /// <returns>True if the layer is occupied.</returns>
        bool IsLayerOccupied(int layerIndex, int managerId, out IScreenBody occupant);

        /// <summary>
        /// Checks if a screen type is currently active.
        /// </summary>
        /// <param name="screenType">The screen type.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="screen">The active screen if found.</param>
        /// <returns>True if the screen is active.</returns>
        bool IsScreenActive(Type screenType, int managerId, out IScreenBody screen);

        /// <summary>
        /// Checks if a screen type is currently active (generic version).
        /// </summary>
        /// <typeparam name="T">The screen type.</typeparam>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="screen">The active screen if found.</param>
        /// <returns>True if the screen is active.</returns>
        bool IsScreenActive<T>(int managerId, out T screen) where T : class, IScreenBody;

        /// <summary>
        /// Gets all active screens across all managers.
        /// </summary>
        /// <returns>All active screens.</returns>
        List<IScreenBody> GetAllActiveScreens();

        /// <summary>
        /// Gets all active screens for a specific manager.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>Active screens for the manager.</returns>
        List<IScreenBody> GetActiveScreensByManager(int managerId);

        /// <summary>
        /// Gets all active screens with a specific tag.
        /// </summary>
        /// <param name="tag">The tag to filter by.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>Active screens with the tag.</returns>
        List<IScreenBody> GetActiveScreensByTag(ScreenTag tag, int managerId);

        /// <summary>
        /// Gets all screens in the passive pool.
        /// </summary>
        /// <returns>All pooled screens.</returns>
        List<IScreenBody> GetAllPooledScreens();

        /// <summary>
        /// Clears all pools and active screens.
        /// </summary>
        void ClearAll();
    }
}
