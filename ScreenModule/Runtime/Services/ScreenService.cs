using System;
using System.Threading.Tasks;
using Strada.Core.DI.Attributes;
using Strada.Core.Patterns;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Unified facade service for managing screens in the application.
    /// Provides a clean API for screen navigation, history management, and lifecycle operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ScreenService acts as the main entry point for all screen-related operations. It delegates
    /// to specialized sub-services for loading, showing, hiding, and unloading screens.
    /// </para>
    /// <para>
    /// Common usage patterns:
    /// <list type="bullet">
    /// <item><description>Opening a screen: <c>await screenService.Open&lt;MainMenuScreen&gt;().ShowAsync();</c></description></item>
    /// <item><description>Going back in history: <c>await screenService.GoBackAsync();</c></description></item>
    /// <item><description>Hiding a screen: <c>await screenService.HideAsync(screen);</c></description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class ScreenService : Service, IScreenService
    {
        [Inject] private IScreenBuilderService _builder;
        [Inject] private IScreenConfigModel _configModel;
        [Inject] private IScreenRuntimeModel _runtimeModel;
        [Inject] private ScreenLoadService _loadService;
        [Inject] private ScreenShowService _showService;
        [Inject] private ScreenHideService _hideService;
        [Inject] private ScreenUnloadService _unloadService;
        [Inject] private ScreenHistoryService _historyService;
        [Inject] private ScreenCheckService _checkService;

        #region Builder Access

        /// <inheritdoc/>
        public IScreenBuilderService Open<T>(int managerId = 0) where T : class, IScreenBody
        {
            return _builder.Open<T>(managerId);
        }

        #endregion

        #region History Navigation

        /// <inheritdoc/>
        public async Task<IScreenBody> GoBackAsync(int managerId = 0)
        {
            if (!_historyService.TryPop(managerId, out var entry))
            {
                return null;
            }

            if (_runtimeModel.IsLayerOccupied(entry.LayerIndex, managerId, out var current))
            {
                await _hideService.HideScreenAsync(current, immediate: false);
            }

            var screenData = new ScreenData
            {
                ScreenType = entry.ScreenType,
                ManagerId = managerId,
                LayerIndex = entry.LayerIndex,
                Parameters = entry.Parameters,
                AddToHistory = false
            };

            return await _showService.ShowScreenAsync(screenData);
        }

        /// <inheritdoc/>
        public bool CanGoBack(int managerId = 0)
        {
            return _historyService.CanGoBack(managerId);
        }

        /// <inheritdoc/>
        public void ClearHistory(int managerId = 0)
        {
            _historyService.Clear(managerId);
        }

        /// <inheritdoc/>
        public int GetHistoryCount(int managerId = 0)
        {
            return _historyService.GetCount(managerId);
        }

        #endregion

        #region Hide Operations

        /// <inheritdoc/>
        public Task HideAsync(IScreenBody screen, bool immediate = false)
        {
            return _hideService.HideScreenAsync(screen, immediate);
        }

        /// <inheritdoc/>
        public Task HideByTagAsync(ScreenTag tag, int managerId = 0, bool immediate = false)
        {
            return _hideService.HideByTagAsync(tag, managerId, immediate);
        }

        /// <inheritdoc/>
        public Task HideLayerAsync(int layerIndex, int managerId = 0, bool immediate = false)
        {
            return _hideService.HideLayerAsync(layerIndex, managerId, immediate);
        }

        #endregion

        #region Unload Operations

        /// <inheritdoc/>
        public Task UnloadAsync(IScreenBody screen, bool immediate = false)
        {
            return _unloadService.UnloadScreenAsync(screen, immediate);
        }

        /// <inheritdoc/>
        public Task UnloadByTagAsync(ScreenTag tag, int managerId = 0, bool immediate = false)
        {
            return _unloadService.UnloadByTagAsync(tag, managerId, immediate);
        }

        #endregion

        #region Preload

        /// <inheritdoc/>
        public async Task PreloadAsync<T>(int managerId = 0) where T : class, IScreenBody
        {
            var config = _configModel.GetConfig(managerId, typeof(T));
            if (config == null)
            {
                throw new InvalidOperationException($"No config found for screen type {typeof(T).Name} on manager {managerId}");
            }

            if (_configModel.IsConfigLoaded(config, out _))
            {
                return;
            }

            await _loadService.LoadScreenAsync(config);
        }

        #endregion

        #region Query Operations

        /// <inheritdoc/>
        public bool IsScreenActive<T>(int managerId = 0) where T : class, IScreenBody
        {
            return _runtimeModel.IsScreenActive<T>(managerId, out _);
        }

        /// <inheritdoc/>
        public bool TryGetActiveScreen<T>(int managerId, out T screen) where T : class, IScreenBody
        {
            return _runtimeModel.IsScreenActive(managerId, out screen);
        }

        /// <inheritdoc/>
        public bool IsLayerOccupied(int layerIndex, int managerId = 0)
        {
            return _runtimeModel.IsLayerOccupied(layerIndex, managerId, out _);
        }

        #endregion
    }
}
