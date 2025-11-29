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
        private SerializedProperty _applySafeArea;

        private void OnEnable()
        {
            _applySafeArea = serializedObject.FindProperty("applySafeArea");
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

            EditorGUILayout.PropertyField(_applySafeArea, new GUIContent("Apply Safe Area",
                "Whether this layer should respect device safe area boundaries."));

            EditorGUILayout.EndVertical();
        }

        private void DrawRuntimeInfo(ScreenLayer layer)
        {
            if (!Application.isPlaying) return;

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Runtime Info", EditorStyles.miniBoldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Child Count", layer.Transform.childCount);

            if (layer.RectTransform != null)
            {
                EditorGUILayout.Vector2Field("Size", layer.RectTransform.rect.size);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
        }
    }
}
