using UnityEditor;
using UnityEngine;

namespace StephanHooft.Dialogue.EditorScripts
{
    [CustomEditor(typeof(DialogueManager))]
    public sealed class DialogueManagerInspector : Editor
    {
        #region Fields

        private SerializedProperty dialogueAsset;
        private SerializedProperty variablesAsset;
        private SerializedProperty debugMode;
        private DialogueManager manager;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Editor Implementation

        private void OnEnable()
        {
            dialogueAsset = serializedObject.FindProperty("dialogueAsset");
            variablesAsset = serializedObject.FindProperty("variablesAsset");
            debugMode = serializedObject.FindProperty("debugMode");
            manager = serializedObject.targetObject as DialogueManager;
        }

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying && manager.DialogueInProgress)
                manager.StopDialogue();
            serializedObject.Update();
            var enabled = GUI.enabled;
            if (manager.DialogueInProgress)
                GUI.enabled = false;
            EditorGUILayout.PropertyField(dialogueAsset);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(variablesAsset);
            GUI.enabled = enabled;
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(debugMode);
            DrawDivider();
            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying)
                DrawEditorTestInterface(manager);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        private static void DrawEditorTestInterface(DialogueManager manager)
        {
            EditorGUILayout.LabelField("Status", GetCurrentStatus(manager));
            if (manager.Text != null && !manager.DialogueInProgress)
            {
                if (GUILayout.Button("Start Dialogue"))
                {
                    var startingKnot = manager.StartingKnot;
                    var startingStitch = manager.StartingStitch;
                    manager.StartDialogue(startingKnot, startingStitch);
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

        private static void DrawDivider()
            => EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
