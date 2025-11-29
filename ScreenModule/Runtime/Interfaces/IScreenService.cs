using System.Threading.Tasks;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Main interface for the screen service.
    /// Provides screen management functionality including opening, hiding, and unloading screens.
    /// </summary>
    public interface IScreenService
    {
        /// <summary>
        /// Opens a screen using the fluent builder API.
        /// </summary>
        /// <typeparam name="T">The screen type to open.</typeparam>
        /// <param name="managerId">The manager ID to open on.</param>
        /// <returns>Builder for configuring the screen.</returns>
        IScreenBuilderService Open<T>(int managerId = 0) where T : class, IScreenBody;

        /// <summary>
        /// Navigates back to the previous screen in history.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>Task that completes with the shown screen, or null if no history.</returns>
        Task<IScreenBody> GoBackAsync(int managerId = 0);

        /// <summary>
        /// Checks if back navigation is possible.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>True if there's history to go back to.</returns>
        bool CanGoBack(int managerId = 0);

        /// <summary>
        /// Clears the navigation history for a manager.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        void ClearHistory(int managerId = 0);

        /// <summary>
        /// Gets the number of entries in the history stack.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>Number of history entries.</returns>
        int GetHistoryCount(int managerId = 0);

        /// <summary>
        /// Hides a screen.
        /// </summary>
        /// <param name="screen">The screen to hide.</param>
        /// <param name="immediate">Whether to skip hide animation.</param>
        /// <returns>Task that completes when the screen is hidden.</returns>
        Task HideAsync(IScreenBody screen, bool immediate = false);

        /// <summary>
        /// Hides all screens with a specific tag.
        /// </summary>
        /// <param name="tag">The tag to filter by.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="immediate">Whether to skip hide animations.</param>
        /// <returns>Task that completes when all screens are hidden.</returns>
        Task HideByTagAsync(ScreenTag tag, int managerId = 0, bool immediate = false);

        /// <summary>
        /// Hides all screens on a specific layer.
        /// </summary>
        /// <param name="layerIndex">The layer index.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="immediate">Whether to skip hide animation.</param>
        /// <returns>Task that completes when the screen is hidden.</returns>
        Task HideLayerAsync(int layerIndex, int managerId = 0, bool immediate = false);

        /// <summary>
        /// Unloads a screen, removing it from pools and destroying it.
        /// </summary>
        /// <param name="screen">The screen to unload.</param>
        /// <param name="immediate">Whether to skip hide animation.</param>
        /// <returns>Task that completes when the screen is unloaded.</returns>
        Task UnloadAsync(IScreenBody screen, bool immediate = false);

        /// <summary>
        /// Unloads all screens with a specific tag.
        /// </summary>
        /// <param name="tag">The tag to filter by.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="immediate">Whether to skip hide animations.</param>
        /// <returns>Task that completes when all screens are unloaded.</returns>
        Task UnloadByTagAsync(ScreenTag tag, int managerId = 0, bool immediate = false);

        /// <summary>
        /// Preloads a screen into the passive pool without showing it.
        /// </summary>
        /// <typeparam name="T">The screen type to preload.</typeparam>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>Task that completes when the screen is loaded.</returns>
        Task PreloadAsync<T>(int managerId = 0) where T : class, IScreenBody;

        /// <summary>
        /// Checks if a screen type is currently active.
        /// </summary>
        /// <typeparam name="T">The screen type.</typeparam>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>True if the screen is active.</returns>
        bool IsScreenActive<T>(int managerId = 0) where T : class, IScreenBody;

        /// <summary>
        /// Checks if a screen type is currently active and returns it.
        /// </summary>
        /// <typeparam name="T">The screen type.</typeparam>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="screen">The active screen if found.</param>
        /// <returns>True if the screen is active.</returns>
        bool TryGetActiveScreen<T>(int managerId, out T screen) where T : class, IScreenBody;

        /// <summary>
        /// Checks if a layer is occupied.
        /// </summary>
        /// <param name="layerIndex">The layer index.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>True if the layer is occupied.</returns>
        bool IsLayerOccupied(int layerIndex, int managerId = 0);
    }
}
