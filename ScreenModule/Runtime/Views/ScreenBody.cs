using System;
using UnityEngine;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Abstract base class for all screens.
    /// Provides animation support and lifecycle hooks.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class ScreenBody : MonoBehaviour, IScreenBody
    {
        private static readonly int ShowTriggerHash = Animator.StringToHash("Show");
        private static readonly int HideTriggerHash = Animator.StringToHash("Hide");

        [Header("Animation")]
        [Tooltip("Optional Unity Animator for animations")]
        [SerializeField] private Animator animator;

        private RectTransform _rectTransform;
        private ScreenData _data;

        #region IScreenBody Implementation

        /// <inheritdoc/>
        public ScreenData Data
        {
            get => _data;
            set => _data = value;
        }

        /// <inheritdoc/>
        public Action<IScreenBody> OnShowAnimationComplete { get; set; }

        /// <inheritdoc/>
        public Action<IScreenBody> OnHideAnimationComplete { get; set; }

        /// <inheritdoc/>
        public GameObject GameObject => gameObject;

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

        #endregion

        #region Animation Support

        /// <summary>
        /// Override to provide a custom animator implementation.
        /// Takes priority over Unity Animator and PlayShowAnimation/PlayHideAnimation.
        /// </summary>
        protected virtual IScreenAnimator CustomAnimator => null;

        #endregion

        #region Lifecycle

        /// <inheritdoc/>
        public virtual void Show()
        {
            gameObject.SetActive(true);

            if (_data == null || !_data.HasShowAnimation)
            {
                OnShowComplete();
                return;
            }

            _data.State |= ScreenState.InShowAnimation;

            if (CustomAnimator != null)
            {
                CustomAnimator.PlayShow(this, OnShowComplete);
            }
            else if (animator != null)
            {
                animator.SetTrigger(ShowTriggerHash);
            }
            else
            {
                PlayShowAnimation();
            }
        }

        /// <inheritdoc/>
        public virtual void Hide()
        {
            if (_data == null || !_data.HasHideAnimation)
            {
                OnHideComplete();
                return;
            }

            _data.State |= ScreenState.InHideAnimation;

            if (CustomAnimator != null)
            {
                CustomAnimator.PlayHide(this, OnHideComplete);
            }
            else if (animator != null)
            {
                animator.SetTrigger(HideTriggerHash);
            }
            else
            {
                PlayHideAnimation();
            }
        }

        /// <summary>
        /// Override to implement custom show animation.
        /// Call ShowAnimationFinished() when done.
        /// </summary>
        protected virtual void PlayShowAnimation()
        {
            ShowAnimationFinished();
        }

        /// <summary>
        /// Override to implement custom hide animation.
        /// Call HideAnimationFinished() when done.
        /// </summary>
        protected virtual void PlayHideAnimation()
        {
            HideAnimationFinished();
        }

        /// <summary>
        /// Call this when show animation completes.
        /// </summary>
        public void ShowAnimationFinished()
        {
            OnShowComplete();
        }

        /// <summary>
        /// Call this when hide animation completes.
        /// </summary>
        public void HideAnimationFinished()
        {
            OnHideComplete();
        }

        private void OnShowComplete()
        {
            if (_data != null)
            {
                _data.State &= ~ScreenState.InShowAnimation;
            }
            OnShowAnimationComplete?.Invoke(this);
            OnScreenShown();
        }

        private void OnHideComplete()
        {
            if (_data != null)
            {
                _data.State &= ~ScreenState.InHideAnimation;
            }
            OnHideAnimationComplete?.Invoke(this);
        }

        /// <inheritdoc/>
        public virtual void OnScreenHidden()
        {
            gameObject.SetActive(false);
            OnScreenDeactivated();
        }

        /// <inheritdoc/>
        public virtual void BeforeSetup()
        {
            gameObject.SetActive(true);
        }

        /// <inheritdoc/>
        public virtual void AfterSetup()
        {
        }

        /// <inheritdoc/>
        public virtual void SetParameters(object[] parameters)
        {   
        }

        #endregion

        #region State Management

        /// <inheritdoc/>
        public bool HasState(ScreenState state)
        {
            return _data != null && (_data.State & state) == state;
        }

        /// <inheritdoc/>
        public void AddState(ScreenState state)
        {
            if (_data != null)
            {
                _data.State |= state;
            }
        }

        /// <inheritdoc/>
        public void RemoveState(ScreenState state)
        {
            if (_data != null)
            {
                _data.State &= ~state;
            }
        }

        #endregion

        #region Virtual Callbacks

        /// <summary>
        /// Called after show animation completes.
        /// </summary>
        protected virtual void OnScreenShown()
        {
        }

        /// <summary>
        /// Called after screen is deactivated and returned to pool.
        /// </summary>
        protected virtual void OnScreenDeactivated()
        {
        }

        #endregion

        #region Unity Callbacks

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        protected virtual void OnDestroy()
        {
            OnShowAnimationComplete = null;
            OnHideAnimationComplete = null;
        }

        #endregion
    }
}
