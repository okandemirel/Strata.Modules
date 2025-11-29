using System;
using System.Collections.Generic;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Interface for the screen configuration model.
    /// Manages screen manager registrations and screen configurations.
    /// </summary>
    public interface IScreenConfigModel
    {
        /// <summary>
        /// Registers a screen manager.
        /// </summary>
        /// <param name="manager">The manager data to register.</param>
        void RegisterManager(ScreenManagerData manager);

        /// <summary>
        /// Unregisters a screen manager.
        /// </summary>
        /// <param name="managerId">The manager ID to unregister.</param>
        void UnregisterManager(int managerId);

        /// <summary>
        /// Gets a registered screen manager.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>The manager data, or null if not found.</returns>
        ScreenManagerData GetManager(int managerId);

        /// <summary>
        /// Checks if a manager is registered.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>True if registered.</returns>
        bool HasManager(int managerId);

        /// <summary>
        /// Registers a screen configuration for a manager.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="screenType">The screen type.</param>
        /// <param name="config">The configuration.</param>
        void RegisterConfig(int managerId, Type screenType, ScreenConfig config);

        /// <summary>
        /// Unregisters a screen configuration.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="screenType">The screen type.</param>
        void UnregisterConfig(int managerId, Type screenType);

        /// <summary>
        /// Gets a screen configuration.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="screenType">The screen type.</param>
        /// <returns>The configuration, or null if not found.</returns>
        ScreenConfig GetConfig(int managerId, Type screenType);

        /// <summary>
        /// Gets all configurations for a specific tag.
        /// </summary>
        /// <param name="tag">The tag to filter by.</param>
        /// <returns>List of matching configurations.</returns>
        List<ScreenConfig> GetConfigsByTag(ScreenTag tag);

        /// <summary>
        /// Gets all registered configurations across all managers.
        /// </summary>
        /// <returns>All registered configurations.</returns>
        List<ScreenConfig> GetAllConfigs();

        /// <summary>
        /// Marks a configuration as loaded with its screen instance.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="screen">The loaded screen instance.</param>
        void MarkConfigLoaded(ScreenConfig config, IScreenBody screen);

        /// <summary>
        /// Marks a configuration as unloaded.
        /// </summary>
        /// <param name="config">The configuration.</param>
        void MarkConfigUnloaded(ScreenConfig config);

        /// <summary>
        /// Checks if a configuration is currently loaded.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="screen">The loaded screen instance if found.</param>
        /// <returns>True if loaded.</returns>
        bool IsConfigLoaded(ScreenConfig config, out IScreenBody screen);

        /// <summary>
        /// Gets all loaded screen instances.
        /// </summary>
        /// <returns>All loaded screens.</returns>
        List<IScreenBody> GetAllLoadedScreens();
    }
}
