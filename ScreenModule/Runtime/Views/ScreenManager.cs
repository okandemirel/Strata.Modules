using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// MonoBehaviour that manages a set of screen layers and configurations.
    /// Place this on a Canvas or UI root object.
    /// </summary>
    public class ScreenManager : MonoBehaviour
    {
        [Header("Manager Settings")]
        [Tooltip("Unique identifier for this manager")]
        [SerializeField] private int managerId = 0;

        [Header("Layers")]
        [Tooltip("Screen layers managed by this manager (order matters)")]
        [SerializeField] private List<ScreenLayer> layers = new();

        [Header("Screen Configurations")]
        [Tooltip("Screen configurations registered with this manager")]
        [SerializeField] private List<ScreenConfig> configs = new();

        /// <summary>
        /// Event raised when this manager is registered.
        /// </summary>
        public static event Action<ScreenManager> OnManagerRegistered;

        /// <summary>
        /// Event raised when this manager is unregistered.
        /// </summary>
        public static event Action<ScreenManager> OnManagerUnregistered;

        /// <summary>
        /// The unique ID of this manager.
        /// </summary>
        public int ManagerId => managerId;

        /// <summary>
        /// The layers managed by this manager.
        /// </summary>
        public IReadOnlyList<ScreenLayer> Layers => layers;

        /// <summary>
        /// The screen configurations registered with this manager.
        /// </summary>
        public IReadOnlyList<ScreenConfig> Configs => configs;

        private void Awake()
        {
            Register();
        }

        private void OnDestroy()
        {
            Unregister();
        }

        /// <summary>
        /// Registers this manager with the screen service.
        /// Called automatically on Awake.
        /// </summary>
        public void Register()
        {
            OnManagerRegistered?.Invoke(this);
        }

        /// <summary>
        /// Unregisters this manager from the screen service.
        /// Called automatically on OnDestroy.
        /// </summary>
        public void Unregister()
        {
            OnManagerUnregistered?.Invoke(this);
        }

        /// <summary>
        /// Gets a layer by index.
        /// </summary>
        /// <param name="index">The layer index.</param>
        /// <returns>The layer, or null if index is invalid.</returns>
        public ScreenLayer GetLayer(int index)
        {
            if (index < 0 || index >= layers.Count)
                return null;
            return layers[index];
        }

        /// <summary>
        /// Gets the number of layers.
        /// </summary>
        public int LayerCount => layers.Count;

        /// <summary>
        /// Adds a layer at runtime.
        /// </summary>
        /// <param name="layer">The layer to add.</param>
        public void AddLayer(ScreenLayer layer)
        {
            if (layer != null && !layers.Contains(layer))
            {
                layers.Add(layer);
            }
        }

        /// <summary>
        /// Adds a configuration at runtime.
        /// </summary>
        /// <param name="config">The configuration to add.</param>
        public void AddConfig(ScreenConfig config)
        {
            if (config != null && !configs.Contains(config))
            {
                configs.Add(config);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var config in configs)
            {
                if (config != null)
                {
                    if (config.Validate(out var error))
                    {
                    }
                    else
                    {
                        Debug.LogWarning($"ScreenConfig '{config.name}' validation failed: {error}", config);
                    }
                }
            }
        }
#endif
    }
}
