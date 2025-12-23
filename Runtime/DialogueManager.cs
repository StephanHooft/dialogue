using Ink.Runtime;
using StephanHooft.Dialogue.Data;
using UnityEngine;

namespace StephanHooft.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueManager", menuName = "Dialogue/Dialogue Manager")]
    public class DialogueManager : ScriptableObject
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
        public event System.Action<DialogueManager, DialogueLine> OnDialogueLine;

        /// <summary>
        /// Invoked when a choice is passed to the <see cref="DialogueManager"/>.
        /// </summary>
        public event System.Action<DialogueManager, int> OnChoice;

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
        /// The most recent <see cref="DialogueLine"/> processed by the <see cref="DialogueManager"/>.
        /// </summary>
        public DialogueLine CurrentDialogueLine { get; private set; } = new();

        /// <summary>
        /// True if the <see cref="DialogueManager"/> is currently running a dialogue.
        /// </summary>
        public bool DialogueInProgress { get; private set; }

        /// <summary>
        /// The <see cref="DialogueManager"/>'s current <see cref="string"/> starting knot, if any.
        /// </summary>
        public string StartingKnot
            => dialogueAsset.StartingKnot;

        /// <summary>
        /// The <see cref="DialogueManager"/>'s current <see cref="string"/> starting stitch, if any.
        /// </summary>
        public string StartingStitch
            => dialogueAsset.StartingStitch;

        /// <summary>
        /// The <see cref="DialogueManager"/>'s current <see cref="string"/> text, if any.
        /// </summary>
        public string Text
            => dialogueAsset.Text;

        private bool TrackingVariables
            => dialogueVariablesAsset != null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Fields

        [SerializeField] 
        private DialogueAsset dialogueAsset;

        [SerializeField]
        [Header("Dialogue Variables (Optional)")]
        private DialogueVariables dialogueVariablesAsset;

        [SerializeField] private bool debug;

        private Story story;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region ScriptableObject Implementation

        private void OnEnable()
        {
            if (TrackingVariables)
                dialogueVariablesAsset.Initialise();
        }

        private void OnDisable()
        {
            CurrentDialogueLine = new();
            DialogueInProgress = false;
            story = null;
            if (TrackingVariables)
                dialogueVariablesAsset.UnInitialise();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        /// <summary>
        /// Begin a dialogue.
        /// </summary>
        /// <param name="startingKnot">The <see cref="string"/> name of the knot (if any) at which to begin the dialogue.
        /// Leave this empty to start in the default position.
        /// </param>
        /// <param name="startingStitch">The <see cref="string"/> name of the stitch (if any) at which to begin the dialogue.
        /// </param>
        public void StartDialogue(string startingKnot = null, string startingStitch = null)
        {
            Exceptions.ThrowIfNull(dialogueAsset.Text, "dialogueAsset");
            if (DialogueInProgress)
                StopCurrentStory();
            StartNewStory(startingKnot ?? dialogueAsset.StartingKnot, startingStitch ?? dialogueAsset.StartingStitch);
        }

        /// <summary>
        /// Begin a dialogue.
        /// </summary>
        /// <param name="dialogue">The <see cref="DialogueAsset"/> to base the new dialogue on.</param>
        /// <param name="startingKnot">The <see cref="string"/> name of the knot at which to begin the dialogue.
        /// Leave this empty to start in the position specified by the <see cref="DialogueAsset"/>.
        /// </param>
        /// <param name="startingStitch">The <see cref="string"/> name of the stitch at which to begin the dialogue.
        /// Leave this empty to start in the position specified by the <see cref="DialogueAsset"/>.
        /// </param>
        public void StartDialogue(DialogueAsset dialogue, string startingKnot = null, string startingStitch = null)
        {
            Exceptions.ThrowIfNull(dialogue, "dialogue");
            Exceptions.ThrowIfNull(dialogue.Text, "DialogueAsset.Text");
            if (DialogueInProgress)
                StopCurrentStory();
            dialogueAsset = dialogue;
            StartNewStory(startingKnot ?? dialogueAsset.StartingKnot, startingStitch ?? dialogueAsset.StartingStitch);
        }

        /// <summary>
        /// Stop running the current dialogue.
        /// </summary>
        public void StopDialogue()
        {
            if (!DialogueInProgress)
                return;
            StopCurrentStory();
        }

        /// <summary>
        /// Advance the current dialogue. Pass no parameters if continuing normally. Pass a choice index if a choice is required for the dialogue to continue.
        /// </summary>
        public void AdvanceDialogue()
        {
            if (DialogueInProgress && CanContinueDialogue)
                ProcessNextDialogueLine();
        }

        /// <summary>
        /// Advance the current dialogue. Pass no parameters if continuing normally. Pass a choice index if a choice is required for the dialogue to continue.
        /// </summary>
        /// <param name="choiceIndex">The <see cref="int"/> index of the choice to select.</param>
        public void AdvanceDialogue(int choiceIndex)
        {
            var count = CurrentDialogueLine.choices.Length;
            if (!DialogueInProgress || CurrentDialogueLine.cue != DialogueCue.Choice)
                return;
            if (choiceIndex >= count || choiceIndex < 0)
                throw Exceptions.IndexOutOfRange(choiceIndex, count);
            OnChoice?.Invoke(this, choiceIndex);
            if (debug)
                Debug.Log($"Selected dialogue choice {choiceIndex}: {CurrentDialogueLine.choices[choiceIndex].text}.");
            story.ChooseChoiceIndex(choiceIndex);
            ProcessNextDialogueLine();
        }

        ///// <summary>
        ///// Load the values of the dialogue's global variables from a JSON file.
        ///// </summary>
        ///// <param name="json">
        ///// A chunk of JSON containing the variables to load.
        ///// </param>
        //public void LoadGlobalVariables(string json)
        //{
        //    if (!TrackingVariables)
        //        throw Exceptions.NotTrackingGlobalVariables;
        //    if (DialogueInProgress)
        //        throw Exceptions.NoSaveLoadDuringDialogue;
        //    Exceptions.ThrowIfNull(dialogueAsset, "dialogueAsset");
        //    dialogueVariablesAsset.LoadVariables(json, story);
        //}

        ///// <summary>
        ///// Save the values of the dialogue's global variables to a JSON file.
        ///// </summary>
        ///// <returns>
        ///// A chunk of JSON containing the saved variables.
        ///// </returns>
        //public string SaveGlobalVariables()
        //{
        //    if (DialogueInProgress)
        //        throw Exceptions.NoSaveLoadDuringDialogue;
        //    Exceptions.ThrowIfNull(dialogueAsset, "dialogueAsset");
        //    return TrackingVariables
        //        ? dialogueVariablesAsset.SaveVariables()
        //        : throw Exceptions.NotTrackingGlobalVariables;
        //}

        private void JumpToKnot(string knotAddress, string stitchAddress)
        {
            if (knotAddress == null || knotAddress == "")
                return;
            if (!story.ContainsKnot(knotAddress))
                throw Exceptions.KnotDoesNotExist(knotAddress);
            if(stitchAddress == null || stitchAddress == "")
                story.ChoosePathString(knotAddress);
            else
            {
                if (!story.ContainsKnot(knotAddress, stitchAddress))
                    throw Exceptions.KnotPlusStitchDoesNotExist(knotAddress, stitchAddress);
                story.ChoosePathString($"{knotAddress}.{stitchAddress}");
            }
        }

        private void ProcessNextDialogueLine()
        {
            var text = story.Continue();
            var tags = story.GetDialogueTags();
            var choices = story.GetDialogueChoices();
            var cue = story.GetDialogueCue();
            CurrentDialogueLine = new DialogueLine(text, tags, choices, cue);
            OnDialogueLine?.Invoke(this, CurrentDialogueLine);
            if (debug)
            {
                Debug.Log(CurrentDialogueLine);
                foreach (var choice in choices)
                    Debug.Log(choice);
            }
        }

        private void StartNewStory(string startingKnot, string startingStitch)
        {
            story = CreateNewStory(dialogueAsset.Text);
            JumpToKnot(startingKnot, startingStitch);
            if (TrackingVariables)
            {
                story.variablesState.variableChangedEvent += VariableChanged;
                dialogueVariablesAsset.StartListening(story);
            }
            OnDialogueStart?.Invoke(this);
            if (debug)
                Debug.Log($"Starting dialogue: {dialogueAsset.Name}.");
            DialogueInProgress = true;
            ProcessNextDialogueLine();
        }

        private void StopCurrentStory()
        {
            story.ResetState();
            CurrentDialogueLine = new();
            if (TrackingVariables)
            {
                story.variablesState.variableChangedEvent -= VariableChanged;
                dialogueVariablesAsset.StopListening(story);
            }
            OnDialogueEnd?.Invoke(this);
            if (debug)
                Debug.Log($"Stopping dialogue: {dialogueAsset.Name}.");
            DialogueInProgress = false;
        }

        private void VariableChanged(string name, Ink.Runtime.Object value)
        {
            OnVariableChanged?.Invoke(this, name, value);
            if (debug)
                Debug.Log($"Dialogue variable {name} changed to: {value}.");
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
        #region Singleton Behaviour

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Exceptions

        private static class Exceptions
        {
            public static System.IndexOutOfRangeException IndexOutOfRange(int index, int count)
                => new($"Choice with index {index} is invalid. Only {count} choices are available.");

            public static System.ArgumentException InvalidKnotFormat(string knot)
                => new($"Knot address '{knot}' is incorrectly formatted.");

            public static System.ArgumentException KnotPlusStitchDoesNotExist(string knot, string stitch)
                => new($"Knot+Stitch address '{knot}.{stitch}' does not exist in the story.");

            public static System.ArgumentException KnotDoesNotExist(string knot)
                => new($"Knot address '{knot}' does not exist in the story.");

            public static System.InvalidOperationException NoSaveLoadDuringDialogue
                => new("The variables of a dialogue may not be saved or loaded while it is in progress.");

            public static void ThrowIfNull(object argument, string paramName)
            {
                if (argument == null)
                    throw new System.ArgumentNullException(paramName);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
