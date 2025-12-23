using Ink.Runtime;
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
        private SerializedProperty dialogueVariables;
        private SerializedProperty debug;
        private DialogueManager manager;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Editor Implementation

        private void OnEnable()
        {
            dialogueAsset = serializedObject.FindProperty("dialogueAsset");
            dialogueVariables = serializedObject.FindProperty("dialogueVariablesAsset");
            debug = serializedObject.FindProperty("debug");
            manager = (DialogueManager)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var enabled = GUI.enabled;
            if (manager.DialogueInProgress)
                GUI.enabled = false;
            EditorGUILayout.PropertyField(dialogueAsset);
            EditorGUILayout.PropertyField(dialogueVariables);
            GUI.enabled = enabled;
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(debug, new GUIContent("Debug Mode"));

            serializedObject.ApplyModifiedProperties(); // What happens if we don't have this?

            if (debug.boolValue && Application.isPlaying)
                DrawEditorTestInterface(manager);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        private static void DrawEditorTestInterface(DialogueManager manager)
        {
            DrawSeparator();
            EditorGUILayout.LabelField("Status", GetCurrentStatus(manager));
            if (manager.Text != null && !manager.DialogueInProgress)
            {
                if (GUILayout.Button("Start Dialogue"))
                {
                    var startingKnot = manager.StartingKnot;
                    if (startingKnot != null && startingKnot != "")
                        manager.StartDialogue(startingKnot);
                    else
                        manager.StartDialogue();
                }
            }
            if (manager.DialogueInProgress)
            {
                var cue = manager.CurrentDialogueLine.cue;
                switch (cue)
                {
                    case DialogueCue.CanContinue:
                        if (GUILayout.Button("Advance Dialogue"))
                            manager.AdvanceDialogue();
                        break;
                    case DialogueCue.Choice:
                        var choices = manager.CurrentDialogueLine.choices;
                        foreach (var choice in choices)
                            if (GUILayout.Button($"Dialogue Choice {choice.index}"))
                                manager.AdvanceDialogue(choice.index);
                        break;
                }
                EditorGUILayout.Space();
                if (GUILayout.Button("Stop Dialogue"))
                    manager.StopDialogue();
            }
        }

        private static string GetCurrentStatus(DialogueManager manager)
            => manager.CurrentDialogueLine.cue switch
            {
                DialogueCue.None => "Inactive",
                DialogueCue.CanContinue => "Can Continue",
                DialogueCue.Choice => "Awaiting Choice",
                DialogueCue.EndReached => "End Reached",
                _ => throw new System.NotImplementedException(),
            };

        private static void DrawSeparator()
            => EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
