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

        private SerializedProperty dialogueProcessor;
        private SerializedProperty dialogueVariables;
        private SerializedProperty dialogueAsset;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Editor Implementation

        private void OnEnable()
        {
            dialogueProcessor = serializedObject.FindProperty("dialogueProcessor");
            dialogueVariables = serializedObject.FindProperty("dialogueVariablesAsset");
            dialogueAsset = serializedObject.FindProperty("dialogueAsset");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            if (!Application.isPlaying)
            {
                if (DrawMandatoryReferenceProperty<DialogueAsset>(dialogueAsset))
                {
                    var asset = dialogueAsset.objectReferenceValue as DialogueAsset;
                    try { var textAsset = asset.Text; }
                    catch (UnityEngine.MissingReferenceException)
                    { EditorGUILayout.HelpBox(NoTextAsset, MessageType.Warning); }
                }
                DrawMandatoryReferenceProperty<DialogueProcessor>(dialogueProcessor);
                EditorGUILayout.PropertyField(dialogueVariables);
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        private bool DrawMandatoryReferenceProperty<T>(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property);
            if (property.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(MustReferenceA(typeof(T)), MessageType.Warning);
                return false;
            }
            return true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Error Messages

        private string NoTextAsset
            => $"The {typeof(DialogueAsset).Name} is missing a {typeof(UnityEngine.TextAsset).Name}.";

        private string MustReferenceA(System.Type type) => string.Format("{0} must reference a {1}",
            typeof(DialogueManager).Name, type.Name);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
