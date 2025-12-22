using Ink.Runtime;
using System.Collections.Generic;

namespace StephanHooft.Dialogue
{
    public static class StoryExtensions
    {
        #region Static Methods

        public static bool ChoicesAvailable(this Story story)
        {
            return story.currentChoices.Count > 0;
        }

        public static bool ContainsKnot(this Story story, string knot)
        {
            var container = story.KnotContainerWithName(knot);
            return container != null;
        }

        public static bool ContainsKnot(this Story story, string knot, string stitch)
        {
            var container = story.KnotContainerWithName(knot);
            if (container == null)
                return false;
            foreach (var stitchKey in container.namedContent.Keys)
                if (stitchKey == stitch)
                    return true;
            return false;
        }

        public static DialogueCue GetDialogueCue(this Story story)
        {
            if (story.canContinue)
                return DialogueCue.CanContinue;
            else if (ChoicesAvailable(story))
                return DialogueCue.Choice;
            else
                return DialogueCue.EndReached;
        }

        public static DialogueChoice[] GetDialogueChoices(this Story story)
        {
            var count = story.currentChoices.Count;
            DialogueChoice[] options = new DialogueChoice[count];
            for (int i = 0; i < count; i++)
            {
                var choice = story.currentChoices[i];
                var index = choice.index;
                var text = choice.text;
                var choiceHasTags = choice.tags != null && choice.tags.Count > 0;
                var tags = choiceHasTags
                    ? GetDialogueTags(story)
                    : new DialogueTag[0];
                options[i] = new(index, text, tags);
            }
            return options;
        }

        public static DialogueTag[] GetDialogueTags(this Story story)
        {
            var tags = story.currentTags;
            var count = tags.Count;
            var parsedTags = new DialogueTag[count];
            for (int i = 0; i < count; i++)
            {
                var tag = tags[i];
                var splitTag = tag.Trim(' ').Split(":");
                if (splitTag.Length != 2)
                    throw new System.ArgumentException($"Tag '{tag}' is incorrectly formatted.");
                parsedTags[i] = new(splitTag[0].Trim(), splitTag[1].Trim());
            }
            return parsedTags;
        }

        public static string[] GetKnots(this Story story, bool includeStitches = false)
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

        #endregion
    }
}
