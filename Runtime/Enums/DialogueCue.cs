namespace StephanHooft.Dialogue
{
    /// <summary>
    /// Enum that is used to communicate the progress state of a dialogue after a <see cref="DialogueLine"/> has
    /// finished processing.
    /// </summary>
    public enum DialogueCue
    {
        /// <summary>
        /// No dialogue cue is available.
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
