using Ink.Runtime;
using UnityEngine;

namespace StephanHooft.Dialogue
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that can play back Ink-based stories.
    /// </summary>
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
        public event System.Action<DialogueManager, string, Value> OnVariableChanged;

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Fields

        [SerializeField] private DialogueAsset dialogueAsset;
        [SerializeField] private VariablesTracker variablesAsset;
        [SerializeField] private DebugMode debugMode = DebugMode.None;

        private Story story;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region ScriptableObject Implementation

        private void OnDisable()
        {
            if (DialogueInProgress)
                StopDialogue();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        /// <summary>
        /// Begin a dialogue.
        /// </summary>
        /// <param name="startingKnot">The <see cref="string"/>
        /// name of the knot (if any) at which to begin the dialogue.
        /// Leave this empty to start in the default position.
        /// </param>
        /// <param name="startingStitch">The <see cref="string"/>
        /// name of the stitch (if any) at which to begin the dialogue.
        /// </param>
        public void StartDialogue(string startingKnot = null, string startingStitch = null)
        {
            if(dialogueAsset.Text == null)
                throw new System.ArgumentNullException("dialogueAsset.Text");
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
            if(dialogue.Text == null)
                throw new System.ArgumentNullException("DialogueAsset.Text");
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
        /// Advance the current dialogue. Pass no parameters if continuing normally. 
        /// Pass a choice index if a choice is required for the dialogue to continue.
        /// </summary>
        public void AdvanceDialogue()
        {
            if (DialogueInProgress && CanContinueDialogue)
                ProcessNextDialogueLine();
        }

        /// <summary>
        /// Advance the current dialogue. Pass no parameters if continuing normally.
        /// Pass a choice index if a choice is required for the dialogue to continue.
        /// </summary>
        /// <param name="choiceIndex">The <see cref="int"/> index of the choice to select.</param>
        public void AdvanceDialogue(int choiceIndex)
        {
            var count = CurrentDialogueLine.choices.Length;
            if (!DialogueInProgress || CurrentDialogueLine.cue != DialogueCue.Choice)
                return;
            if (choiceIndex >= count || choiceIndex < 0)
                throw new System.IndexOutOfRangeException
                    ($"Choice with index {choiceIndex} is invalid. Only {count} choices are available.");
            OnChoice?.Invoke(this, choiceIndex);
            if (debugMode.Full())
                Debug.Log($"{this.name} || Selected dialogue choice {choiceIndex}:" +
                    $" {CurrentDialogueLine.choices[choiceIndex].text}.");
            story.ChooseChoiceIndex(choiceIndex);
            ProcessNextDialogueLine();
        }

        /// <summary>
        /// Gets the <see cref="Value"/> of a dialogue variable for a dialogue in progress.
        /// <para>This method will throw a <see cref="System.InvalidOperationException"/> if called while the
        /// <see cref="DialogueManager"/> is not currently processing a dialogue.</para>
        /// </summary>
        /// <param name="variableName">The <see cref="string"/> name of the variable to get.</param>
        /// <returns>The <see cref="Value"/> of the specified variable.</returns>
        public Value GetVariable(string variableName)
        {
            if (DialogueInProgress)
            {
                if (story.variablesState.GlobalVariableExistsWithName(variableName))
                    return story.variablesState.GetVariableWithName(variableName) as Value;
                else throw new System.Collections.Generic.KeyNotFoundException
                        ($"No dialogue variable with name {variableName} was found.");
            }
            throw new System.InvalidOperationException
                ($"Cannot get dialogue variables when no dialogue is in progress.");
        }

        private void JumpToKnot(string knot, string stitch)
        {
            if (knot == null || knot == "")
                return;
            if (!story.ContainsKnot(knot))
                throw new System.ArgumentException
                    ($"Knot address '{knot}' does not exist in the story.");
            if (stitch == null || stitch == "")
                story.ChoosePathString(knot);
            else
            {
                if (!story.ContainsKnot(knot, stitch))
                    throw new System.ArgumentException
                        ($"Knot + Stitch address '{knot}.{stitch}' does not exist in the story.");
                story.ChoosePathString($"{knot}.{stitch}");
            }
        }

        private void ProcessNextDialogueLine()
        {
            var text = story.Continue();
            var tags = story.GetLineTags();
            var choices = story.GetDialogueChoices();
            var cue = story.GetDialogueCue();
            CurrentDialogueLine = new DialogueLine(text, tags, choices, cue);
            OnDialogueLine?.Invoke(this, CurrentDialogueLine);
            if (!debugMode.None())
            {
                Debug.Log($"{name} | {CurrentDialogueLine}");
                foreach (var choice in choices)
                    Debug.Log($"{name} | {choice}");
            }
        }

        private void StartNewStory(string startingKnot, string startingStitch)
        {
            story = CreateNewStory(dialogueAsset.Text);
            JumpToKnot(startingKnot, startingStitch);
            StartTrackingVariables(variablesAsset, this, story, debugMode.Full());
            story.variablesState.variableChangedEvent += VariableChanged;
            OnDialogueStart?.Invoke(this);
            var globalTags = story.GetGlobalTags();
            if (debugMode.Full())
                Debug.Log($"{name} | Starting dialogue: {dialogueAsset.Name}\n" +
                    $"{(globalTags.Length > 0 ? $"Global Tags ({globalTags.Length}): [{string.Join("; ", globalTags)}]" : "")}");
            DialogueInProgress = true;
            ProcessNextDialogueLine();
        }

        private void StopCurrentStory()
        {
            story.variablesState.variableChangedEvent -= VariableChanged;
            if (variablesAsset != null)
                variablesAsset.StopTracking();
            story = null;
            CurrentDialogueLine = new();
            OnDialogueEnd?.Invoke(this);
            if (debugMode.Full())
                Debug.Log($"{name} | Stopping dialogue: {dialogueAsset.Name}\n");
            DialogueInProgress = false;
        }

        private void VariableChanged(string name, Ink.Runtime.Object value)
        {
            if(value is Value)
                OnVariableChanged?.Invoke(this, name, value as Value);
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

        private static void StartTrackingVariables
            (VariablesTracker variablesAsset, DialogueManager manager, Story story, bool debug = false)
        {
            if (variablesAsset == null)
                return;
            if (variablesAsset.StartTracking(manager))
                foreach (System.Collections.Generic.KeyValuePair<string, Value> kvp in variablesAsset)
                {
                    var currentValue = story.variablesState.GetVariableWithName(kvp.Key) as Value;
                    if (kvp.Value.valueType == currentValue.valueType)
                        story.variablesState.SetGlobal(kvp.Key, kvp.Value);
                }
            else if (debug)
                Debug.LogWarning($"{manager.name} | {variablesAsset.name} did not start tracking variables.");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
