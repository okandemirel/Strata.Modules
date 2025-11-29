using UnityEditor;
using UnityEngine;

namespace Strada.Modules.Screen.Editor
{
    /// <summary>
    /// Custom editor for ScreenLayer MonoBehaviour.
    /// </summary>
    [CustomEditor(typeof(ScreenLayer))]
    public class ScreenLayerEditor : UnityEditor.Editor
    {
        private SerializedProperty _layerIndex;
        private SerializedProperty _applySafeArea;
        private SerializedProperty _clearOnManagerUnregister;

        private void OnEnable()
        {
            _layerIndex = serializedObject.FindProperty("_layerIndex");
            _applySafeArea = serializedObject.FindProperty("_applySafeArea");
            _clearOnManagerUnregister = serializedObject.FindProperty("_clearOnManagerUnregister");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var layer = (ScreenLayer)target;

            DrawHeader();
            DrawProperties();
            DrawRuntimeInfo(layer);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Screen Layer", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
        }

        private void DrawProperties()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(_layerIndex, new GUIContent("Layer Index",
                "The order index of this layer. Lower values are rendered behind higher values."));

            EditorGUILayout.PropertyField(_applySafeArea, new GUIContent("Apply Safe Area",
                "Whether this layer should respect device safe area boundaries."));

            EditorGUILayout.PropertyField(_clearOnManagerUnregister, new GUIContent("Clear On Unregister",
                "Whether to clear all screens in this layer when the manager is unregistered."));

            EditorGUILayout.EndVertical();
        }

        private void DrawRuntimeInfo(ScreenLayer layer)
        {
            if (!Application.isPlaying) return;

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Runtime Info", EditorStyles.miniBoldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Screen Count", layer.ScreenCount);

            if (layer.RectTransform != null)
            {
                EditorGUILayout.Vector2Field("Size", layer.RectTransform.rect.size);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
        }
    }
}
