namespace StephanHooft.Dialogue
{
    /// <summary>
    /// A <see cref="struct"/> to represent the contents of a parsed dialogue tag.
    /// </summary>
    public readonly struct DialogueTag
    {
        #region Fields

        /// <summary>
        /// The <see cref="string"/> label, or "type" of the <see cref="DialogueTag"/>.
        /// </summary>
        public readonly string label;

        /// <summary>
        /// The value of the <see cref="DialogueTag"/>.
        /// </summary>
        public readonly string value;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Constructor

        /// <summary>
        /// Create a new <see cref="DialogueTag"/>.
        /// </summary>
        /// <param name="label">
        /// A <see cref="string"/> label, or "type".
        /// </param>
        /// <param name="value">
        /// A <see cref="string"/> value.
        /// </param>
        public DialogueTag(string label, string value)
        {
            this.label = label;
            this.value = value;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
