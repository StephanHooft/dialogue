using Ink.Runtime;
using StephanHooft.Dialogue.Data;
using UnityEngine;

namespace StephanHooft.Dialogue
{
    /// <summary>
    /// A <see cref="MonoBehaviour"/> that encapsulates an ink-based story for Unity to generate dialogue feeds.
    /// This class can be addressed directly, or can be activated through a <see cref="DialogueTrigger"/>.
    /// <para>
    /// Optionally, a <see cref="DialogueVariables"/> can be assigned in-Editor if the state of variables should be
    /// stored or preserved between sessions and/or scenes.
    /// </para>
    /// </summary>
    public sealed class DialogueManager : MonoBehaviour
    {
        #region Events

        /// <summary>
        /// Invoked when the <see cref="DialogueManager"/> starts progressing a dialogue.
        /// </summary>
        public event System.Action<DialogueManager> OnDialogueStart;

        /// <summary>
        /// Invoked when the <see cref="DialogueManager"/> stops progressing a dialogue.
        /// </summary>
        public event System.Action<DialogueManager> OnDialogueEnd;

        /// <summary>
        /// Invoked when the <see cref="DialogueManager"/> processes a new dialogue line.
        /// </summary>
        public event System.Action<DialogueManager, DialogueLine> OnNewDialogueLine;

        /// <summary>
        /// Invoked when the <see cref="DialogueManager"/> changes a dialogue variable.
        /// </summary>
        public event System.Action<DialogueManager, string, Ink.Runtime.Object> OnVariableChanged;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Properties

        /// <summary>
        /// Whether the dialogue can be advanced. (Only applicable if a dialogue is in progress.)
        /// </summary>
        public bool CanContinueDialogue
            => DialogueInProgress && story.canContinue;

        /// <summary>
        /// The current <see cref="DialogueCue"/>, indicating the state of a dialogue in progress.
        /// </summary>
        public DialogueCue CurrentDialogueCue
            => inProgress ? DialogueCue.None : line.cue;

        /// <summary>
        /// The most recent <see cref="DialogueLine"/> processed by the <see cref="DialogueManager"/>.
        /// </summary>
        public DialogueLine CurrentDialogueLine
            => line;

        /// <summary>
        /// True if the <see cref="DialogueManager"/> is currently running a dialogue.
        /// </summary>
        public bool DialogueInProgress
            => inProgress;

        public string Text
            => dialogueAsset != null ? dialogueAsset.text : null;

        private bool TrackingVariables
            => dialogueVariablesAsset != null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Fields

        [SerializeField] private TextAsset dialogueAsset;
        [SerializeField] private string startingKnot;

        [SerializeField]
        [Header("Dialogue Variables (Optional)")]
        private DialogueVariables dialogueVariablesAsset;

        private Story story;
        private bool inProgress = false;
        private DialogueLine line = new();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region MonoBehaviour Implementation

