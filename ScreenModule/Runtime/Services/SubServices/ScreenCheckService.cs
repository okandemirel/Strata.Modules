using System;
using Strada.Core.DI.Attributes;
using Strada.Core.Patterns;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Service for screen validation checks.
    /// </summary>
    public class ScreenCheckService : Service
    {
        [Inject] private IScreenConfigModel _configModel;
        [Inject] private IScreenRuntimeModel _runtimeModel;

        /// <summary>
        /// Checks if a screen type is already active (would be a duplicate).
        /// </summary>
        /// <param name="screenType">The screen type to check.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="existingScreen">The existing screen if found.</param>
        /// <returns>True if the screen is already active.</returns>
        public bool IsScreenDuplicate(Type screenType, int managerId, out IScreenBody existingScreen)
        {
            return _runtimeModel.IsScreenActive(screenType, managerId, out existingScreen);
        }

        /// <summary>
        /// Checks if a screen type is already active (generic version).
        /// </summary>
        /// <typeparam name="T">The screen type.</typeparam>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="existingScreen">The existing screen if found.</param>
        /// <returns>True if the screen is already active.</returns>
        public bool IsScreenDuplicate<T>(int managerId, out T existingScreen) where T : class, IScreenBody
        {
            return _runtimeModel.IsScreenActive(managerId, out existingScreen);
        }

        /// <summary>
        /// Checks if a layer is already occupied.
        /// </summary>
        /// <param name="layerIndex">The layer index.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="occupant">The occupying screen if found.</param>
        /// <returns>True if the layer is occupied.</returns>
        public bool IsLayerFull(int layerIndex, int managerId, out IScreenBody occupant)
        {
            return _runtimeModel.IsLayerOccupied(layerIndex, managerId, out occupant);
        }

        /// <summary>
        /// Validates if a screen can be opened.
        /// </summary>
        /// <param name="screenType">The screen type.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="layerIndex">The layer index.</param>
        /// <param name="forceOpen">Whether force open is enabled.</param>
        /// <param name="error">Error message if validation fails.</param>
        /// <returns>True if the screen can be opened.</returns>
        public bool CanOpenScreen(Type screenType, int managerId, int layerIndex, bool forceOpen, out string error)
        {
            error = null;

            // Check if manager exists
            var manager = _configModel.GetManager(managerId);
            if (manager == null)
            {
                error = $"Manager {managerId} not found";
                return false;
            }

            // Check if layer is valid
            if (!manager.IsValidLayerIndex(layerIndex))
            {
                error = $"Layer index {layerIndex} is invalid for manager {managerId}";
                return false;
            }

            // Check for duplicates
            if (IsScreenDuplicate(screenType, managerId, out _))
            {
                error = $"Screen {screenType.Name} is already active on manager {managerId}";
                return false;
            }

            // Check if layer is full (unless force open)
            if (!forceOpen && IsLayerFull(layerIndex, managerId, out _))
            {
                error = $"Layer {layerIndex} is already occupied on manager {managerId}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a screen configuration exists.
        /// </summary>
        /// <param name="screenType">The screen type.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>True if the config exists.</returns>
        public bool HasConfig(Type screenType, int managerId)
        {
            return _configModel.GetConfig(managerId, screenType) != null;
        }

        /// <summary>
        /// Checks if a screen is in a valid state for showing.
        /// </summary>
        /// <param name="screen">The screen to check.</param>
        /// <returns>True if the screen can be shown.</returns>
        public bool CanShow(IScreenBody screen)
        {
            if (screen == null)
                return false;

            // Can't show if already in use
            if (screen.HasState(ScreenState.InUse))
                return false;

            // Can't show if loading or unloading
            if (screen.HasState(ScreenState.Loading) || screen.HasState(ScreenState.Unloading))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a screen is in a valid state for hiding.
        /// </summary>
        /// <param name="screen">The screen to check.</param>
        /// <returns>True if the screen can be hidden.</returns>
        public bool CanHide(IScreenBody screen)
        {
            if (screen == null)
                return false;

            // Must be in use to hide
            if (!screen.HasState(ScreenState.InUse))
                return false;

            // Can't hide if already in hide animation or unloading
            if (screen.HasState(ScreenState.InHideAnimation) || screen.HasState(ScreenState.Unloading))
                return false;

            return true;
        }
    }
}
