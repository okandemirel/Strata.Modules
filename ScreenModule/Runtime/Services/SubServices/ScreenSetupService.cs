using Strada.Core.DI.Attributes;
using Strada.Core.Patterns;
using UnityEngine;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Service for setting up screen transforms and parenting.
    /// </summary>
    public class ScreenSetupService : Service
    {
        [Inject] private IScreenConfigModel _configModel;

        /// <summary>
        /// Sets up a screen's transform to fill its layer.
        /// </summary>
        /// <param name="screen">The screen to set up.</param>
        public void SetupScreen(IScreenBody screen)
        {
            if (screen?.Data == null)
                return;

            var manager = _configModel.GetManager(screen.Data.ManagerId);
            if (manager == null)
                return;

            var layer = manager.GetLayer(screen.Data.LayerIndex);
            if (layer == null)
                return;

            screen.BeforeSetup();

            var rectTransform = screen.RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(layer.Transform, false);

                ConfigureRectTransformToFill(rectTransform);
            }

            if (screen.GameObject != null)
            {
                screen.GameObject.SetActive(true);
            }

            screen.AfterSetup();
        }

        /// <summary>
        /// Configures a RectTransform to fill its parent.
        /// </summary>
        /// <param name="rectTransform">The RectTransform to configure.</param>
        private void ConfigureRectTransformToFill(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;

            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
        }

        /// <summary>
        /// Moves a screen to the pool parent (hides it).
        /// </summary>
        /// <param name="screen">The screen to move.</param>
        /// <param name="poolParent">The pool parent transform.</param>
        public void MoveToPool(IScreenBody screen, Transform poolParent)
        {
            if (screen?.GameObject == null || poolParent == null)
                return;

            screen.GameObject.transform.SetParent(poolParent, false);
            screen.GameObject.SetActive(false);
        }
    }
}
