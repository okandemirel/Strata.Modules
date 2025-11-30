using System;
using UnityEngine;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// ScriptableObject configuration for a screen. Defines how the screen
    /// should be loaded, its default layer, animations, and other settings.
    /// </summary>
    [CreateAssetMenu(fileName = "ScreenConfig", menuName = "Strada/Modules/Screen/Screen Config")]
    public class ScreenConfig : ScriptableObject
    {
        [Header("Loading")]
        [Tooltip("How this screen should be loaded")]
        [SerializeField] private ScreenLoadType loadType = ScreenLoadType.DirectPrefab;

        [Tooltip("Direct reference to the screen prefab (for DirectPrefab load type)")]
        [SerializeField] private GameObject directPrefab;

        [Tooltip("Path in Resources folder (for Resource load type)")]
        [SerializeField] private string resourcePath;

        [Tooltip("Addressable key/address (for Addressable load type)")]
        [SerializeField] private string addressableKey;

        [Header("Display")]
        [Tooltip("Default layer index for this screen")]
        [SerializeField] private int defaultLayer = 0;

        [Tooltip("Category tag for this screen")]
        [SerializeField] private ScreenTag tag = ScreenTag.Default;

        [Header("Animation")]
        [Tooltip("Whether this screen has a show/entrance animation")]
        [SerializeField] private bool hasShowAnimation = false;

        [Tooltip("Whether this screen has a hide/exit animation")]
        [SerializeField] private bool hasHideAnimation = false;

        [Header("Type Binding")]
        [Tooltip("Full type name of the IScreenBody implementation (e.g., MyGame.UI.MainMenuScreen)")]
        [SerializeField] private string screenTypeName;

        private Type _resolvedScreenType;
        private bool _typeResolved;

        #region Properties

        /// <summary>
        /// How this screen should be loaded.
        /// </summary>
        public ScreenLoadType LoadType => loadType;

        /// <summary>
        /// Direct reference to the screen prefab.
        /// </summary>
        public GameObject DirectPrefab => directPrefab;

        /// <summary>
        /// Path in Resources folder for Resource loading.
        /// </summary>
        public string ResourcePath => resourcePath;

        /// <summary>
        /// Addressable key/address for Addressable loading.
        /// </summary>
        public string AddressableKey => addressableKey;

        /// <summary>
        /// Default layer index for this screen.
        /// </summary>
        public int DefaultLayer => defaultLayer;

        /// <summary>
        /// Category tag for this screen.
        /// </summary>
        public ScreenTag Tag => tag;

        /// <summary>
        /// Whether this screen has a show animation.
        /// </summary>
        public bool HasShowAnimation => hasShowAnimation;

        /// <summary>
        /// Whether this screen has a hide animation.
        /// </summary>
        public bool HasHideAnimation => hasHideAnimation;

        /// <summary>
        /// Full type name of the screen.
        /// </summary>
        public string ScreenTypeName => screenTypeName;

        /// <summary>
        /// Resolved Type of the screen. Call ResolveType() first.
        /// </summary>
        public Type ScreenType => _resolvedScreenType;

        #endregion

        /// <summary>
        /// Resolves the screen type from the prefab's IScreenBody component.
        /// For DirectPrefab load type, uses the prefab directly (no reflection).
        /// </summary>
        /// <returns>True if type was resolved successfully.</returns>
        public bool ResolveType()
        {
            if (_typeResolved)
                return _resolvedScreenType != null;

            _typeResolved = true;

            // For DirectPrefab: get type directly from the prefab - no reflection needed
            if (loadType == ScreenLoadType.DirectPrefab && directPrefab != null)
            {
                var screenBody = directPrefab.GetComponent<IScreenBody>();
                if (screenBody != null)
                {
                    _resolvedScreenType = screenBody.GetType();
                    return true;
                }
            }

            // For Resource/Addressable: we don't have the prefab yet,
            // so we need to defer type resolution until the prefab is loaded
            // The caller should handle this case appropriately
            return false;
        }

        /// <summary>
        /// Copies configuration values to a ScreenData instance.
        /// </summary>
        public void CopyToData(ScreenData data)
        {
            data.ScreenType = _resolvedScreenType;
            data.LayerIndex = defaultLayer;
            data.Tag = tag;
            data.HasShowAnimation = hasShowAnimation;
            data.HasHideAnimation = hasHideAnimation;
        }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <returns>True if configuration is valid.</returns>
        public bool Validate(out string error)
        {
            error = null;

            if (defaultLayer < 0)
            {
                error = "Default layer must be >= 0";
                return false;
            }

            switch (loadType)
            {
                case ScreenLoadType.DirectPrefab:
                    if (directPrefab == null)
                    {
                        error = "Direct prefab is required for DirectPrefab load type";
                        return false;
                    }
                    if (directPrefab.GetComponent<IScreenBody>() == null)
                    {
                        error = "Direct prefab must have an IScreenBody component";
                        return false;
                    }
                    break;

                case ScreenLoadType.Resource:
                    if (string.IsNullOrEmpty(resourcePath))
                    {
                        error = "Resource path is required for Resource load type";
                        return false;
                    }
                    break;

                case ScreenLoadType.Addressable:
                    if (string.IsNullOrEmpty(addressableKey))
                    {
                        error = "Addressable key is required for Addressable load type";
                        return false;
                    }
                    break;
            }

            return true;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Reset resolved type cache when config changes in editor
            _typeResolved = false;
            _resolvedScreenType = null;
        }
#endif
    }
}
