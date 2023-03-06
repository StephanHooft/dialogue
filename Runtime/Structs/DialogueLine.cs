namespace StephanHooft.Dialogue
{
    /// <summary>
    /// A <see cref="struct"/> to represent the contents of a dialogue line, as well as its potential tags and
    /// associated choices.
    /// </summary>
    public readonly struct DialogueLine
    {
        #region Fields

        /// <summary>
        /// The <see cref="DialogueLine"/>'s <see cref="string"/> text.
        /// </summary>
        public readonly string text;

        /// <summary>
        /// The <see cref="DialogueLine"/>'s <see cref="DialogueTag"/>s, if any.
        /// </summary>
        public readonly DialogueTag[] tags;

        /// <summary>
        /// The <see cref="DialogueLine"/>'s <see cref="DialogueChoice"/>s, if any.
        /// </summary>
        public readonly DialogueChoice[] choices;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Constructor

        /// <summary>
        /// Create a new <see cref="DialogueLine"/>
        /// </summary>
        /// <param name="text">
        /// The line's <see cref="string"/> text.
        /// </param>
        /// <param name="tags">
        /// <see cref="DialogueTag"/>s, if any.
        /// </param>
        /// <param name="choices">
        /// <see cref="DialogueChoice"/>s, if any.
        /// </param>
        public DialogueLine(string text, DialogueTag[] tags, DialogueChoice[] choices)
        {
            this.text = text;
            this.tags = tags;
            this.choices = choices;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
