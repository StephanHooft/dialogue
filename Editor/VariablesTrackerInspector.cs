using UnityEditor;
using UnityEngine;

namespace StephanHooft.Dialogue.EditorScripts
{
    [CustomEditor(typeof(VariablesTracker))]
    public class VariablesTrackerInspector : Editor
    {
        #region Fields
        private SerializedProperty variablesAsset;
        private SerializedProperty debugMode;
        private SerializedProperty saveData;
        private SerializedProperty storeSavedDataInTracker;
        private VariablesTracker variablesTracker;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Editor Implementation

        private void OnEnable()
        {
            variablesAsset = serializedObject.FindProperty("variablesAsset");
            debugMode = serializedObject.FindProperty("debugMode");
            saveData = serializedObject.FindProperty("saveData");
            storeSavedDataInTracker = serializedObject.FindProperty("storeSavedDataInTracker");
            variablesTracker = serializedObject.targetObject as VariablesTracker;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (!Application.isPlaying)
            {
                var bufferedAsset = variablesAsset.objectReferenceValue as TextAsset;
                EditorGUILayout.PropertyField(variablesAsset);
                var newAsset = variablesAsset.objectReferenceValue as TextAsset;
                if (newAsset != null)
                {
                    if (!newAsset.IsValidInkStory(out _))
                    {
                        Debug.LogError($"{newAsset.name} doet not contain valid Ink JSON.");
                        variablesAsset.objectReferenceValue = bufferedAsset;
                    }
                }
            }
            EditorGUILayout.PropertyField(storeSavedDataInTracker);
            if(!storeSavedDataInTracker.boolValue && !Application.isPlaying && saveData.stringValue.Length > 0)
            {
                saveData.stringValue = null;
                Debug.Log($"{variablesTracker.name}'s save data was removed.");
            }
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(debugMode);
            serializedObject.ApplyModifiedProperties();
            DrawDivider();
            if (Application.isPlaying && !variablesTracker.Tracking)
            {
                if (storeSavedDataInTracker.boolValue)
                {
                    if (GUILayout.Button("Save Variables"))
                        variablesTracker.SaveVariables();
                    if (GUILayout.Button("Load Variables"))
                        variablesTracker.LoadVariables();
                    EditorGUILayout.Separator();
                }
                if (GUILayout.Button("Reset Variables"))
                    variablesTracker.ResetVariables();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        private static void DrawDivider()
            => EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
