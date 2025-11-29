using UnityEngine;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Interface for screen layers - containers that hold screens.
    /// </summary>
    public interface IScreenLayer
    {
        /// <summary>
        /// Whether this layer should apply safe area adjustments.
        /// </summary>
        bool ApplySafeArea { get; }

        /// <summary>
        /// The RectTransform of this layer.
        /// </summary>
        RectTransform RectTransform { get; }

        /// <summary>
        /// The Transform of this layer.
        /// </summary>
        Transform Transform { get; }
    }
}
