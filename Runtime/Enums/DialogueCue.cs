namespace StephanHooft.Dialogue
{
    /// <summary>
    /// Enum that is used to communicate to a <see cref="DialogueProcessor"/> which dialogue cue (if any) should be
    /// displayed after a <see cref="DialogueLine"/> has finished processing.
    /// </summary>
    public enum DialogueCue
    {
        /// <summary>
        /// No dialogue cue should be shown.
        /// </summary>
        None,

        /// <summary>
        /// A dialogue cue should be shown to indicate that the story can continue.
        /// </summary>
        CanContinue,

        /// <summary>
        /// A dialogue cue should be shown to indicate that the end of the story has been reached.
        /// </summary>
        EndReached,
    }
}
