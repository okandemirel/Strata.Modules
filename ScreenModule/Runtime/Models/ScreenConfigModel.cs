using System;
using System.Collections.Generic;
using Strada.Core.Patterns;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Manages screen manager registrations and screen configurations.
    /// </summary>
    public class ScreenConfigModel : Model, IScreenConfigModel
    {
        private readonly Dictionary<int, ScreenManagerData> _managers = new();

        private readonly Dictionary<ScreenConfig, IScreenBody> _loadedScreens = new();

        private readonly Dictionary<ScreenTag, List<ScreenConfig>> _tagIndex = new();

        protected override void OnInitialize()
        {
            base.OnInitialize();

            foreach (ScreenTag tag in Enum.GetValues(typeof(ScreenTag)))
            {
                _tagIndex[tag] = new List<ScreenConfig>();
            }
        }

        protected override void OnDispose()
        {
            _managers.Clear();
            _loadedScreens.Clear();
            foreach (var list in _tagIndex.Values)
                list.Clear();

            base.OnDispose();
        }

        #region Manager Registration

        public void RegisterManager(ScreenManagerData manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            _managers[manager.ManagerId] = manager;
        }

        public void UnregisterManager(int managerId)
        {
            if (_managers.TryGetValue(managerId, out var manager))
            {
                foreach (var config in manager.Configs.Values)
                {
                    _tagIndex[config.Tag].Remove(config);
                    _loadedScreens.Remove(config);
                }

                _managers.Remove(managerId);
            }
        }

        public ScreenManagerData GetManager(int managerId)
        {
            return _managers.TryGetValue(managerId, out var manager) ? manager : null;
        }

        public bool HasManager(int managerId)
        {
            return _managers.ContainsKey(managerId);
        }

        #endregion

        #region Config Registration

        public void RegisterConfig(int managerId, Type screenType, ScreenConfig config)
        {
            if (screenType == null)
                throw new ArgumentNullException(nameof(screenType));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (!_managers.TryGetValue(managerId, out var manager))
            {
                manager = new ScreenManagerData(managerId);
                _managers[managerId] = manager;
            }

            manager.Configs[screenType] = config;

            if (!_tagIndex[config.Tag].Contains(config))
            {
                _tagIndex[config.Tag].Add(config);
            }

            config.ResolveType();
        }

        public void UnregisterConfig(int managerId, Type screenType)
        {
            if (_managers.TryGetValue(managerId, out var manager))
            {
                if (manager.Configs.TryGetValue(screenType, out var config))
                {
                    _tagIndex[config.Tag].Remove(config);
                    _loadedScreens.Remove(config);
                    manager.Configs.Remove(screenType);
                }
            }
        }

        public ScreenConfig GetConfig(int managerId, Type screenType)
        {
            if (_managers.TryGetValue(managerId, out var manager))
            {
                return manager.Configs.TryGetValue(screenType, out var config) ? config : null;
            }
            return null;
        }

        public List<ScreenConfig> GetConfigsByTag(ScreenTag tag)
        {
            return new List<ScreenConfig>(_tagIndex[tag]);
        }

        public List<ScreenConfig> GetAllConfigs()
        {
            var result = new List<ScreenConfig>();
            foreach (var manager in _managers.Values)
            {
                result.AddRange(manager.Configs.Values);
            }
            return result;
        }

        #endregion

        #region Loaded Screen Tracking

        public void MarkConfigLoaded(ScreenConfig config, IScreenBody screen)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (screen != null)
            {
                _loadedScreens[config] = screen;
            }
            else
            {
                _loadedScreens.Remove(config);
            }
        }

        public void MarkConfigUnloaded(ScreenConfig config)
        {
            if (config != null)
            {
                _loadedScreens.Remove(config);
            }
        }

        public bool IsConfigLoaded(ScreenConfig config, out IScreenBody screen)
        {
            screen = null;
            if (config == null)
                return false;

            return _loadedScreens.TryGetValue(config, out screen);
        }

        public List<IScreenBody> GetAllLoadedScreens()
        {
            return new List<IScreenBody>(_loadedScreens.Values);
        }

        #endregion
    }
}
