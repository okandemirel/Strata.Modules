using UnityEditor;
using UnityEngine;

namespace Strada.Modules.Screen.Editor
{
    /// <summary>
    /// Custom editor for ScreenConfig ScriptableObject.
    /// Provides validation and helpful UI for configuring screens.
    /// </summary>
    [CustomEditor(typeof(ScreenConfig))]
    public class ScreenConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty _screenTypeName;
        private SerializedProperty _loadType;
        private SerializedProperty _prefab;
        private SerializedProperty _resourcePath;
        private SerializedProperty _addressableKey;
        private SerializedProperty _defaultLayerIndex;
        private SerializedProperty _tag;
        private SerializedProperty _poolable;
        private SerializedProperty _poolSize;
        private SerializedProperty _addToHistory;

        private bool _showAdvanced = true;
        private bool _showPoolSettings = true;

        private void OnEnable()
        {
            _screenTypeName = serializedObject.FindProperty("screenTypeName");
            _loadType = serializedObject.FindProperty("loadType");
            _prefab = serializedObject.FindProperty("prefab");
            _resourcePath = serializedObject.FindProperty("resourcePath");
            _addressableKey = serializedObject.FindProperty("addressableKey");
            _defaultLayerIndex = serializedObject.FindProperty("defaultLayerIndex");
            _tag = serializedObject.FindProperty("tag");
            _poolable = serializedObject.FindProperty("poolable");
            _poolSize = serializedObject.FindProperty("poolSize");
            _addToHistory = serializedObject.FindProperty("addToHistory");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = (ScreenConfig)target;

            DrawHeader();
            DrawScreenTypeSection(config);
            DrawLoadingSection();
            DrawLayerSection();
            DrawPoolSection();
            DrawAdvancedSection();
            DrawValidationMessages(config);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Screen Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
        }

        private void DrawScreenTypeSection(ScreenConfig config)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Screen Type", EditorStyles.miniBoldLabel);

            EditorGUILayout.PropertyField(_screenTypeName, new GUIContent("Type Name"));

            if (config.ResolveType())
            {
                EditorGUILayout.HelpBox($"Resolved: {config.ScreenType.FullName}", MessageType.Info);
            }
            else if (!string.IsNullOrEmpty(_screenTypeName.stringValue))
            {
                EditorGUILayout.HelpBox("Type not found. Make sure the type exists and inherits from ScreenBody.", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawLoadingSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Loading", EditorStyles.miniBoldLabel);

            EditorGUILayout.PropertyField(_loadType, new GUIContent("Load Type"));

            var loadType = (ScreenLoadType)_loadType.enumValueIndex;

            switch (loadType)
            {
                case ScreenLoadType.DirectPrefab:
                    EditorGUILayout.PropertyField(_prefab, new GUIContent("Prefab"));
                    if (_prefab.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox("Assign a prefab to use Direct Prefab loading.", MessageType.Warning);
                    }
                    break;

                case ScreenLoadType.Resource:
                    EditorGUILayout.PropertyField(_resourcePath, new GUIContent("Resource Path"));
                    if (string.IsNullOrEmpty(_resourcePath.stringValue))
                    {
                        EditorGUILayout.HelpBox("Enter a Resources path (without 'Resources/' prefix).", MessageType.Warning);
                    }
                    break;

                case ScreenLoadType.Addressable:
                    EditorGUILayout.PropertyField(_addressableKey, new GUIContent("Addressable Key"));
                    if (string.IsNullOrEmpty(_addressableKey.stringValue))
                    {
                        EditorGUILayout.HelpBox("Enter an Addressables key or address.", MessageType.Warning);
                    }
                    break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawLayerSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Layer Settings", EditorStyles.miniBoldLabel);

            EditorGUILayout.PropertyField(_defaultLayerIndex, new GUIContent("Default Layer"));
            EditorGUILayout.PropertyField(_tag, new GUIContent("Screen Tag"));

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawPoolSection()
        {
            _showPoolSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_showPoolSettings, "Pooling");

            if (_showPoolSettings)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.PropertyField(_poolable, new GUIContent("Enable Pooling"));

                if (_poolable.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_poolSize, new GUIContent("Pool Size"));
                    if (_poolSize.intValue < 1)
                    {
                        _poolSize.intValue = 1;
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(5);
        }

        private void DrawAdvancedSection()
        {
            _showAdvanced = EditorGUILayout.BeginFoldoutHeaderGroup(_showAdvanced, "Advanced");

            if (_showAdvanced)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.PropertyField(_addToHistory, new GUIContent("Add To History",
                    "Whether this screen should be added to navigation history when shown."));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawValidationMessages(ScreenConfig config)
        {
            EditorGUILayout.Space(10);

            var issues = ValidateConfig(config);

            if (issues.Length > 0)
            {
                EditorGUILayout.HelpBox(string.Join("\n", issues), MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("Configuration is valid.", MessageType.Info);
            }
        }

        private string[] ValidateConfig(ScreenConfig config)
        {
            var issues = new System.Collections.Generic.List<string>();

            if (string.IsNullOrEmpty(_screenTypeName.stringValue))
            {
                issues.Add("• Screen Type Name is required.");
            }
            else if (!config.ResolveType())
            {
                issues.Add("• Screen Type could not be resolved.");
            }

            var loadType = (ScreenLoadType)_loadType.enumValueIndex;
            switch (loadType)
            {
                case ScreenLoadType.DirectPrefab:
                    if (_prefab.objectReferenceValue == null)
                    {
                        issues.Add("• Prefab is required for Direct Prefab loading.");
                    }
                    break;
                case ScreenLoadType.Resource:
                    if (string.IsNullOrEmpty(_resourcePath.stringValue))
                    {
                        issues.Add("• Resource Path is required for Resource loading.");
                    }
                    break;
                case ScreenLoadType.Addressable:
                    if (string.IsNullOrEmpty(_addressableKey.stringValue))
                    {
                        issues.Add("• Addressable Key is required for Addressable loading.");
                    }
                    break;
            }

            return issues.ToArray();
        }
    }
}
