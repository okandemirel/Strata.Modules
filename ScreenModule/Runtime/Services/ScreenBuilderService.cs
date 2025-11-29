using System;
using System.Threading.Tasks;
using Strada.Core.DI.Attributes;
using Strada.Core.Patterns;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Service for building and showing screens.
    /// </summary>
    public class ScreenBuilderService : Service, IScreenBuilderService
    {
        [Inject] private IScreenConfigModel _configModel;
        [Inject] private IScreenRuntimeModel _runtimeModel;
        [Inject] private ScreenShowService _showService;
        [Inject] private ScreenHideService _hideService;
        [Inject] private ScreenCheckService _checkService;

        private ScreenData _currentData;
        private bool _forceOpen;
        private Type _currentScreenType;

        /// <inheritdoc/>
        public IScreenBuilderService Open<T>(int managerId = 0) where T : class, IScreenBody
        {
            Reset();

            _currentScreenType = typeof(T);
            _currentData = new ScreenData
            {
                ScreenType = _currentScreenType,
                ManagerId = managerId
            };

            var config = _configModel.GetConfig(managerId, _currentScreenType);
            if (config != null)
            {
                config.CopyToData(_currentData);
            }

            return this;
        }

        /// <inheritdoc/>
        public IScreenBuilderService SetLayer(int layerIndex)
        {
            if (_currentData != null)
            {
                _currentData.LayerIndex = layerIndex;
            }
            return this;
        }

        /// <inheritdoc/>
        public IScreenBuilderService SetForceOpen(bool force)
        {
            _forceOpen = force;
            return this;
        }

        /// <inheritdoc/>
        public IScreenBuilderService SetParameters(params object[] parameters)
        {
            if (_currentData != null)
            {
                _currentData.Parameters = parameters ?? Array.Empty<object>();
            }
            return this;
        }

        /// <inheritdoc/>
        public IScreenBuilderService AddToHistory()
        {
            if (_currentData != null)
            {
                _currentData.AddToHistory = true;
            }
            return this;
        }

        /// <inheritdoc/>
        public async Task<IScreenBody> ShowAsync()
        {
            if (_currentData == null || _currentScreenType == null)
            {
                throw new InvalidOperationException("No screen configured. Call Open<T>() first.");
            }

            if (!_checkService.CanOpenScreen(
                _currentScreenType,
                _currentData.ManagerId,
                _currentData.LayerIndex,
                _forceOpen,
                out var error))
            {
                if (_checkService.IsScreenDuplicate(_currentScreenType, _currentData.ManagerId, out var existing))
                {
                    Reset();
                    return existing;
                }

                throw new InvalidOperationException(error);
            }

            if (_forceOpen && _runtimeModel.IsLayerOccupied(_currentData.LayerIndex, _currentData.ManagerId, out var occupant))
            {
                await _hideService.HideScreenAsync(occupant, immediate: true);
            }

            var dataToShow = _currentData;

            Reset();

            return await _showService.ShowScreenAsync(dataToShow);
        }

        /// <inheritdoc/>
        public async Task<T> ShowAsync<T>() where T : class, IScreenBody
        {
            var screen = await ShowAsync();
            return screen as T;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            _currentData = null;
            _currentScreenType = null;
            _forceOpen = false;
        }
    }
}
