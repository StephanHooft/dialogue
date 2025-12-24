using Ink.Runtime;
using System.Collections.Generic;

namespace StephanHooft.Dialogue
{
    /// <summary>
    /// Extension methods for <see cref="Story"/>.
    /// </summary>
    public static class StoryExtensions
    {
        #region Static Methods

        /// <summary>
        /// Returns <see cref="true"/> if the <see cref="Story"/> currently requires a choice to be made.
        /// </summary>
        public static bool ChoicesAvailable(this Story story)
        {
            return story.currentChoices.Count > 0;
        }

        /// <summary>
        /// Returns <see cref="true"/> if the <see cref="Story"/> contains a knot with the specified <see cref="string"/> name.
        /// </summary>
        /// <param name="knot">The <see cref="string"/> knot to check for.</param>
        public static bool ContainsKnot(this Story story, string knot)
        {
            var container = story.KnotContainerWithName(knot);
            return container != null;
        }

        /// <summary>
        /// Returns <see cref="true"/> if the <see cref="Story"/> contains a knot + stitch combo with the specified <see cref="string"/> names.
        /// </summary>
        /// <param name="knot">The <see cref="string"/> knot to check for.</param>
        /// <param name="stitch">The <see cref="string"/> stitch to check for.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the <see cref="DialogueCue"/> for the <see cref="Story"/>'s current state.
        /// </summary>
        public static DialogueCue GetDialogueCue(this Story story)
        {
            if (story.canContinue)
                return DialogueCue.CanContinue;
            else if (story.ChoicesAvailable())
                return DialogueCue.Choice;
            else
                return DialogueCue.EndReached;
        }

        /// <summary>
        /// Returns an array of <see cref="DialogueChoice"/>s for the <see cref="Story"/>'s current state.
        /// </summary>
        public static DialogueChoice[] GetDialogueChoices(this Story story)
        {
            var count = story.currentChoices.Count;
            DialogueChoice[] options = new DialogueChoice[count];
            for (int i = 0; i < count; i++)
            {
                var choice = story.currentChoices[i];
                var index = choice.index;
                var text = choice.text;
                var tags = choice.GetChoiceTags();
                options[i] = new(index, text, tags);
            }
            return options;
        }

        /// <summary>
        /// Returns an array of <see cref="string"/> tags for a dialogue <see cref="Choice"/>.
        /// </summary>
        public static string[] GetChoiceTags(this Choice choice)
        {
            if (choice.tags == null || choice.tags.Count == 0)
                return new string[0];
            return choice.tags.ToArray();
        }

        /// <summary>
        /// Returns the <see cref="Story"/>'s global <see cref="string"/> tags.
        /// </summary>
        public static string[] GetGlobalTags(this Story story)
        {
            var tags = story.globalTags;
            if (tags == null)
                return new string[0];
            else
                return tags.ToArray();
        }

        /// <summary>
        /// Returns an array of <see cref="string"/> tags for a specific knot in the <see cref="Story"/>.
        /// </summary>
        /// <param name="knot">The <see cref="string"/> name of the knot to get tags (if any) for.</param>
        public static string[] GetKnotTags(this Story story, string knot)
        {
            if (knot == null || knot == "" || !story.ContainsKnot(knot))
                return new string[0];
            var tags = story.TagsForContentAtPath(knot);
            if (tags == null)
                return new string[0];
            return story.TagsForContentAtPath(knot).ToArray();
        }

        /// <summary>
        /// Returns an array of <see cref="string"/> dialogue tags for the <see cref="Story"/>'s current state.
        /// </summary>
        public static string[] GetLineTags(this Story story)
        {
            if (story.currentTags == null || story.currentTags.Count == 0)
                return new string[0];
            return story.currentTags.ToArray();
        }

        /// <summary>
        /// Returns an array of the <see cref="string"/> knots in the <see cref="Story"/>.
        /// <para>Note that this method will reset the <see cref="Story"/> in order to test valid knots!</para>
        /// </summary>
        public static string[] GetKnots(this Story story)
        {
            if (story.mainContentContainer.namedOnlyContent == null)
                return new string[0];
            var keys = story.mainContentContainer.namedOnlyContent.Keys;
            var output = new List<string>();
            foreach (var knot in keys)
            {
                // Not all knots are legal. (For example functions are also "Knots".)
                // This block tests whether each purported knot would trigger a StoryException.
                story.ResetState();
                story.ChoosePathString(knot);
                try
                {
                    story.Continue();
                    if (!knot.Contains(' '))
                    {
                        output.Add(knot);
                    }
                }
                catch (StoryException) { }
            }
            story.ResetState();
            return output.ToArray();
        }

        /// <summary>
        /// Returns an array of the <see cref="string"/> strings in one of the <see cref="Story"/>'s knots.
        /// </summary>
        /// <param name="knot">The <see cref="string"/> name of the knot to get stitches (if any) for.</param>
        public static string[] GetStitches(this Story story, string knot)
        {
            var container = story.KnotContainerWithName(knot);
            if (container == null)
                return new string[0];
            var output = new List<string>();
            foreach (var stitch in container.namedContent.Keys)
                output.Add(stitch);
            return output.ToArray();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
