using Ink.Runtime;
using System.Collections.Generic;
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
            if (!Application.isPlaying)
            {
                if (DrawMandatoryReferenceProperty<DialogueManager>(dialogueManager))
                    DrawEntryLabelProperty();
            }
            else
            {
                GUI.enabled = false;
                var value = startingKnot.stringValue;
                EditorGUILayout.TextField("Starting Knot", string.IsNullOrEmpty(value) ? "- Not set -" : value);
                GUI.enabled = true;
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

        private void DrawEntryLabelProperty()
        {
            var manager = (DialogueManager)dialogueManager.objectReferenceValue;
            var text = manager.AssetText;
            if(text == null)
            {
                EditorGUILayout.HelpBox(NoDialogueAsset, MessageType.Warning);
                startingKnot.stringValue = "";
            }
            else
            {
                story = new(text);
                var knots = GetAllKnots(story);
                var options = new string[knots.Length + 1];
                int selected = 0;
                options[0] = "- Not set -";
                var currentValue = startingKnot.stringValue;
                for (int i = 0; i < knots.Length; i++)
                {
                    var knot = knots[i];
                    options[i + 1] = knot;
                    if (currentValue == knot)
                        selected = i + 1;
                }
                var newSelected = EditorGUILayout.Popup("Starting Knot", selected, options);
                startingKnot.stringValue = newSelected == 0 ? "" : options[newSelected];
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Static Methods

        private static string[] GetAllKnots(Story story, bool includeStitches = false)
        {
            var keys = story.mainContentContainer.namedOnlyContent.Keys;
            var output = new List<string>();
            foreach (var knot in keys)
                if (!knot.Contains(' '))
                {
                    output.Add(knot);
                    if (includeStitches)
                    {
                        var container = story.KnotContainerWithName(knot);
                        foreach (var stitch in container.namedContent.Keys)
                            output.Add(string.Format("{0}.{1}", knot, stitch));
                    }
                }
            return output.ToArray();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Error Messages

        private static string MustReferenceA(System.Type type)
            => $"{DialogueManager} must reference a {type.Name}";

        private static string NoDialogueAsset
            => $"The {DialogueManager} is not referencing a {DialogueAsset}, " +
            $"or the {DialogueAsset} is missing a text file. No starting knot can be set.";

        private static string DialogueAsset
            => typeof(Data.DialogueAsset).Name;
        private static string DialogueManager
            => typeof(DialogueManager).Name;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
