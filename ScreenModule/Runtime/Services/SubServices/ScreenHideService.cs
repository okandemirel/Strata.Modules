using System.Collections.Generic;
using System.Threading.Tasks;
using Strada.Core.DI.Attributes;
using Strada.Core.Patterns;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Service for hiding screens.
    /// </summary>
    public class ScreenHideService : Service
    {
        [Inject] private IScreenRuntimeModel _runtimeModel;
        [Inject] private ScreenCheckService _checkService;

        /// <summary>
        /// Hides a screen.
        /// </summary>
        /// <param name="screen">The screen to hide.</param>
        /// <param name="immediate">Whether to skip hide animation.</param>
        /// <returns>Task that completes when the screen is hidden.</returns>
        public async Task HideScreenAsync(IScreenBody screen, bool immediate = false)
        {
            if (screen == null)
                return;

            if (!_checkService.CanHide(screen))
                return;

            _runtimeModel.RemoveFromActive(screen);

            if (immediate || !screen.Data.HasHideAnimation)
            {
                CompleteHide(screen);
            }
            else
            {
                var hideTcs = new TaskCompletionSource<bool>();

                screen.OnHideAnimationComplete = s =>
                {
                    CompleteHide(s);
                    hideTcs.TrySetResult(true);
                };

                screen.Hide();
                await hideTcs.Task;
            }
        }

        /// <summary>
        /// Completes the hide process for a screen.
        /// </summary>
        private void CompleteHide(IScreenBody screen)
        {
            screen.OnScreenHidden();

            _runtimeModel.AddToPassivePool(screen);
        }

        /// <summary>
        /// Hides all screens with a specific tag.
        /// </summary>
        /// <param name="tag">The tag to filter by.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="immediate">Whether to skip hide animations.</param>
        /// <returns>Task that completes when all screens are hidden.</returns>
        public async Task HideByTagAsync(ScreenTag tag, int managerId, bool immediate = false)
        {
            var screens = _runtimeModel.GetActiveScreensByTag(tag, managerId);
            var tasks = new List<Task>();

            foreach (var screen in screens)
            {
                tasks.Add(HideScreenAsync(screen, immediate));
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Hides all screens on a specific layer.
        /// </summary>
        /// <param name="layerIndex">The layer index.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="immediate">Whether to skip hide animation.</param>
        /// <returns>Task that completes when the screen is hidden.</returns>
        public async Task HideLayerAsync(int layerIndex, int managerId, bool immediate = false)
        {
            if (_runtimeModel.IsLayerOccupied(layerIndex, managerId, out var screen))
            {
                await HideScreenAsync(screen, immediate);
            }
        }

        /// <summary>
        /// Hides all active screens for a manager.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="immediate">Whether to skip hide animations.</param>
        /// <returns>Task that completes when all screens are hidden.</returns>
        public async Task HideAllAsync(int managerId, bool immediate = false)
        {
            var screens = _runtimeModel.GetActiveScreensByManager(managerId);
            var tasks = new List<Task>();

            foreach (var screen in screens)
            {
                tasks.Add(HideScreenAsync(screen, immediate));
            }

            await Task.WhenAll(tasks);
        }
    }
}
