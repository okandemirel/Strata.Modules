using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Strada.Modules.Screen.Editor
{
    /// <summary>
    /// Custom editor for ScreenManager MonoBehaviour.
    /// Provides layer management and configuration validation.
    /// </summary>
    [CustomEditor(typeof(ScreenManager))]
    public class ScreenManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty _managerId;
        private SerializedProperty _layers;
        private SerializedProperty _configs;

        private bool _showLayers = true;
        private bool _showConfigs = true;
        private Vector2 _configScrollPosition;

        private void OnEnable()
        {
            _managerId = serializedObject.FindProperty("managerId");
            _layers = serializedObject.FindProperty("layers");
            _configs = serializedObject.FindProperty("configs");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var manager = (ScreenManager)target;

            DrawHeader(manager);
            DrawManagerId();
            DrawLayersSection(manager);
            DrawConfigsSection();
            DrawRuntimeInfo(manager);
            DrawValidation(manager);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(ScreenManager manager)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Screen Manager", EditorStyles.boldLabel);

            if (Application.isPlaying)
            {
                var statusStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = Color.green }
                };
                EditorGUILayout.LabelField("[RUNTIME]", statusStyle, GUILayout.Width(70));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        private void DrawManagerId()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(_managerId, new GUIContent("Manager ID",
                "Unique identifier for this screen manager. Use different IDs for multiple managers."));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawLayersSection(ScreenManager manager)
        {
            _showLayers = EditorGUILayout.BeginFoldoutHeaderGroup(_showLayers, $"Layers ({_layers.arraySize})");

            if (_showLayers)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                for (int i = 0; i < _layers.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    var layerProp = _layers.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(layerProp, new GUIContent($"Layer {i}"));

                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        _layers.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("+ Add Layer", GUILayout.Width(100)))
                {
                    _layers.InsertArrayElementAtIndex(_layers.arraySize);
                }

                if (GUILayout.Button("Auto-Find Layers", GUILayout.Width(120)))
                {
                    AutoFindLayers(manager);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(5);
        }

        private void DrawConfigsSection()
        {
            _showConfigs = EditorGUILayout.BeginFoldoutHeaderGroup(_showConfigs, $"Screen Configs ({_configs.arraySize})");

            if (_showConfigs)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                if (_configs.arraySize > 5)
                {
                    _configScrollPosition = EditorGUILayout.BeginScrollView(_configScrollPosition, GUILayout.MaxHeight(200));
                }

                for (int i = 0; i < _configs.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    var configProp = _configs.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(configProp, new GUIContent($"Config {i}"));

                    var config = configProp.objectReferenceValue as ScreenConfig;
                    if (config != null && config.ResolveType())
                    {
                        EditorGUILayout.LabelField(config.ScreenType.Name, EditorStyles.miniLabel, GUILayout.Width(100));
                    }

                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        _configs.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (_configs.arraySize > 5)
                {
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("+ Add Config", GUILayout.Width(100)))
                {
                    _configs.InsertArrayElementAtIndex(_configs.arraySize);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(5);
        }

        private void DrawRuntimeInfo(ScreenManager manager)
        {
            if (!Application.isPlaying) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Runtime Info", EditorStyles.miniBoldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Layer Count", manager.LayerCount);
            EditorGUILayout.IntField("Config Count", manager.Configs.Count);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawValidation(ScreenManager manager)
        {
            var issues = ValidateManager();

            if (issues.Count > 0)
            {
                EditorGUILayout.HelpBox(string.Join("\n", issues), MessageType.Warning);
            }
        }

        private List<string> ValidateManager()
        {
            var issues = new List<string>();

            var managers = FindObjectsOfType<ScreenManager>();
            var idCounts = new Dictionary<int, int>();

            foreach (var m in managers)
            {
                var prop = new SerializedObject(m).FindProperty("_managerId");
                var id = prop.intValue;

                if (!idCounts.ContainsKey(id))
                    idCounts[id] = 0;
                idCounts[id]++;
            }

            if (idCounts.ContainsKey(_managerId.intValue) && idCounts[_managerId.intValue] > 1)
            {
                issues.Add($"• Manager ID {_managerId.intValue} is used by multiple managers in the scene.");
            }

            if (_layers.arraySize == 0)
            {
                issues.Add("• No layers configured. Add at least one ScreenLayer.");
            }

            for (int i = 0; i < _layers.arraySize; i++)
            {
                var layerProp = _layers.GetArrayElementAtIndex(i);
                if (layerProp.objectReferenceValue == null)
                {
                    issues.Add($"• Layer {i} is not assigned.");
                }
            }

            for (int i = 0; i < _configs.arraySize; i++)
            {
                var configProp = _configs.GetArrayElementAtIndex(i);
                if (configProp.objectReferenceValue == null)
                {
                    issues.Add($"• Config {i} is not assigned.");
                }
            }

            var seenTypes = new HashSet<string>();
            for (int i = 0; i < _configs.arraySize; i++)
            {
                var configProp = _configs.GetArrayElementAtIndex(i);
                var config = configProp.objectReferenceValue as ScreenConfig;

                if (config != null && config.ResolveType())
                {
                    var typeName = config.ScreenType.FullName;
                    if (seenTypes.Contains(typeName))
                    {
                        issues.Add($"• Duplicate config for type {config.ScreenType.Name}.");
                    }
                    else
                    {
                        seenTypes.Add(typeName);
                    }
                }
            }

            return issues;
        }

        private void AutoFindLayers(ScreenManager manager)
        {
            Undo.RecordObject(manager, "Auto-Find Layers");

            var layers = manager.GetComponentsInChildren<ScreenLayer>(true);

            serializedObject.Update();
            _layers.ClearArray();

            foreach (var layer in layers)
            {
                _layers.InsertArrayElementAtIndex(_layers.arraySize);
                _layers.GetArrayElementAtIndex(_layers.arraySize - 1).objectReferenceValue = layer;
            }

            serializedObject.ApplyModifiedProperties();

            Debug.Log($"[ScreenManager] Found {layers.Length} layers.");
        }
    }
}
