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
        #region Fields

        [SerializeField]
        private DialogueManager dialogueManager;

        [SerializeField]
        private string startingKnot;

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
        /// Activate the <see cref="DialogueTrigger"/>.
        /// </summary>
        public void Trigger()
        {
            if (!dialogueManager.DialogueInProgress)
                dialogueManager.StartDialogue(startingKnot);
            else
                Debug.LogError("Cannot trigger dialogue. DialogueManager is already progressing a story");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Exceptions

        private static class Exceptions
        {
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
