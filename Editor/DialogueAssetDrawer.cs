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

            label = EditorGUI.BeginProperty(position, label, property);
            var bufferedAssetValue = (TextAsset)asset.objectReferenceValue;
            EditorGUI.ObjectField(position, asset, label);
            var newAssetValue = (TextAsset)asset.objectReferenceValue;
            if (bufferedAssetValue != newAssetValue)
            {
                startingKnot.stringValue = "";
                startingStitch.stringValue = "";
            }
            if (newAssetValue != null)
            {
                EditorGUI.indentLevel++;
                try
                {
                    var story = new Story(newAssetValue.text);
                    startingKnot.stringValue = DrawKnotProperty("Starting Knot", startingKnot.stringValue, story);
                    startingStitch.stringValue = DrawStitchProperty("Starting Stitch", startingStitch.stringValue, startingKnot.stringValue, story);
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    Debug.LogError("The selected asset is not a valid Ink story.");
                    asset.objectReferenceValue = bufferedAssetValue;
                    startingKnot.stringValue = "";
                    startingStitch.stringValue = "";
                    return;
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
            var selectedKnotIndex = EditorGUILayout.Popup(label, selected, knotOptions);
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
            var selectedStitchIndex = EditorGUILayout.Popup(label, selected, stitchOptions);
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
