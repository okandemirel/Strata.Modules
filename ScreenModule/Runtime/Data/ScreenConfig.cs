using System;
using UnityEngine;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// ScriptableObject configuration for a screen. Defines how the screen
    /// should be loaded, its default layer, animations, and other settings.
    /// </summary>
    [CreateAssetMenu(fileName = "ScreenConfig", menuName = "Strada/Screen/Screen Config")]
    public class ScreenConfig : ScriptableObject
    {
        [Header("Loading")]
        [Tooltip("How this screen should be loaded")]
        [SerializeField] private ScreenLoadType _loadType = ScreenLoadType.DirectPrefab;

        [Tooltip("Direct reference to the screen prefab (for DirectPrefab load type)")]
        [SerializeField] private GameObject _directPrefab;

        [Tooltip("Path in Resources folder (for Resource load type)")]
        [SerializeField] private string _resourcePath;

        [Tooltip("Addressable key/address (for Addressable load type)")]
        [SerializeField] private string _addressableKey;

        [Header("Display")]
        [Tooltip("Default layer index for this screen")]
        [SerializeField] private int _defaultLayer = 0;

        [Tooltip("Category tag for this screen")]
        [SerializeField] private ScreenTag _tag = ScreenTag.Default;

        [Header("Animation")]
        [Tooltip("Whether this screen has a show/entrance animation")]
        [SerializeField] private bool _hasShowAnimation = false;

        [Tooltip("Whether this screen has a hide/exit animation")]
        [SerializeField] private bool _hasHideAnimation = false;

        [Header("Type Binding")]
        [Tooltip("Full type name of the IScreenBody implementation (e.g., MyGame.UI.MainMenuScreen)")]
        [SerializeField] private string _screenTypeName;

        // Cached resolved type
        private Type _resolvedScreenType;
        private bool _typeResolved;

        #region Properties

        /// <summary>
        /// How this screen should be loaded.
        /// </summary>
        public ScreenLoadType LoadType => _loadType;

        /// <summary>
        /// Direct reference to the screen prefab.
        /// </summary>
        public GameObject DirectPrefab => _directPrefab;

        /// <summary>
        /// Path in Resources folder for Resource loading.
        /// </summary>
        public string ResourcePath => _resourcePath;

        /// <summary>
        /// Addressable key/address for Addressable loading.
        /// </summary>
        public string AddressableKey => _addressableKey;

        /// <summary>
        /// Default layer index for this screen.
        /// </summary>
        public int DefaultLayer => _defaultLayer;

        /// <summary>
        /// Category tag for this screen.
        /// </summary>
        public ScreenTag Tag => _tag;

        /// <summary>
        /// Whether this screen has a show animation.
        /// </summary>
        public bool HasShowAnimation => _hasShowAnimation;

        /// <summary>
        /// Whether this screen has a hide animation.
        /// </summary>
        public bool HasHideAnimation => _hasHideAnimation;

        /// <summary>
        /// Full type name of the screen.
        /// </summary>
        public string ScreenTypeName => _screenTypeName;

        /// <summary>
        /// Resolved Type of the screen. Call ResolveType() first.
        /// </summary>
        public Type ScreenType => _resolvedScreenType;

        #endregion

        /// <summary>
        /// Resolves the screen type from the type name string.
        /// </summary>
        /// <returns>True if type was resolved successfully.</returns>
        public bool ResolveType()
        {
            if (_typeResolved)
                return _resolvedScreenType != null;

            _typeResolved = true;

            if (string.IsNullOrEmpty(_screenTypeName))
            {
                // Try to get type from prefab
                if (_directPrefab != null)
                {
                    var screenBody = _directPrefab.GetComponent<IScreenBody>();
                    if (screenBody != null)
                    {
                        _resolvedScreenType = screenBody.GetType();
                        return true;
                    }
                }
                return false;
            }

            _resolvedScreenType = Type.GetType(_screenTypeName);

            if (_resolvedScreenType == null)
            {
                // Try searching all assemblies
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    _resolvedScreenType = assembly.GetType(_screenTypeName);
                    if (_resolvedScreenType != null)
                        break;
                }
            }

            return _resolvedScreenType != null;
        }

        /// <summary>
        /// Copies configuration values to a ScreenData instance.
        /// </summary>
        public void CopyToData(ScreenData data)
        {
            data.ScreenType = _resolvedScreenType;
            data.LayerIndex = _defaultLayer;
            data.Tag = _tag;
            data.HasShowAnimation = _hasShowAnimation;
            data.HasHideAnimation = _hasHideAnimation;
        }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <returns>True if configuration is valid.</returns>
        public bool Validate(out string error)
        {
            error = null;

            if (_defaultLayer < 0)
            {
                error = "Default layer must be >= 0";
                return false;
            }

            switch (_loadType)
            {
                case ScreenLoadType.DirectPrefab:
                    if (_directPrefab == null)
                    {
                        error = "Direct prefab is required for DirectPrefab load type";
                        return false;
                    }
                    if (_directPrefab.GetComponent<IScreenBody>() == null)
                    {
                        error = "Direct prefab must have an IScreenBody component";
                        return false;
                    }
                    break;

                case ScreenLoadType.Resource:
                    if (string.IsNullOrEmpty(_resourcePath))
                    {
                        error = "Resource path is required for Resource load type";
                        return false;
                    }
                    break;

                case ScreenLoadType.Addressable:
                    if (string.IsNullOrEmpty(_addressableKey))
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
            // Auto-populate type name from prefab if empty
            if (string.IsNullOrEmpty(_screenTypeName) && _directPrefab != null)
            {
                var screenBody = _directPrefab.GetComponent<IScreenBody>();
                if (screenBody != null)
                {
                    _screenTypeName = screenBody.GetType().AssemblyQualifiedName;
                }
            }
        }
#endif
    }
}
