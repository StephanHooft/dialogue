using Ink.Runtime;
using UnityEditor;
using UnityEngine;

namespace StephanHooft.Dialogue.EditorScripts
{
    /// <summary>
    /// A custom <see cref="PropertyDrawer"/> for the <see cref="DialogueAsset"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(DialogueAsset))]
    public class DialogueAssetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var asset = property.FindPropertyRelative("asset");
            var startingKnot = property.FindPropertyRelative("startingKnot");

            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var bufferedAssetValue = (TextAsset)asset.objectReferenceValue;
            EditorGUI.ObjectField(position, asset, label);
            var newAssetValue = (TextAsset)asset.objectReferenceValue;
            if (bufferedAssetValue != newAssetValue)
                startingKnot.stringValue = "";
            if (newAssetValue != null)
            {
                EditorGUI.indentLevel++;
                try
                {
                    var story = new Story(newAssetValue.text);
                    startingKnot.stringValue = DrawKnotProperty("Starting Knot", startingKnot.stringValue, story);
                    EditorGUI.indentLevel--;
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    Debug.LogError("The selected asset is not a valid Ink story.");
                    asset.objectReferenceValue = bufferedAssetValue;
                    EditorGUI.indentLevel--;
                    return;
                }
            }
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        private static string DrawKnotProperty(string label, string value, Story story)
        {
            if (!GetKnots(story, out var knots))
                return "";
            var options = new string[knots.Length + 1];
            int selected = 0;
            options[0] = "- Not set -";
            for (int i = 0; i < knots.Length; i++)
            {
                var knot = knots[i];
                options[i + 1] = knot;
                if (value == knot)
                    selected = i + 1;
            }
            var newSelected = EditorGUILayout.Popup(label, selected, options);
            return newSelected == 0 ? "" : options[newSelected];
        }

        private static bool GetKnots(Story story, out string[] knots)
        {
            knots = new string[0];
            if (story == null)
                return false;
            knots = story.GetKnots();
            if (knots.Length == 0)
                return false;
            else
                return true;
        }
    }
}
