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
            var startingStitch = property.FindPropertyRelative("startingStitch");

            var bufferedValue = asset.objectReferenceValue as TextAsset;
            var bufferedKnot = startingKnot.stringValue;
            var bufferedStitch = startingStitch.stringValue;
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.ObjectField(position, asset, label);
            var newValue = asset.objectReferenceValue as TextAsset;

            if (bufferedValue != newValue)
            {
                startingKnot.stringValue = "";
                startingStitch.stringValue = "";
            }
            if (newValue != null)
            {
                EditorGUI.indentLevel++;
                if (newValue.IsValidInkStory(out var story))
                {
                    startingKnot.stringValue = DrawKnotProperty("Starting Knot", startingKnot.stringValue, story);
                    startingStitch.stringValue = DrawStitchProperty("Starting Stitch", startingStitch.stringValue, startingKnot.stringValue, story);
                }
                else
                {
                    Debug.LogError($"{newValue.name} doet not contain valid Ink JSON.");
                    asset.objectReferenceValue = bufferedValue;
                    startingKnot.stringValue = bufferedKnot;
                    startingStitch.stringValue = bufferedStitch;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
        }

        private static string DrawKnotProperty(string label, string knotValue, Story story)
        {
            if (!GetKnots(story, out var knots))
                return "";
            var knotOptions = new string[knots.Length + 1];
            int selected = 0;
            knotOptions[0] = "- Not set -";
            for (int i = 0; i < knots.Length; i++)
            {
                var knot = knots[i];
                knotOptions[i + 1] = knot;
                if (knotValue == knot)
                    selected = i + 1;
            }
            var tooltipLabel = new GUIContent(label, "Determines which knot the dialogue asset should start from.");
            var selectedKnotIndex = EditorGUILayout.Popup(tooltipLabel, selected, knotOptions);
            return selectedKnotIndex > 0 ? knotOptions[selectedKnotIndex] : "";
        }

        private static string DrawStitchProperty(string label, string stitchValue, string knotValue, Story story)
        {
            if (!GetStitches(story, knotValue, out var stitches))
                return "";
            var stitchOptions = new string[stitches.Length + 1];
            int selected = 0;
            stitchOptions[0] = "- Not set -";
            for (int i = 0; i < stitches.Length; i++)
            {
                var stitch = stitches[i];
                stitchOptions[i + 1] = stitch;
                if (stitchValue == stitch)
                    selected = i + 1;
            }
            var tooltipLabel = new GUIContent
                (label, "Determines which stitch (within the selected knot) the dialogue asset should start from.");
            var selectedStitchIndex = EditorGUILayout.Popup(tooltipLabel, selected, stitchOptions);
            return selectedStitchIndex > 0 ? stitchOptions[selectedStitchIndex] : "";
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

        private static bool GetStitches(Story story, string knot, out string[] stitches)
        {
            stitches = new string[0];
            if (story == null)
                return false;
            stitches = story.GetStitches(knot);
            if (stitches.Length == 0)
                return false;
            //Debug.Log($"Found {stitches.Length} stitches in knot {knot}!");
            return true;
        }
    }
}
