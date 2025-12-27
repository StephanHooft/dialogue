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
        private VariablesTracker variablesTracker;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Editor Implementation

        private void OnEnable()
        {
            variablesAsset = serializedObject.FindProperty("variablesAsset");
            debugMode = serializedObject.FindProperty("debugMode");
            variablesTracker = serializedObject.targetObject as VariablesTracker;
        }

        public override void OnInspectorGUI()
        {
            //if (!Application.isPlaying && variablesTracker.Initialised)
            //    variablesTracker.ResetVariables();
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
                EditorGUILayout.Separator();
            }
            EditorGUILayout.PropertyField(debugMode);
            serializedObject.ApplyModifiedProperties();
            DrawDivider();
            if (!variablesTracker.Tracking)
            {
                if (GUILayout.Button("Save Variables")) { }
                if (GUILayout.Button("Load Variables")) { }
                EditorGUILayout.Separator();
                if (GUILayout.Button("Reset Variable Defaults")) { }
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
