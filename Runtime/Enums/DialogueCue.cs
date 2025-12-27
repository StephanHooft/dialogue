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

    public static class DialogueCueExtensions
    {
        /// <summary>
        /// Returns <see cref="true"/> if the <see cref="DialogueCue"/> has value <see cref="DialogueCue.CanContinue"/>.
        /// </summary>
        public static bool CanContinue(this DialogueCue dialogueCue)
            => dialogueCue == DialogueCue.CanContinue;

        /// <summary>
        /// Returns <see cref="true"/> if the <see cref="DialogueCue"/> has value <see cref="DialogueCue.Choice"/>.
        /// </summary>
        public static bool Choice(this DialogueCue dialogueCue)
            => dialogueCue == DialogueCue.Choice;

        /// <summary>
        /// Returns <see cref="true"/> if the <see cref="DialogueCue"/> has value <see cref="DialogueCue.EndReached"/>.
        /// </summary>
        public static bool EndReached(this DialogueCue dialogueCue)
            => dialogueCue == DialogueCue.EndReached;

        /// <summary>
        /// Returns <see cref="true"/> if the <see cref="DialogueCue"/> has value <see cref="DialogueCue.None"/>.
        /// </summary>
        public static bool None(this DialogueCue dialogueCue)
            => dialogueCue == DialogueCue.None;
    }
}
