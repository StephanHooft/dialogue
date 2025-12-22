using System;

namespace StephanHooft.Dialogue
{
    /// <summary>
    /// A <see cref="struct"/> that contains all relevant information about an available dialogue choice.
    /// </summary>
    public readonly struct DialogueChoice
    {
        #region Fields

        /// <summary>
        /// The <see cref="int"/> index of the dialogue choice.
        /// </summary>
        public readonly int index;

        /// <summary>
        /// The <see cref="string"/> text with which to represent the dialogue choice.
        /// </summary>
        public readonly string text;

        /// <summary>
        /// The "choice tags" associated with the dialogue choice, if any.
        /// </summary>
        public readonly DialogueTag[] tags;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Constructor

        /// <summary>
        /// Create a new <see cref="DialogueChoice"/>.
        /// </summary>
        /// <param name="index">
        /// The <see cref="int"/> index of the dialogue choice.
        /// </param>
        /// <param name="text">
        /// The <see cref="string"/> text with which to represent the dialogue choice.
        /// </param>
        /// <param name="tags">
        /// The "choice tags" associated with the dialogue choice, if any.
        /// </param>
        public DialogueChoice(int index, string text, DialogueTag[] tags)
        {
            this.index = index;
            this.text = text;
            this.tags = tags;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
