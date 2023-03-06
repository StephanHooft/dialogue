using UnityEngine;

namespace StephanHooft.Dialogue.Data
{
    /// <summary>
    /// A simple <see cref="DialogueAsset"/> that directly references a <see cref="TextAsset"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "New DialogueAsset", menuName = "Dialogue/Dialogue Asset (Single)", order = 1)]
    public class SingleDialogueAsset : DialogueAsset
    {
        #region Properties

        public override string Text
            => asset != null
            ? asset.text
            : throw Exceptions.AssetMissing(name);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Fields

        [SerializeField] private TextAsset asset;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Exceptions

        private static class Exceptions
        {
            public static MissingReferenceException AssetMissing(string name)
                => new($"{TypeName} {name} is missing is a {typeof(TextAsset).Name}.");
            
            private static string TypeName => typeof(DialogueAsset).Name;

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
