using Ink.Runtime;
using StephanHooft.Dialogue.Data;
using UnityEditor;
using UnityEngine;

namespace StephanHooft.Dialogue.EditorScripts
{
    /// <summary>
    /// Custom inspector for the <see cref="DialogueManager"/> class.
    /// </summary>
    [CustomEditor(typeof(DialogueManager))]
    [CanEditMultipleObjects]
    public sealed class DialogueManagerInspector : Editor
    {
        #region Fields

        private SerializedProperty dialogueAsset;
        private SerializedProperty startingKnot;
        private SerializedProperty dialogueVariables;
        private Story story;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Editor Implementation

        private void OnEnable()
        {
            dialogueAsset = serializedObject.FindProperty("dialogueAsset");
            startingKnot = serializedObject.FindProperty("startingKnot");
            dialogueVariables = serializedObject.FindProperty("dialogueVariablesAsset");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            var enabled = GUI.enabled;
            if (Application.isPlaying)
                GUI.enabled = false;
            var bufferedAsset = (TextAsset)dialogueAsset.objectReferenceValue;
            EditorGUILayout.PropertyField(dialogueAsset);
            var asset = (TextAsset)dialogueAsset.objectReferenceValue;
            if (bufferedAsset != asset)
                startingKnot.stringValue = "";
            if(asset != null)
            {
                EditorGUI.indentLevel++;
                story = new(asset.text);
                startingKnot.stringValue = PropertyDrawers.DrawKnotProperty("Starting Knot", startingKnot.stringValue, story);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(dialogueVariables);
            GUI.enabled = enabled;
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
