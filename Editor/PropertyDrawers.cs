using Ink.Runtime;
using UnityEditor;
using UnityEngine;

namespace StephanHooft.Dialogue.EditorScripts
{
    /// <summary>
    /// A clas with helper functions to draw custom Inspectors for Dialogue scripts.
    /// </summary>
    public static class PropertyDrawers
    {
        /// <summary>
        /// Draws a control for a dialogue asset to source an ink <see cref="Story"/>. If possible, this also draws a second control to select a specific knot (stored as a <see cref="string"/> value).
        /// </summary>
        /// <param name="dialogueAsset">The <see cref="TextAsset"/> <see cref="SerializedProperty"/> to use for the dialogue asset control.</param>
        /// <param name="startingKnot">The <see cref="string"/> <see cref="SerializedProperty"/> to use for the starting knot control.</param>
        /// <param name="story">The <see cref="Story"/> to parse available knots from.</param>
        /// <param name="allowEditingKnot">Set to false to draw a read-only property.</param>
        public static void DrawDialogueAssetProperty(SerializedProperty dialogueAsset, SerializedProperty startingKnot, Story story, bool allowEditingKnot = true)
        {
            // Draw Dialogue Asset properties
            var bufferedAsset = (TextAsset)dialogueAsset.objectReferenceValue;
            EditorGUILayout.PropertyField(dialogueAsset);
            var asset = (TextAsset)dialogueAsset.objectReferenceValue;
            if (bufferedAsset != asset)
                startingKnot.stringValue = "";
            if (asset != null)
            {
                EditorGUI.indentLevel++;
                story = new(asset.text);
                startingKnot.stringValue = PropertyDrawers.DrawKnotProperty("Starting Knot", startingKnot.stringValue, story, allowEditingKnot);
                EditorGUI.indentLevel--;
            }
        }

        private static string DrawKnotProperty(string label, string value, Story story, bool allowEditing)
        {
            if(!GetKnots(story, out var knots))
                return "";
            if (allowEditing)
            {
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
            else
            {
                var enabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUILayout.LabelField(label, string.IsNullOrEmpty(value) ? "- Not set -" : value);
                GUI.enabled = enabled;
                return value;
            }
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
