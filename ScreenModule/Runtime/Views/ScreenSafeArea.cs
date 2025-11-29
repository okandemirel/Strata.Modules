using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// MonoBehaviour that applies safe area adjustments to screen layers.
    /// Monitors for screen changes and updates layer layouts accordingly.
    /// </summary>
    public class ScreenSafeArea : MonoBehaviour
    {
        [Header("Safe Area Settings")]
        [Tooltip("Layers that should have safe area applied")]
        [SerializeField] private List<ScreenLayer> safeLayers = new();

        [Tooltip("Apply safe area on start")]
        [SerializeField] private bool applyOnStart = true;

        private Rect _lastSafeArea;
        private ScreenOrientation _lastOrientation;
        private Vector2Int _lastResolution;

        private void Start()
        {
            if (applyOnStart)
            {
                ForceApplySafeArea();
            }
        }

        private void Update()
        {
            if (HasScreenChanged())
            {
                ApplySafeArea();
            }
        }

        /// <summary>
        /// Checks if the screen properties have changed since last check.
        /// </summary>
        /// <returns>True if screen properties changed.</returns>
        private bool HasScreenChanged()
        {
            return UnityEngine.Screen.safeArea != _lastSafeArea
                || UnityEngine.Screen.orientation != _lastOrientation
                || UnityEngine.Screen.width != _lastResolution.x
                || UnityEngine.Screen.height != _lastResolution.y;
        }

        /// <summary>
        /// Forces safe area to be applied regardless of change detection.
        /// </summary>
        public void ForceApplySafeArea()
        {
            _lastSafeArea = Rect.zero;
            ApplySafeArea();
        }

        /// <summary>
        /// Applies safe area to all registered layers.
        /// </summary>
        private void ApplySafeArea()
        {
            _lastSafeArea = UnityEngine.Screen.safeArea;
            _lastOrientation = UnityEngine.Screen.orientation;
            _lastResolution = new Vector2Int(UnityEngine.Screen.width, UnityEngine.Screen.height);

            foreach (var layer in safeLayers.Where(l => l != null && l.ApplySafeArea))
            {
                ApplyToLayer(layer, _lastSafeArea);
            }
        }

        /// <summary>
        /// Applies safe area to a specific layer.
        /// </summary>
        /// <param name="layer">The layer to apply to.</param>
        /// <param name="safeArea">The safe area rect.</param>
        private void ApplyToLayer(ScreenLayer layer, Rect safeArea)
        {
            var rectTransform = layer.RectTransform;
            if (rectTransform == null)
                return;

            var screenWidth = UnityEngine.Screen.width;
            var screenHeight = UnityEngine.Screen.height;

            if (screenWidth <= 0 || screenHeight <= 0)
                return;

            var anchorMin = new Vector2(
                safeArea.x / screenWidth,
                safeArea.y / screenHeight
            );

            var anchorMax = new Vector2(
                (safeArea.x + safeArea.width) / screenWidth,
                (safeArea.y + safeArea.height) / screenHeight
            );

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// Adds a layer to be managed by this safe area handler.
        /// </summary>
        /// <param name="layer">The layer to add.</param>
        public void AddLayer(ScreenLayer layer)
        {
            if (layer != null && !safeLayers.Contains(layer))
            {
                safeLayers.Add(layer);
                if (layer.ApplySafeArea)
                {
                    ApplyToLayer(layer, _lastSafeArea);
                }
            }
        }

        /// <summary>
        /// Removes a layer from safe area management.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        public void RemoveLayer(ScreenLayer layer)
        {
            safeLayers.Remove(layer);
        }

        /// <summary>
        /// Gets the current safe area rect.
        /// </summary>
        public Rect CurrentSafeArea => _lastSafeArea;

        /// <summary>
        /// Gets the list of managed layers.
        /// </summary>
        public IReadOnlyList<ScreenLayer> SafeLayers => safeLayers;
    }
}
