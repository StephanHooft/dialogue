using Ink.Runtime;
using UnityEditor;
using UnityEngine;

namespace StephanHooft.Dialogue.EditorScripts
{
    public static class PropertyDrawers
    {
        /// <summary>
        /// Draws a control to select a specific knot (stored as a <see cref="string"/> value) in an ink <see cref="Story"/>.
        /// </summary>
        /// <param name="label">The <see cref="string"/> label for the control.</param>
        /// <param name="value">The current <see cref="string"/> value.</param>
        /// <param name="story">The <see cref="Story"/> to parse available knots from.</param>
        /// <returns>A <see cref="string"/> representing the selected knot.</returns>
        public static string DrawKnotProperty(string label, string value, Story story)
        {
            if (story == null)
                return "";
            if (!Application.isPlaying)
            {
                var knots = story.GetKnots();
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
    }
}
