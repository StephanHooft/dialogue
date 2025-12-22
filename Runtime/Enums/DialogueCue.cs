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
        /// Dialogue can continue.
        /// </summary>
        CanContinue,

        /// <summary>
        /// A choice must be made before dialogue can continue.
        /// </summary>
        Choice,

        /// <summary>
        /// The end of a dialogue has been reached.
        /// </summary>
        EndReached,
    }
}
