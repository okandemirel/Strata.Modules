using System;
using System.Threading.Tasks;
using Strada.Core.DI.Attributes;
using Strada.Core.Patterns;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Service for showing screens.
    /// </summary>
    public class ScreenShowService : Service
    {
        [Inject] private IScreenConfigModel _configModel;
        [Inject] private IScreenRuntimeModel _runtimeModel;
        [Inject] private ScreenLoadService _loadService;
        [Inject] private ScreenSetupService _setupService;
        [Inject] private ScreenCheckService _checkService;
        [Inject] private ScreenHideService _hideService;
        [Inject] private ScreenHistoryService _historyService;

        /// <summary>
        /// Shows a screen with the specified data.
        /// </summary>
        /// <param name="screenData">The screen data.</param>
        /// <returns>Task that completes with the shown screen.</returns>
        public async Task<IScreenBody> ShowScreenAsync(ScreenData screenData)
        {
            if (screenData?.ScreenType == null)
                throw new ArgumentNullException(nameof(screenData));

            var managerId = screenData.ManagerId;
            var screenType = screenData.ScreenType;
            var layerIndex = screenData.LayerIndex;

            var config = _configModel.GetConfig(managerId, screenType);
            if (config == null)
                throw new InvalidOperationException($"No config found for screen type {screenType.Name} on manager {managerId}");

            config.CopyToData(screenData);

            if (_runtimeModel.IsLayerOccupied(layerIndex, managerId, out var occupant))
            {
                await _hideService.HideScreenAsync(occupant, immediate: true);
            }

            IScreenBody screen;
            if (_runtimeModel.TryGetFromPool(screenType, out screen))
            {
                screen.Data = screenData;
            }
            else
            {
                screen = await _loadService.LoadScreenAsync(config);
                screen.Data = screenData;
                _runtimeModel.RemoveFromPassivePool(screen);
            }

            _setupService.SetupScreen(screen);

            if (screenData.Parameters != null && screenData.Parameters.Length > 0)
            {
                screen.SetParameters(screenData.Parameters);
            }

            _runtimeModel.AddToActive(screen);

            if (screenData.AddToHistory)
            {
                _historyService.Push(screen);
            }

            var showTcs = new TaskCompletionSource<bool>();

            screen.OnShowAnimationComplete = s =>
            {
                showTcs.TrySetResult(true);
            };

            screen.Show();

            if (screenData.HasShowAnimation)
            {
                await showTcs.Task;
            }

            return screen;
        }

        /// <summary>
        /// Shows a screen of a specific type.
        /// </summary>
        /// <typeparam name="T">The screen type.</typeparam>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="layerIndex">The layer index.</param>
        /// <param name="parameters">Optional parameters.</param>
        /// <param name="addToHistory">Whether to add to history.</param>
        /// <returns>Task that completes with the shown screen.</returns>
        public async Task<T> ShowScreenAsync<T>(int managerId, int layerIndex, object[] parameters = null, bool addToHistory = false)
            where T : class, IScreenBody
        {
            var screenData = new ScreenData
            {
                ScreenType = typeof(T),
                ManagerId = managerId,
                LayerIndex = layerIndex,
                Parameters = parameters ?? Array.Empty<object>(),
                AddToHistory = addToHistory
            };

            var screen = await ShowScreenAsync(screenData);
            return screen as T;
        }
    }
}