        private void Awake()
        {
            if (dialogueAsset != null)
                story = CreateNewStory(dialogueAsset.text);
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
        /// Begin a dialogue.
        /// </summary>
        /// <param name="startingKnot">
        /// The <see cref="string"/> name of the knot at which to begin the dialogue.
        /// </param>
        public void Begin(string startingKnot = null)
        {
            if (DialogueInProgress)
                throw Exceptions.DialogueAlreadyInProgress;
            Exceptions.ThrowIfNull(dialogueAsset, "dialogueAsset");
            story.ResetState();
            if (startingKnot != null && startingKnot != "") JumpToKnot(startingKnot);
            if (TrackingVariables)
            {
                story.variablesState.variableChangedEvent += VariableChanged;
                dialogueVariablesAsset.StartListening(story);
            }
            OnDialogueStart?.Invoke(this);
            inProgress = true;
            ProcessNextDialogueLine();
        }

        /// <summary>
        /// Stop running the current dialogue.
        /// </summary>
        public void End()
        {
            if (!DialogueInProgress)
                throw Exceptions.NoDialogueInProgress;
            inProgress = false;
            line = new();
            if (TrackingVariables)
                dialogueVariablesAsset.StopListening(story);
            OnDialogueEnd?.Invoke(this);
        }

        /// <summary>
        /// Advance the current dialogue.
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

        //private void SelectDialogChoice(int index)
        //{
        //    var count = story.currentChoices.Count;
        //    if (count == 0)
        //        throw Exceptions.NoOptionsAvailable;
        //    if (index >= count)
        //        throw Exceptions.IndexOutOfRange(index, count);
        //    story.ChooseChoiceIndex(index);
        //    ProcessNextDialogueLine();
        //}

        /// <summary>
        /// Load the values of the dialogue's global variables from a JSON file.
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
        /// Save the values of the dialogue's global variables to a JSON file.
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

        private void JumpToKnot(string knotAddress)
        {
            if (string.IsNullOrEmpty(knotAddress))
                throw Exceptions.KnotIsNullorEmpty;
            if (knotAddress.Contains("."))
            {
                var splitString = knotAddress.Split(".");
                if (splitString.Length > 2)
                    throw Exceptions.InvalidKnotFormat(knotAddress);
                var knot = splitString[0];
                var stitch = splitString[1];
                if (!story.ContainsKnot(knot,stitch))
                    throw Exceptions.KnotPlusStitchDoesNotExist(knot, stitch);
            }
            else if (!story.ContainsKnot(knotAddress))
                throw Exceptions.KnotDoesNotExist(knotAddress);
            story.ChoosePathString(knotAddress);
        }

        private void ProcessNextDialogueLine()
        {
            var text = story.Continue();
            var tags = story.GetDialogueTags();
            var choices = story.GetDialogueChoices();
            var cue = story.GetDialogueCue();
            line = new DialogueLine(text, tags, choices, cue);
            OnNewDialogueLine?.Invoke(this, line);
        }

        private void VariableChanged(string name, Ink.Runtime.Object value)
        {
            OnVariableChanged?.Invoke(this, name, value);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Static Methods

        public static Story CreateNewStory(string text)
        {
            var story = new Story(text);
            story.onError += (msg, type) =>
            {
                if (type == Ink.ErrorType.Warning)
                    Debug.LogWarning(msg);
                else
                    Debug.LogError(msg);
            };
            return story;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Exceptions

        private static class Exceptions
        {
            public static System.InvalidOperationException DialogueAlreadyInProgress
                => new("Cannot perform operation while a dialogue is already in progress.");

            public static System.IndexOutOfRangeException IndexOutOfRange(int index, int count)
                => new($"Choice with index {index} is invalid. Only {count} choices are available.");

            public static System.ArgumentException InvalidKnotFormat(string knot)
                => new($"Knot address '{knot}' is incorrectly formatted.");

            public static System.ArgumentException KnotPlusStitchDoesNotExist(string knot, string stitch)
                => new($"Knot+Stitch address '{knot}.{stitch}' does not exist in the story.");

            public static System.ArgumentException KnotDoesNotExist(string knot)
                => new($"Knot address '{knot}' does not exist in the story.");

            public static System.ArgumentException KnotIsNullorEmpty
                => new("Knot address cannot be null or empty.");

            public static System.InvalidOperationException NoDialogueInProgress
                => new("No dialogue is currently in progress");

            public static System.InvalidOperationException NoOptionsAvailable
                => new("No dialogue options are available.");

            public static System.InvalidOperationException NoSaveLoadDuringDialogue
                => new("The variables of a Story may not be adjusted while a Dialogue is in progress.");

            public static System.InvalidOperationException NotTrackingGlobalVariables
                => new($"Method cannot be called: No {GlobalVariablesName} has been set.");

            public static System.InvalidOperationException StoryCannotContinue
                => new("The story cannot continue further.");

            public static void ThrowIfNull(object argument, string paramName)
            {
                if (argument == null)
                    throw new System.ArgumentNullException(paramName);
            }

            private static string GlobalVariablesName
                => typeof(DialogueVariables).Name;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
