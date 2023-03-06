using UnityEditor;
using UnityEngine;

namespace StephanHooft.Dialogue.EditorScripts
{
    [CustomPropertyDrawer(typeof(DialogueProcessor.DialogueProcessorSettings))]
    public class DialogueProcessorSettingsDrawer : PropertyDrawer
    {
        private static bool expand = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            expand = EditorGUI.BeginFoldoutHeaderGroup(position, expand, label);
            if (expand)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                var dialogueSpeed = property.FindPropertyRelative("dialogueSpeed");
                var feedbackEnabled = property.FindPropertyRelative("feedbackEnabled");
                var feedbackFrequency = property.FindPropertyRelative("feedbackFrequency");
                EditorGUILayout.PropertyField(dialogueSpeed);
                EditorGUILayout.PropertyField(feedbackEnabled);
                if (feedbackEnabled.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(feedbackFrequency);
                    EditorGUI.indentLevel--;
                }
                if (EditorGUI.EndChangeCheck())
                    property.serializedObject.ApplyModifiedProperties();
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndFoldoutHeaderGroup();
            EditorGUI.EndProperty();
        }
    }
}
