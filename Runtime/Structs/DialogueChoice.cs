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
        /// The <see cref="int"/> index of the <see cref="DialogueChoice"/>.
        /// </summary>
        public readonly int index;

        /// <summary>
        /// The <see cref="string"/> text with which to represent the <see cref="DialogueChoice"/>.
        /// </summary>
        public readonly string text;

        /// <summary>
        /// The "choice tags" associated with the <see cref="DialogueChoice"/>, if any.
        /// </summary>
        public readonly string[] tags;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Constructor

        /// <summary>
        /// Create a new <see cref="DialogueChoice"/>.
        /// </summary>
        /// <param name="index">
        /// The <see cref="int"/> index of the <see cref="DialogueChoice"/>.
        /// </param>
        /// <param name="text">
        /// The <see cref="string"/> text with which to represent the <see cref="DialogueChoice"/>.
        /// </param>
        /// <param name="tags">
        /// The <see cref="string"/> tags associated with the <see cref="DialogueChoice"/>, if any.
        /// </param>
        public DialogueChoice(int index, string text, string[] tags)
        {
            this.index = index;
            this.text = text;
            this.tags = tags;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods
        public override string ToString()
        {
            return $"Dialogue choice {index}: {text}\n{(tags.Length > 0 ? $"|| Tags: {string.Join(", ", tags)}" : "")}";
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
