using Ink.Runtime;
using StephanHooft.Dialogue.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StephanHooft.Dialogue
{
    /// <summary>
    /// A <see cref="MonoBehaviour"/> that encapsulates an ink-based story for Unity. Only one ink story can be active at
    /// once, and the story cannot be swapped at runtime.
    /// This class can be used directly, or through a <see cref="DialogueTrigger"/>. A <see cref="DialogueAsset"/> and
    /// <see cref="DialogueProcessor"/> must be assigned in-Editor for it to work.
    /// <para>
    /// Optionally, a <see cref="DialogueVariables"/> can be assigned in-Editor if the state of variables should be
    /// stored or preserved between sessions and/or scenes.
    /// </para>
    /// </summary>
    public sealed class DialogueManager : MonoBehaviour
    {
        #region Events

        /// <summary>
        /// Invoked when the <see cref="DialogueManager"/> begins a story.
        /// </summary>
        public event Action OnDialogueBegin;

        /// <summary>
        /// Invoked when the <see cref="DialogueManager"/> stops progressing a story.
        /// </summary>
        public event Action OnDialogueEnd;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Properties

        /// <summary>
        /// The <see cref="DialogueAsset"/> assigned to the <see cref="DialogueManager"/>.
        /// </summary>
        public DialogueAsset Asset
        {
            get { return dialogueAsset; }
            set
            {
                if (DialogueInProgress)
                    throw Exceptions.DialogueAlreadyInProgress;
                story = value is not null ? CreateNewStory(value.Text) : null;
                dialogueAsset = value;
            }
        }

        /// <summary>
        /// Whether the ink story can be advanced. (Only applicable if a story is in progress.)
        /// </summary>
        public bool CanContinueDialogue
            => DialogueInProgress && story.canContinue;

        /// <summary>
        /// True if the <see cref="DialogueManager"/> is currently running an ink story.
        /// </summary>
        public bool DialogueInProgress
            => inProgress;

        /// <summary>
        /// True if the <see cref="DialogueManager"/> is running an ink story, which is waiting on a choice.
        /// </summary>
        public bool DialogueChoicesAvailable
            => DialogueInProgress && ChoicesAvailable(story);

        /// <summary>
        /// The <see cref="DialogueProcessor"/> assigned to the <see cref="DialogueManager"/>.
        /// </summary>
        public DialogueProcessor Processor
        {
            get { return dialogueProcessor; }
            set
            {
                if (DialogueInProgress)
                    throw Exceptions.DialogueAlreadyInProgress;
                dialogueProcessor = value;
            }
        }

        /// <summary>
        /// True if the <see cref="DialogueManager"/> is waiting on its assigned <see cref="DialogueProcessor"/> to
        /// process a <see cref="DialogueLine"/>.
        /// </summary>
        public bool WaitingForDialogueLineToProcess
            => dialogueProcessor.ProcessingDialogueLine;

        private bool TrackingVariables
            => dialogueVariablesAsset != null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Fields

        [SerializeField]
        private DialogueAsset dialogueAsset;

        [SerializeField]
        private DialogueProcessor dialogueProcessor;

        [SerializeField]
        [Header("Dialogue Variables (Optional)")]
        private DialogueVariables dialogueVariablesAsset;

        private Story story;
        private bool inProgress = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region MonoBehaviour Implementation

        private void Awake()
        {
            if (dialogueAsset != null)
                story = CreateNewStory(dialogueAsset.Text);
            if (TrackingVariables)
                dialogueVariablesAsset.Initialise();
        }

        private void OnDestroy()
        {
            if (TrackingVariables)
                dialogueVariablesAsset.UnInitialise();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        /// <summary>
        /// Advance the ink story.
        /// </summary>
        /// </param>
        public void Advance()
        {
            if (!DialogueInProgress)
                throw Exceptions.NoDialogueInProgress;
            if (!CanContinueDialogue)
                throw Exceptions.StoryCannotContinue;
            ProcessNextDialogueLine();
        }

        /// <summary>
        /// Begin the ink story.
        /// </summary>
        public void Begin()
        {
            if (DialogueInProgress)
                throw Exceptions.DialogueAlreadyInProgress;
            Exceptions.ThrowIfNull(dialogueAsset, "dialogueAsset");
            Exceptions.ThrowIfNull(dialogueProcessor, "dialogueProcessor");
            story.ResetState();
            inProgress = true;
            dialogueProcessor.OpenDialogueInterface();
            if (TrackingVariables)
                dialogueVariablesAsset.StartListening(story);
            OnDialogueBegin?.Invoke();
            ProcessNextDialogueLine();
        }

        /// <summary>
        /// Begin the ink story.
        /// </summary>
        /// <param name="startingKnot">
        /// The <see cref="string"/> name of the knot at which to begin the ink story.
        /// </param>
        public void Begin(string startingKnot)
        {
            if (DialogueInProgress)
                throw Exceptions.DialogueAlreadyInProgress;
            Exceptions.ThrowIfNull(dialogueAsset, "dialogueAsset");
            Exceptions.ThrowIfNull(dialogueProcessor, "dialogueProcessor");
            story.ResetState();
            JumpToKnot(startingKnot);
            inProgress = true;
            dialogueProcessor.OpenDialogueInterface();
            if(TrackingVariables)
                dialogueVariablesAsset.StartListening(story);
            OnDialogueBegin?.Invoke();
            ProcessNextDialogueLine();
        }

        /// <summary>
        /// Stop running the ink story.
        /// </summary>
        public void End()
        {
            if (!DialogueInProgress)
                throw Exceptions.NoDialogueInProgress;
            inProgress = false;
            if (TrackingVariables)
                dialogueVariablesAsset.StopListening(story);
            dialogueProcessor.CloseDialogueInterface();
            OnDialogueEnd?.Invoke();
        }

        /// <summary>
        /// Load the values of the ink story's global variables from a JSON file.
        /// </summary>
        /// <param name="json">
        /// A chunk of JSON containing the variables to load.
        /// </param>
        public void LoadGlobalVariables(string json)
        {
            if (!TrackingVariables)
                throw Exceptions.NotTrackingGlobalVariables;
            if (DialogueInProgress)
                throw Exceptions.NoSaveLoadDuringDialogue;
            Exceptions.ThrowIfNull(dialogueAsset, "dialogueAsset");
            dialogueVariablesAsset.LoadVariables(json, story);
        }

        /// <summary>
        /// If waiting on a <see cref="DialogueLine"/> to be processed, this method can be used to "rush" that line.
        /// </summary>
        public void RushCurrentDialogueLine()
        {
            dialogueProcessor.RushCurrentDialogueLine();
        }

        /// <summary>
        /// Save the values of the ink story's global variables to a JSON file.
        /// </summary>
        /// <returns>
        /// A chunk of JSON containing the saved variables.
        /// </returns>
        public string SaveGlobalVariables()
        {
            if (DialogueInProgress)
                throw Exceptions.NoSaveLoadDuringDialogue;
            Exceptions.ThrowIfNull(dialogueAsset, "dialogueAsset");
            return TrackingVariables
                ? dialogueVariablesAsset.SaveVariables()
                : throw Exceptions.NotTrackingGlobalVariables;
        }

        private Story CreateNewStory(string text)
        {
            story = new(text);
            story.onError += (msg, type) =>
            {
                if (type == Ink.ErrorType.Warning)
                    Debug.LogWarning(msg);
                else
                    Debug.LogError(msg);
            };
            return story;
        }

        private DialogueCue GetDialogueCue()
        {
            if (story.canContinue)
                return DialogueCue.CanContinue;
            else if (!ChoicesAvailable(story))
                return DialogueCue.EndReached;
            else
                return DialogueCue.None;
        }

        private DialogueChoice[] GetDialogueChoices()
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
                    ? ParseTags(choice.tags)
                    : new DialogueTag[0];
                options[i] = new(index, text, tags, SelectDialogChoice);
            }
            return options;
        }

        private void JumpToKnot(string knotAddress)
        {
            if (string.IsNullOrEmpty(knotAddress))
                throw Exceptions.KnotIsNull;
            if (knotAddress.Contains("."))
            {
                var splitString = knotAddress.Split(".");
                if (splitString.Length > 2)
                    throw Exceptions.InvalidKnotFormat(knotAddress);
                var knot = splitString[0];
                var stitch = splitString[1];
                if (!ContainsKnot(story, knot, stitch))
                    throw Exceptions.KnotPlusStitchDoesNotExist(knot, stitch);
            }
            else if (!ContainsKnot(story, knotAddress))
                throw Exceptions.KnotDoesNotExist(knotAddress);
            story.ChoosePathString(knotAddress);
        }

        private DialogueTag[] ParseTags(List<string> tags)
        {
            var count = tags.Count;
            var parsedTags = new DialogueTag[count];
            for(int i = 0; i < count; i++)
            {
                var tag = tags[i];
                var splitTag = tag.Split(":");
                if (splitTag.Length != 2)
                    throw Exceptions.TagIncorrectlyFormatted(tag);
                parsedTags[i] = new(splitTag[0].Trim(), splitTag[1].Trim());
            }
            return parsedTags;
        }

        private void ProcessNextDialogueLine()
        {
            var text = story.Continue();
            var tags = ParseTags(story.currentTags);
            var options = GetDialogueChoices();
            var line = new DialogueLine(text, tags, options);
            var cue = GetDialogueCue();
            dialogueProcessor.ProcessDialogueLine(line, cue, ProcessNextDialogueLine);
        }

        private void SelectDialogChoice(int index)
        {
            var count = story.currentChoices.Count;
            if(count == 0)
                throw Exceptions.NoOptionsAvailable;
            if (index >= count)
                throw Exceptions.IndexOutOfRange(index, count);
            story.ChooseChoiceIndex(index);
            ProcessNextDialogueLine();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Static Methods

        private static bool ContainsKnot(Story story, string knot)
        {
            var container = story.KnotContainerWithName(knot);
            return container != null;
        }

        private static bool ContainsKnot(Story story, string knot, string stitch)
        {
            var container = story.KnotContainerWithName(knot);
            if (container == null)
                return false;
            foreach (var stitchKey in container.namedContent.Keys)
                if (stitchKey == stitch)
                    return true;
            return false;
        }

        private static bool ChoicesAvailable(Story story)
        {
            return story.currentChoices.Count > 0;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Exceptions

        private static class Exceptions
        {
            public static InvalidOperationException DialogueAlreadyInProgress
                => new("Cannot perform operation while a dialogue is already in progress.");

            public static IndexOutOfRangeException IndexOutOfRange(int index, int count)
                => new($"Choice with index {index} is invalid. Only {count} choices are available.");

            public static ArgumentException InvalidKnotFormat(string knot)
                => new($"Knot address '{knot}' is incorrectly formatted.");

            public static ArgumentException KnotPlusStitchDoesNotExist(string knot, string stitch)
                => new($"Knot+Stitch address '{knot}.{stitch}' does not exist in the story.");

            public static ArgumentException KnotDoesNotExist(string knot)
                => new($"Knot address '{knot}' does not exist in the story.");

            public static ArgumentException KnotIsNull
                => new("Knot address cannot be null or empty.");

            public static InvalidOperationException NoDialogueInProgress
                => new("No dialogue is currently in progress");

            public static InvalidOperationException NoOptionsAvailable
                => new("No dialogue options are available.");

            public static InvalidOperationException NoSaveLoadDuringDialogue
                => new("The variables of a Story may not be adjusted while a Dialogue is in progress.");

            public static InvalidOperationException NotTrackingGlobalVariables
                => new($"Method cannot be called: No {GlobalVariablesName} has been set.");

            public static InvalidOperationException StoryCannotContinue
                => new("The story cannot continue further.");

            public static ArgumentException TagIncorrectlyFormatted(string tag)
                => new($"Tag '{tag}' is incorrectly formatted.");

            public static void ThrowIfNull(object argument, string paramName)
            {
                if (argument == null)
                    throw new ArgumentNullException(paramName);
            }

            private static string GlobalVariablesName
                => typeof(DialogueVariables).Name;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
