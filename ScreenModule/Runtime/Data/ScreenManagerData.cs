using System;
using System.Collections.Generic;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Data container for a screen manager, holding its layers and registered screen configs.
    /// </summary>
    [Serializable]
    public class ScreenManagerData
    {
        /// <summary>
        /// Unique identifier for this manager.
        /// </summary>
        public int ManagerId;

        /// <summary>
        /// List of layers managed by this manager.
        /// </summary>
        public List<ScreenLayer> Layers = new();

        /// <summary>
        /// Mapping of screen types to their configurations.
        /// </summary>
        public Dictionary<Type, ScreenConfig> Configs = new();

        /// <summary>
        /// Creates a new ScreenManagerData with the specified ID.
        /// </summary>
        public ScreenManagerData(int managerId)
        {
            ManagerId = managerId;
        }

        /// <summary>
        /// Gets the layer at the specified index.
        /// </summary>
        /// <param name="index">Layer index.</param>
        /// <returns>The ScreenLayer at the index, or null if index is invalid.</returns>
        public ScreenLayer GetLayer(int index)
        {
            if (index < 0 || index >= Layers.Count)
                return null;
            return Layers[index];
        }

        /// <summary>
        /// Gets the configuration for a screen type.
        /// </summary>
        /// <param name="screenType">The screen type.</param>
        /// <returns>The ScreenConfig, or null if not registered.</returns>
        public ScreenConfig GetConfig(Type screenType)
        {
            return Configs.TryGetValue(screenType, out var config) ? config : null;
        }

        /// <summary>
        /// Registers a screen configuration.
        /// </summary>
        /// <param name="screenType">The screen type.</param>
        /// <param name="config">The configuration.</param>
        public void RegisterConfig(Type screenType, ScreenConfig config)
        {
            Configs[screenType] = config;
        }

        /// <summary>
        /// Unregisters a screen configuration.
        /// </summary>
        /// <param name="screenType">The screen type to unregister.</param>
        /// <returns>True if the config was removed.</returns>
        public bool UnregisterConfig(Type screenType)
        {
            return Configs.Remove(screenType);
        }

        /// <summary>
        /// Checks if a screen type is registered.
        /// </summary>
        /// <param name="screenType">The screen type to check.</param>
        /// <returns>True if registered.</returns>
        public bool HasConfig(Type screenType)
        {
            return Configs.ContainsKey(screenType);
        }

        /// <summary>
        /// Gets all registered screen configurations.
        /// </summary>
        public IEnumerable<ScreenConfig> GetAllConfigs()
        {
            return Configs.Values;
        }

        /// <summary>
        /// Gets the number of layers.
        /// </summary>
        public int LayerCount => Layers.Count;

        /// <summary>
        /// Validates that a layer index is valid for this manager.
        /// </summary>
        /// <param name="layerIndex">The layer index to validate.</param>
        /// <returns>True if valid.</returns>
        public bool IsValidLayerIndex(int layerIndex)
        {
            return layerIndex >= 0 && layerIndex < Layers.Count;
        }
    }
}
