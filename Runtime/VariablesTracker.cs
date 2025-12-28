using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StephanHooft.Dialogue
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that allows a <see cref="DialogueManager"/> to persist variables across
    /// different dialogue sessions.
    /// </summary>
    [CreateAssetMenu(fileName = "VariablesTracker", menuName = "Dialogue/Variables Tracker", order = 30)]
    public class VariablesTracker : ScriptableObject, IEnumerable<KeyValuePair<string, Value>>
    {
        #region Events

        /// <summary>
        /// Invoked when the <see cref="VariablesTracker"/> starts tracking a dialogue.
        /// </summary>
        public event System.Action<VariablesTracker> OnTrackingStart;

        /// <summary>
        /// Invoked when the <see cref="DialogueManager"/> stops tracking a dialogue.
        /// </summary>
        public event System.Action<VariablesTracker> OnTrackingEnd;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Properties

        /// <summary>
        /// <see cref="true"/> if the <see cref="VariablesTracker"/> is able to track variables.
        /// </summary>
        public bool Initialised
            => variables != null;

        /// <summary>
        /// Gets or sets the variable associated with the specified <see cref="string"/> name.
        /// </summary>
        /// <param name="variableName">The <see cref="string"/> name of the variable to get or set.</param>
        /// <returns>The <see cref="Value"/> associated with the specified name.
        /// If the specified variable name is not found, a KeyNotFoundException will be thrown.</returns>
        public Value this[string variableName]
        {
            get
            {
                if (!variables.ContainsKey(variableName))
                    throw new KeyNotFoundException($"No dialogue variable with name {variableName} was found.");
                return variables[variableName];
            }
            set
            {
                if (!Initialised)
                    return;
                if (Tracking)
                    throw new System.InvalidOperationException($"Cannot set values while tracking a dialogue.");
                if (value == null)
                    throw new System.ArgumentNullException($"Dialogue variables may not be set to null.");
                if (!variables.ContainsKey(variableName))
                    throw new KeyNotFoundException($"No dialogue variable with name {variableName} was found.");
                if (value.valueType != variables[variableName].valueType)
                    throw new System.ArgumentException($"Cannot set a variable of type {value.valueType} to " +
                        $"{variables[variableName].valueType} {variableName}.");
                variables[variableName] = value;
                if (!debugMode.None())
                    Debug.Log($"{name} | {value.valueType} variable [{variableName}] changed to: [{value}]\n");
            }
        }

        /// <summary>
        /// <see cref="true"/> if the <see cref="VariablesTracker"/> is currently tracking a dialogue.
        /// </summary>
        public bool Tracking
            => trackedManager != null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Fields

        [SerializeField]
        [Tooltip("A TextAsset that contains an Ink story compiled to JSON.\n" +
            "Make sure this asset includes all variables you want to track.")]
        private TextAsset variablesAsset;

        [SerializeField]
        [Tooltip("Enable to make the VariablesTracker also store variables internally whenever they are saved.\n" +
            "When disabled, the VariableTracker's internal variable storage is cleared.")]
        private bool storeSavedDataInTracker;

        [SerializeField]
        [Tooltip("Limits the number of debug log messages printed.")]
        private DebugMode debugMode = DebugMode.None;
        [SerializeField] private string saveData = null;

        private Dictionary<string, Value> variables = null;
        private DialogueManager trackedManager;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Base Classes Implementation

        private void OnEnable()
        {
            trackedManager = null;
            variables = null;
            if(variablesAsset != null)
                InitialiseVariables(variablesAsset);
        }

        IEnumerator<KeyValuePair<string, Value>> IEnumerable<KeyValuePair<string, Value>>.GetEnumerator()
        {
            return variables.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return variables.GetEnumerator();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        /// <summary>
        /// Load a variables state from a JSON <see cref="string"/>, or from the <see cref="VariablesTracker"/>'s
        /// internal storage.
        /// </summary>
        /// <param name="json">A <see cref="string"/> to read from. Leave <see cref="null"/> to load the
        /// <see cref="<see cref="VariablesTracker"/>'s internal storage, if any."/></param>
        public void LoadVariables(string json = null)
        {
            if (!Initialised)
                throw new System.InvalidOperationException();
            if (Tracking)
                trackedManager.StopDialogue();
            var loadSource = json ?? saveData;
            if (loadSource != null && loadSource.Length > 0)
            {
                var state = DialogueVariablesState.FromJSON(json ?? saveData);
                variables = state.ExportToDictionary();
                if (debugMode.Full())
                    Debug.Log($"{name} | Loaded variables.\n{json}");
            }
            else if (debugMode.Full())
                Debug.Log($"{name} | No data to load.");
        }

        /// <summary>
        /// Reset tracked variables to their default values.
        /// </summary>
        public void ResetVariables()
        {
            if (!Initialised)
                return;
            if (Tracking)
                trackedManager.StopDialogue();
            if (variablesAsset != null)
                InitialiseVariables(variablesAsset);
            if (debugMode.Full())
                Debug.Log($"{name} | Variables reset.\n");
        }

        /// <summary>
        /// Save the current variables state to a JSON <see cref="string"/>.
        /// </summary>
        public string SaveVariables()
        {
            if (!Initialised)
                throw new System.InvalidOperationException();
            if (Tracking)
                trackedManager.StopDialogue();
            var state = new DialogueVariablesState(variables);
            var data = state.ToJSON();
            if (storeSavedDataInTracker)
            {
                saveData = data;
                if (debugMode.Full())
                    Debug.Log($"{name} | Saved variables.\n{saveData}");
            }
            return data;
        }

        /// <summary>
        /// Change the <see cref="TextAsset"/> that specifies which variables the <see cref="VariablesTracker"/>
        /// keeps track of.
        /// </summary>
        /// <param name="asset">A <see cref="TextAsset"/> with JSON that compiles to a valid Ink story.</param>
        /// <returns><see cref="true"/> if the <see cref="VariablesTracker"/> was able to initialise based on the 
        /// provided <see cref="TextAsset"/>.</returns>
        public bool SetAsset(TextAsset asset)
        {
            if (Tracking)
                trackedManager.StopDialogue();
            if (InitialiseVariables(asset))
                variablesAsset = asset;
            return Initialised;
        }

        /// <summary>
        /// Start tracking variables for the specified <see cref="DialogueManager"/>.
        /// </summary>
        /// <param name="manager">The <see cref="DialogueManager"/> to track.</param>
        /// <returns><see cref="true"/> if the <see cref="VariablesTracker"/> was able to start tracking.</returns>
        public bool StartTracking(DialogueManager manager)
        {
            if (Tracking)
                trackedManager.StopDialogue();
            if (!Initialised|| manager == null)
                return false;
            manager.OnVariableChanged += VariableChanged;
            trackedManager = manager;
            OnTrackingStart?.Invoke(this);
            if (debugMode.Full())
                Debug.Log($"{name} | Started tracking {manager.name}.\n");
            return true;
        }

        /// <summary>
        /// Stop tracking variables.
        /// </summary>
        public void StopTracking()
        {
            if(trackedManager != null)
            {
                trackedManager.OnVariableChanged -= VariableChanged;
                OnTrackingEnd?.Invoke(this);
                if (debugMode.Full())
                    Debug.Log($"{name} | Stopped tracking {trackedManager.name}.\n");
            }
            trackedManager = null;
        }

        /// <summary>
        /// Determines whether the <see cref="VariablesTracker"/> contains the specified variable.
        /// </summary>
        /// <param name="variableName">The <see cref="string"/> name of the variable to locate.</param>
        /// <returns><see cref="true"/> if the <see cref="VariablesTracker"/> tracks a variable with the specified name;
        /// otherwise, <see cref="false"/>.</returns>
        public bool TracksVariable(string variableName)
        {
            if (!Initialised)
                return false;
            return variables.ContainsKey(variableName);
        }

        /// <summary>
        /// Gets the <see cref="Value"/> associated with the specified <see cref="string"/> variable name.
        /// </summary>
        /// <param name="variableName">The <see cref="string"/> name of the variable to get.</param>
        /// <param name="value">When this method returns, contains the <see cref="Value"/> associated with the specified
        /// name, if the name is found; otherwise, <see cref="null"/>.</param>
        /// <returns><see cref="true"/> if the <see cref="VariablesTracker"/> tracks a variable with the specified
        /// name; otherwise, <see cref="false"/>.</returns>
        public bool TryGetValue(string variableName, out Value value)
        {
            value = null;
            if (variables.ContainsKey(variableName))
            {
                value = variables[variableName];
                return true;
            }
            return false;
        }

        private void VariableChanged(DialogueManager manager, string name, Value value)
        {
            if (variables.ContainsKey(name))
            {
                if(variables[name].valueType != value.valueType)
                    throw new System.ArgumentException($"Variable {value} of type {value.valueType} " +
                        $"does not match type {variables[name].valueType}.");
                variables[name] = value;
                if (!debugMode.None())
                    Debug.Log($"{this.name} | {value.valueType} variable [{name}] changed to: [{value}].\n");
            }
            else if(!debugMode.None())
                Debug.LogWarning($"{name} | Observed change to unknown variable {name}.");
        }

        private bool InitialiseVariables(TextAsset asset)
        {
            if (asset.IsValidInkStory(out var story))
            {
                var globalVariables = story.variablesState;
                if (variables == null)
                    variables = new();
                else
                    variables.Clear();
                foreach (string variable in globalVariables)
                {
                    Value value = globalVariables.GetVariableWithName(variable) as Value;
                    variables.Add(variable, value);
                }
                return true;
            }
            else
                return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
