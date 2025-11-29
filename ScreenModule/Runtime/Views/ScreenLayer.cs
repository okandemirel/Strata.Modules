using UnityEngine;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// MonoBehaviour that marks a transform as a screen layer.
    /// Screens are parented to layers when shown.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ScreenLayer : MonoBehaviour, IScreenLayer
    {
        [Tooltip("Whether to apply safe area adjustments to screens on this layer")]
        [SerializeField] private bool applySafeArea = false;

        private RectTransform _rectTransform;

        /// <inheritdoc/>
        public bool ApplySafeArea => applySafeArea;

        /// <inheritdoc/>
        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }
                return _rectTransform;
            }
        }

        /// <inheritdoc/>
        public Transform Transform => transform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
    }
}
