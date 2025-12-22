using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;

namespace StephanHooft.Dialogue.Data
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that helps store, save, and load the state of a dialogue's global variables.
    /// <para>
    /// The methods in this class do not need to be called directly. A <see cref="DialogueManager"/> relying on the
    /// <see cref="DialogueVariables"/> will do this automatically.
    /// </para>
    /// </summary>
    [CreateAssetMenu(fileName = "New DialogueVariables", menuName = "Dialogue/Dialogue Variables", order = 30)]
    public sealed class DialogueVariables : ScriptableObject
    {
        #region Properties

        private bool Initialised
            => variables != null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Fields

        [SerializeField] private TextAsset asset;

        private bool Listening
            => activeStory != null;

        private Story activeStory;
        private Story globalVariablesStory;
        private Dictionary<string, Ink.Runtime.Object> variables;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        /// <summary>
        /// This method must be called before others can be used, to set up the <see cref="DialogueVariables"/>'s
        /// internal infrastructure.
        /// </summary>
        public void Initialise()
        {
            if (!Initialised)
            {
                globalVariablesStory = new(asset.text);
                variables = new();
                PopulateVariables(globalVariablesStory);
            }
        }

        /// <summary>
        /// Loads the <see cref="DialogueVariables"/>'s state from a chunk of JSON.
        /// </summary>
        /// <param name="stateJson">
        /// A <see cref="string"/> containing the <see cref="DialogueVariables"/>'s intended JSON representation.
        /// </param>
        /// <param name="storyToLoadTo">
        /// The <see cref="Story"/> (expected to be run by the <see cref="DialogueManager"/> to load the variables to,
        /// in addition to loading them into the <see cref="DialogueVariables"/>.
        /// </param>
        public void LoadVariables(string stateJson, Story storyToLoadTo = null)
        {
            if (!Initialised)
                throw Exceptions.NotInitialised;
            if (Listening)
                throw Exceptions.NoSaveLoadWhileListening;
            globalVariablesStory.state.LoadJson(stateJson);
            PopulateVariables(globalVariablesStory);
            if(storyToLoadTo != null)
                VariablesToStory(storyToLoadTo);
        }

        /// <summary>
        /// Saves the <see cref="DialogueVariables"/>'s state to a chunk of JSON.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> containing the <see cref="DialogueVariables"/>'s JSON representation.
        /// </returns>
        public string SaveVariables()
        {
            if (!Initialised)
                throw Exceptions.NotInitialised;
            if (Listening)
                throw Exceptions.NoSaveLoadWhileListening;
            VariablesToStory(globalVariablesStory);
            return globalVariablesStory.state.ToJson();
        }

        /// <summary>
        /// Instructs the <see cref="DialogueVariables"/> to begin mirroring (changes to) an ink <see cref="Story"/>.
        /// </summary>
        /// <param name="story">
        /// 
        /// </param>
        public void StartListening(Story story)
        {
            if (!Initialised)
                throw Exceptions.NotInitialised;
            if (Listening)
                throw Exceptions.AlreadyListening;
            Exceptions.ThrowIfNull(story, "story");
            VariablesToStory(story);
            story.variablesState.variableChangedEvent += VariableChanged;
            activeStory = story;
        }

        /// <summary>
        /// Instructs the <see cref="DialogueVariables"/> to stop mirroring (changes to) an ink <see cref="Story"/>.
        /// </summary>
        /// <param name="story">
        /// The ink <see cref="Story"/> to stop listening to.
        /// </param>
        public void StopListening(Story story)
        {
            if (!Initialised)
                throw Exceptions.NotInitialised;
            if (!Listening)
                throw Exceptions.NotListening;
            Exceptions.ThrowIfNull(story, "story");
            if (activeStory == story)
            {
                story.variablesState.variableChangedEvent -= VariableChanged;
                activeStory = null;
            }
            else
                throw Exceptions.ListeningToDifferentStory;
        }

        /// <summary>
        /// Cleanup method, to be called just before the entities using the <see cref="DialogueVariables"/> are taken
        /// down.
        /// </summary>
        public void UnInitialise()
        {
            if (Initialised)
            {
                if (Listening)
                    StopListening(activeStory);
                variables = null;
                globalVariablesStory = null;
            }
        }

        private void PopulateVariables(Story story)
        {
            var state = story.variablesState;
            variables.Clear();
            foreach (string name in state)
            {
                var value = state.GetVariableWithName(name);
                variables.Add(name, value);
            }
        }

        private void VariableChanged(string name, Ink.Runtime.Object value)
        {
            if (variables.ContainsKey(name))
            {
                variables.Remove(name);
                variables.Add(name, value);
            }
        }

        private void VariablesToStory(Story story)
        {
            foreach(var variable in variables)
                story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Exceptions

        private static class Exceptions
        {
            public static System.InvalidOperationException AlreadyListening
                => new($"The {TypeName} is already listening to a story.");

            public static System.InvalidOperationException NoSaveLoadWhileListening
                => new($"{TypeName} cannot save or load while listening to a story.");

            public static System.ArgumentException ListeningToDifferentStory
                => new($"The {TypeName} is listening to a different story.");

            public static System.ArgumentException KeyInvalid(string variableName)
                => new($"Variable '{variableName}' is not present in {TypeName}");

            public static System.InvalidOperationException NotInitialised
                => new($"{TypeName} must be initialised before other methods can be called on it.");

            public static System.InvalidOperationException NotListening
                => new($"The {TypeName} is not listening to a story.");

            public static void ThrowIfNull(object argument, string paramName)
            {
                if (argument == null)
                    throw new System.ArgumentNullException(paramName);
            }

            public static string TypeName
                => typeof(DialogueVariables).Name;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
