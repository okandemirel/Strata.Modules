using System.Threading.Tasks;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Interface for the screen builder service.
    /// Provides a fluent API for configuring and opening screens.
    /// </summary>
    public interface IScreenBuilderService
    {
        /// <summary>
        /// Initiates opening a screen of the specified type.
        /// </summary>
        /// <typeparam name="T">The screen type.</typeparam>
        /// <param name="managerId">The manager ID to open on.</param>
        /// <returns>This builder for chaining.</returns>
        IScreenBuilderService Open<T>(int managerId = 0) where T : class, IScreenBody;

        /// <summary>
        /// Sets the layer index for the screen.
        /// </summary>
        /// <param name="layerIndex">The layer index.</param>
        /// <returns>This builder for chaining.</returns>
        IScreenBuilderService SetLayer(int layerIndex);

        /// <summary>
        /// Sets whether to force open the screen even if the layer is occupied.
        /// If true, the existing screen on the layer will be hidden.
        /// </summary>
        /// <param name="force">Whether to force open.</param>
        /// <returns>This builder for chaining.</returns>
        IScreenBuilderService SetForceOpen(bool force);

        /// <summary>
        /// Sets parameters to pass to the screen.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>This builder for chaining.</returns>
        IScreenBuilderService SetParameters(params object[] parameters);

        /// <summary>
        /// Marks this screen to be added to navigation history.
        /// </summary>
        /// <returns>This builder for chaining.</returns>
        IScreenBuilderService AddToHistory();

        /// <summary>
        /// Shows the screen asynchronously.
        /// </summary>
        /// <returns>Task that completes with the shown screen.</returns>
        Task<IScreenBody> ShowAsync();

        /// <summary>
        /// Shows the screen asynchronously with typed return.
        /// </summary>
        /// <typeparam name="T">The screen type.</typeparam>
        /// <returns>Task that completes with the shown screen.</returns>
        Task<T> ShowAsync<T>() where T : class, IScreenBody;

        /// <summary>
        /// Resets the builder state for reuse.
        /// </summary>
        void Reset();
    }
}
