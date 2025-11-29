using System.Collections.Generic;
using System.Threading.Tasks;
using Strada.Core.DI.Attributes;
using Strada.Core.Patterns;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Service for unloading screens.
    /// </summary>
    public class ScreenUnloadService : Service
    {
        [Inject] private IScreenConfigModel _configModel;
        [Inject] private IScreenRuntimeModel _runtimeModel;
        [Inject] private ScreenHideService _hideService;
        [Inject] private ScreenLoadService _loadService;

        /// <summary>
        /// Unloads a screen, removing it from pools and destroying it.
        /// </summary>
        /// <param name="screen">The screen to unload.</param>
        /// <param name="immediate">Whether to skip hide animation.</param>
        /// <returns>Task that completes when the screen is unloaded.</returns>
        public async Task UnloadScreenAsync(IScreenBody screen, bool immediate = false)
        {
            if (screen == null)
                return;

            screen.AddState(ScreenState.Unloading);

            if (screen.HasState(ScreenState.InUse))
            {
                await _hideService.HideScreenAsync(screen, immediate);
            }

            ScreenConfig config = null;
            if (screen.Data?.ScreenType != null)
            {
                config = _configModel.GetConfig(screen.Data.ManagerId, screen.Data.ScreenType);
            }

            _loadService.UnloadScreen(screen, config);
        }

        /// <summary>
        /// Unloads all screens with a specific tag.
        /// </summary>
        /// <param name="tag">The tag to filter by.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="immediate">Whether to skip hide animations.</param>
        /// <returns>Task that completes when all screens are unloaded.</returns>
        public async Task UnloadByTagAsync(ScreenTag tag, int managerId, bool immediate = false)
        {
            var activeScreens = _runtimeModel.GetActiveScreensByTag(tag, managerId);
            var tasks = new List<Task>();

            foreach (var screen in activeScreens)
            {
                tasks.Add(UnloadScreenAsync(screen, immediate));
            }

            await Task.WhenAll(tasks);

            var pooledScreens = _runtimeModel.GetAllPooledScreens();
            foreach (var screen in pooledScreens)
            {
                if (screen.Data?.Tag == tag && screen.Data.ManagerId == managerId)
                {
                    ScreenConfig config = null;
                    if (screen.Data.ScreenType != null)
                    {
                        config = _configModel.GetConfig(managerId, screen.Data.ScreenType);
                    }
                    _loadService.UnloadScreen(screen, config);
                }
            }
        }

        /// <summary>
        /// Unloads all screens for a manager.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="immediate">Whether to skip hide animations.</param>
        /// <returns>Task that completes when all screens are unloaded.</returns>
        public async Task UnloadAllAsync(int managerId, bool immediate = false)
        {
            var activeScreens = _runtimeModel.GetActiveScreensByManager(managerId);
            var tasks = new List<Task>();

            foreach (var screen in activeScreens)
            {
                tasks.Add(UnloadScreenAsync(screen, immediate));
            }

            await Task.WhenAll(tasks);

            var pooledScreens = _runtimeModel.GetAllPooledScreens();
            foreach (var screen in pooledScreens)
            {
                if (screen.Data?.ManagerId == managerId)
                {
                    ScreenConfig config = null;
                    if (screen.Data.ScreenType != null)
                    {
                        config = _configModel.GetConfig(managerId, screen.Data.ScreenType);
                    }
                    _loadService.UnloadScreen(screen, config);
                }
            }
        }

        /// <summary>
        /// Unloads all screens across all managers.
        /// </summary>
        /// <param name="immediate">Whether to skip hide animations.</param>
        /// <returns>Task that completes when all screens are unloaded.</returns>
        public async Task UnloadEverythingAsync(bool immediate = false)
        {
            var activeScreens = _runtimeModel.GetAllActiveScreens();
            var tasks = new List<Task>();

            foreach (var screen in activeScreens)
            {
                tasks.Add(UnloadScreenAsync(screen, immediate));
            }

            await Task.WhenAll(tasks);

            var pooledScreens = _runtimeModel.GetAllPooledScreens();
            foreach (var screen in pooledScreens)
            {
                ScreenConfig config = null;
                if (screen.Data?.ScreenType != null)
                {
                    config = _configModel.GetConfig(screen.Data.ManagerId, screen.Data.ScreenType);
                }
                _loadService.UnloadScreen(screen, config);
            }
        }
    }
}
