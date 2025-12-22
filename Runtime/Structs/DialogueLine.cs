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

        /// <summary>
        /// Die <see cref="DialogueLine"/>'s <see cref="DialogueCue"/>.
        /// </summary>
        public readonly DialogueCue cue;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Constructor

        /// <summary>
        /// Create a new <see cref="DialogueLine"/>
        /// </summary>
        /// <param name="text">
        /// The <see cref="DialogueLine"/>'s <see cref="string"/> text.
        /// </param>
        /// <param name="tags">
        /// <see cref="DialogueTag"/>s, if any.
        /// </param>
        /// <param name="choices">
        /// <see cref="DialogueChoice"/>s, if any.
        /// </param>
        /// <param name="cue">
        /// The <see cref="DialogueLine"/>'s <see cref="DialogueCue"/>.
        /// </param>
        public DialogueLine(string text, DialogueTag[] tags, DialogueChoice[] choices, DialogueCue cue)
        {
            this.text = text;
            this.tags = tags;
            this.choices = choices;
            this.cue = cue;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
