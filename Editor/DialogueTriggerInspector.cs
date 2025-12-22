using Ink.Runtime;
using UnityEditor;
using UnityEngine;

namespace StephanHooft.Dialogue.EditorScripts
{
    /// <summary>
    /// Custom inspector for the <see cref="DialogueTrigger"/> class.
    /// </summary>
    [CustomEditor(typeof(DialogueTrigger))]
    [CanEditMultipleObjects]
    public class DialogueTriggerInspector : Editor
    {
        #region Fields

        private SerializedProperty dialogueManager;
        private SerializedProperty startingKnot;
        private Story story;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Editor Implementation

        private void OnEnable()
        {
            dialogueManager = serializedObject.FindProperty("dialogueManager");
            startingKnot = serializedObject.FindProperty("startingKnot");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            if (DrawMandatoryReferenceProperty<DialogueManager>(dialogueManager))
            {
                EditorGUI.indentLevel++;
                var manager = (DialogueManager)dialogueManager.objectReferenceValue;
                var text = manager.Text;
                story = new(text);
                startingKnot.stringValue = PropertyDrawers.DrawKnotProperty("Starting Knot", startingKnot.stringValue, story);
                EditorGUI.indentLevel--;
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

        private static string MustReferenceA(System.Type type)
            => $"{DialogueManager} must reference a {type.Name}";

        private static string DialogueManager
            => typeof(DialogueManager).Name;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
