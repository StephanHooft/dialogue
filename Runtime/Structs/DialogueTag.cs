using System;

namespace StephanHooft.Dialogue
{
    /// <summary>
    /// A struct to encapsulate <see cref="string"/> tags for <see cref="DialogueLine"/> and <see cref="DialogueChoice"/>.
    /// <para>Tags can have scope (delineated with ::) or parameters (deliniated with "=" and separated with ",").
    /// Scope and parameters are parsed when the <see cref="DialogueTag"/> is constructed.</para>
    /// <para>Formatting example: <code>tag=parameter1,parameter2::scope</code></para>
    /// </summary>
    public readonly struct DialogueTag
    {
        /// <summary>
        /// Returns the number of parameters held by the <see cref="DialogueTag"/>.
        /// </summary>
        public int ParameterCount
            => parameters == null ? 0 : parameters.Length;

        /// <summary>
        /// The <see cref="DialogueTag"/>'s label, without scope and/or parameters.
        /// <para>
        /// If not scoped and without parameters, equal to the <see cref="string"/> value used to create the tag.
        /// </para>
        /// </summary>
        public readonly string label;

        private readonly string[] parameters;

        private readonly string scope;

        /// <summary>
        /// Create a new <see cref="DialogueTag"/>.
        /// </summary>
        /// <param name="tag">The <see cref="string"/> on which to base the tag.
        /// <para>Will be parsed for scope or parameters.</para></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tag"/> is null.</exception>
        public DialogueTag(string tag)
        {
            if(tag == null)
                throw new ArgumentNullException(nameof(tag));
            label = tag.Trim();
            if(CheckForScope(ref label, out scope))
            {
                CheckForParameters(ref label, out parameters);
                return;
            }
            CheckForParameters(ref label, out parameters);
        }

        /// <summary>
        /// Retrieves the <see cref="DialogueTag"/>'s <see cref="string"/> parameters.
        /// </summary>
        /// <param name="parameters">The <see cref="DialogueTag"/>'s parameters, if any.</param>
        /// <returns><see cref="true"/> if the <see cref="DialogueTag"/> has parameters.</returns>
        public bool HasParameters(out string[] parameters)
        {
            if(this.parameters != null)
            {
                parameters = this.parameters;
                return true;
            }
            parameters = null;
            return false;
        }

        /// <summary>
        /// Retrieves the <see cref="DialogueTag"/>'s <see cref="string"/> scope.
        /// </summary>
        /// <param name="scope">The <see cref="DialogueTag"/>'s scope, if any.</param>
        /// <returns><see cref="true"/> if the <see cref="DialogueTag"/> has a scope.</returns>
        public bool Scoped(out string scope)
        {
            if (this.scope != null)
            {
                scope = this.scope;
                return true;
            }
            scope = null;
            return false;
        }

        public override readonly string ToString()
            => $"{label}{(parameters == null ? "" : $"={string.Join(',', parameters)}")}{(scope == null ? "" : $"::{scope}")}";

        private static bool CheckForScope(ref string label, out string scope)
        {
            if (label.SplitIfContains("::", out var split) && split.Length == 2)
            {
                label = split[0];
                scope = split[1];
                return true;
            }
            scope = null;
            return false;
        }

        private static bool CheckForParameters(ref string label, out string[] parameters)
        {
            if(!label.Contains("::") && label.SplitIfContains('=', out var split) && split.Length == 2)
            {
                label = split[0];
                if (split[1].SplitIfContains(',', out parameters))
                    return true;
                parameters = new[] { split[1] };
                return true;
            }
            parameters = null;
            return false;
        }
    }
}
