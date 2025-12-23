using System;
using UnityEngine;

namespace StephanHooft.Dialogue
{
    /// <summary>
    /// A struct to encapsulate a (reference to) a <see cref="TextAsset"/> and a <see cref="string"/> starting node.
    /// Used to source Ink-based dialogues by a <see cref="DialogueManager"/>.
    /// </summary>
    [Serializable]
    public struct DialogueAsset
    {
        #region Properties

        /// <summary>
        /// The name of the <see cref="TextAsset"/> encapsulated by the <see cref="DialogueAsset"/>.
        /// </summary>
        public string Name
            => asset != null ? asset.name : "";

        /// <summary>
        /// A <see cref="string"/> address of a knot to jump to when starting a dialogue.
        /// </summary>
        public string StartingKnot 
            => startingKnot;

        /// <summary>
        /// A <see cref="string"/> of a stitch (within the starting knot) to jump to when starting a dialogue.
        /// </summary>
        public string StartingStitch
            => startingStitch;

        /// <summary>
        /// The contents of the <see cref="TextAsset"/> encapsulated by the <see cref="DialogueAsset"/>.
        /// </summary>
        public string Text
        {
            get => asset != null ? asset.text : null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Fields

        [SerializeField] private TextAsset asset;
        [SerializeField] private string startingKnot;
        [SerializeField] private string startingStitch;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Constructor

        /// <summary>
        /// Create a new <see cref="DialogueAsset"/>.
        /// </summary>
        /// <param name="asset">A <see cref="TextAsset"/>.</param>
        public DialogueAsset(TextAsset asset)
        {
            this.asset = asset;
            startingKnot = null;
            startingStitch = null;
        }

        /// <summary>
        /// Create a new <see cref="DialogueAsset"/>.
        /// </summary>
        /// <param name="asset">A <see cref="TextAsset"/>.</param>
        /// <param name="startingKnot">Address of a knot in the text asset to start from.</param>
        public DialogueAsset(TextAsset asset, string startingKnot)
        {
            this.asset = asset;
            this.startingKnot = startingKnot;
            startingStitch = null;
        }

        /// <summary>
        /// Create a new <see cref="DialogueAsset"/>.
        /// </summary>
        /// <param name="asset">A <see cref="TextAsset"/>.</param>
        /// <param name="startingKnot">Address of a knot in the text asset to start from.</param>
        /// <param name="startingStitch">Address of a stitch in the text asset to start from.</param>
        public DialogueAsset(TextAsset asset, string startingKnot, string startingStitch)
        {
            this.asset = asset;
            this.startingKnot = startingKnot;
            this.startingStitch = startingStitch;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
