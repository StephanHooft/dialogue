using UnityEngine;

namespace StephanHooft.Dialogue.Data
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> to provide a <see cref="DialogueManager"/> with the ink JSON file it needs to
    /// function.
    /// </summary>
    public abstract class DialogueAsset : ScriptableObject
    {
        #region Properties

        /// <summary>
        /// JSON text to run an ink story with. Must be overridden by classes inheriting from
        /// <see cref="DialogueAsset"/>, and the source of the <see cref="string"/> text can therefore differ.
        /// </summary>
        public virtual string Text { get; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
