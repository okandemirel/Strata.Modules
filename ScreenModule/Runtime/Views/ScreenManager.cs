using System.Collections.Generic;
using UnityEngine;

namespace Strada.Modules.Screen
{
    public class ScreenManager : MonoBehaviour
    {
        [Header("Manager Settings")]
        [SerializeField] private int managerId = 0;

        [Header("Layers")]
        [SerializeField] private List<ScreenLayer> layers = new();

        [Header("Screen Configurations")]
        [SerializeField] private List<ScreenConfig> configs = new();

        public int ManagerId => managerId;
        public IReadOnlyList<ScreenLayer> Layers => layers;
        public IReadOnlyList<ScreenConfig> Configs => configs;
        public int LayerCount => layers.Count;

        public ScreenLayer GetLayer(int index)
        {
            if (index < 0 || index >= layers.Count)
                return null;
            return layers[index];
        }

        public void AddLayer(ScreenLayer layer)
        {
            if (layer != null && !layers.Contains(layer))
            {
                layers.Add(layer);
            }
        }

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
                    if (!config.Validate(out var error))
                    {
                        Debug.LogWarning($"ScreenConfig '{config.name}' validation failed: {error}", config);
                    }
                }
            }
        }
#endif
    }
}
