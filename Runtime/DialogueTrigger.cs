using UnityEngine;

namespace StephanHooft.Dialogue
{
    /// <summary>
    /// A <see cref="MonoBehaviour"/> that can be used to kickstart a <see cref="DialogueManager"/> at a particular
    /// knot. One ink story (broken up in to multiple knots) can thus be used to drive content for multiple
    /// <see cref="DialogueTrigger"/>s in one scene.
    /// </summary>
    public sealed class DialogueTrigger : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// True if the <see cref="DialogueTrigger"/>'s associated story can be advanced.
        /// </summary>
        public bool DialogueCanContinue
            => dialogueManager != null && dialogueManager.CanContinueDialogue;

        /// <summary>
        /// True if the <see cref="DialogueTrigger"/>'s associated story is in progress, with the
        /// <see cref="DialogueTrigger"/> as the leading cause.
        /// </summary>
        public bool DialogueInProgress
            => token != null;

        /// <summary>
        /// True if the <see cref="DialogueTrigger"/>'s associated story is waiting on a choice.
        /// </summary>
        public bool DialogueChoicesAvailable
            => dialogueManager != null & dialogueManager.DialogueChoicesAvailable;

        /// <summary>
        /// True if the <see cref="DialogueTrigger"/>'s assigned <see cref="DialogueManager"/> is waiting for
        /// a <see cref="DialogueLine"/> to finish processing.
        /// </summary>
        public bool WaitingForDialogueLineToProcess
            => dialogueManager != null && dialogueManager.WaitingForDialogueLineToProcess;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Fields

        [SerializeField]
        private DialogueManager dialogueManager;

        [SerializeField]
        private string startingKnot;

        private DialogueManager.Token token;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region MonoBehaviour Implementation

        private void Awake()
        {
            Exceptions.ThrowIfNull(dialogueManager, "dialogueManager");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        /// <summary>
        /// Advance the <see cref="DialogueTrigger"/>'s story.
        /// </summary>
        public void Advance()
        {
            dialogueManager.Advance(token);
        }

        /// <summary>
        /// Begin the <see cref="DialogueTrigger"/>'s story.
        /// </summary>
        public void Begin()
        {
            if (DialogueInProgress)
                throw Exceptions.AlreadyInProgress;
            token = dialogueManager.Begin(startingKnot);
        }

        /// <summary>
        /// End the <see cref="DialogueTrigger"/>'s story.
        /// </summary>
        public void End()
        {
            if (!DialogueInProgress)
                throw Exceptions.NotInProgress;
            dialogueManager.End(token);
            token = null;
        }

        /// <summary>
        /// If waiting on a <see cref="DialogueLine"/> to be processed, this method can be used to "rush" that line.
        /// </summary>
        public void RushCurrentDialogueLine()
        {
            dialogueManager.RushCurrentDialogueLine(token);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Exceptions

        private static class Exceptions
        {
            public static System.InvalidOperationException AlreadyInProgress
                => new($"{DialogueTrigger} already has a dialogue in progress.");

            public static System.InvalidOperationException NotInProgress
                => new($"{DialogueTrigger} does not have dialogue in progress.");

            public static void ThrowIfNull(object argument, string paramName)
            {
                if (argument == null)
                    throw new System.ArgumentNullException(paramName);
            }

            private static string DialogueTrigger
                => typeof(DialogueTrigger).Name;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
