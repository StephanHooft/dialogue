using Ink.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StephanHooft.Dialogue
{
    /// <summary>
    /// A serializable data object for Ink story variables.
    /// </summary>
    [Serializable]
    public class DialogueVariablesState
    {
        #region Fields

        public List<BoolVariable> boolVariables = new();
        public List<IntVariable> intVariables = new();
        public List<FloatVariable> floatVariables = new();
        public List<StringVariable> stringVariables = new();
        public List<ListVariable> listVariables = new();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Constructors

        /// <summary>
        /// Create a new serialisable <see cref="DialogueVariablesState"/>.
        /// </summary>
        /// <param name="inkVariables">A <see cref="VariablesState"/> to source variables from.</param>
        public DialogueVariablesState(VariablesState inkVariables)
        {
            if (inkVariables == null)
                throw new ArgumentNullException($"inkVariables cannot be null.");
            foreach (string variable in inkVariables)
            {
                Value value = inkVariables.GetVariableWithName(variable) as Value;
                AddValue(variable, value);
            }
        }

        /// <summary>
        /// Create a new serialisable <see cref="DialogueVariablesState"/>.
        /// </summary>
        /// <param name="variables">A Dictionary to source variables from.</param>
        public DialogueVariablesState(Dictionary<string, Value> variables)
        {
            if (variables == null)
                throw new ArgumentNullException($"variables cannot be null.");
            foreach (var variable in variables)
                AddValue(variable.Key, variable.Value);
        }

        /// <summary>
        /// Create a new serialisable <see cref="DialogueVariablesState"/> from a JSON <see cref="string"/>.
        /// </summary>
        /// <param name="json">A JSON <see cref="string"/> to source variables from.</param>
        /// <returns>A new <see cref="DialogueVariablesState"/>.</returns>
        public static DialogueVariablesState FromJSON(string json)
        {
            if (json == null)
                throw new ArgumentNullException($"json string cannot be null.");
            return JsonUtility.FromJson<DialogueVariablesState>(json);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        /// <summary>
        /// Add a new <see cref="Value"/> to the <see cref="DialogueVariablesState"/>.
        /// </summary>
        /// <param name="name">The <see cref="string"/> name for the <see cref="Value"/> to add.</param>
        /// <param name="value">The <see cref="Value"/> to add.</param>
        public void AddValue(string name, Value value)
        {
            if (name == null)
                throw new ArgumentNullException($"name cannot be null.");
            if (value == null)
                throw new ArgumentNullException($"value cannot be null.");
            switch (value.valueType)
            {
                case Ink.Runtime.ValueType.Bool:
                    boolVariables.Add((name, value as BoolValue));
                    break;
                case Ink.Runtime.ValueType.Int:
                    intVariables.Add((name, value as IntValue));
                    break;
                case Ink.Runtime.ValueType.Float:
                    floatVariables.Add((name, value as FloatValue));
                    break;
                case Ink.Runtime.ValueType.List:
                    listVariables.Add((name, value as ListValue));
                    break;
                case Ink.Runtime.ValueType.String:
                    stringVariables.Add((name, value as StringValue));
                    break;
                default:
                    throw new NotImplementedException($"{value.valueType} is not supported.");
            }
        }

        /// <summary>
        /// Export the <see cref="DialogueVariablesState"/> to a Dictionary.
        /// </summary>
        /// <returns>The exported Dictionary.</returns>
        public Dictionary<string, Value> ExportToDictionary()
        {
            var output = new Dictionary<string, Value>();
            foreach (var boolVar in boolVariables)
                output.Add(boolVar.name, boolVar);
            foreach (var intVar in intVariables)
                output.Add(intVar.name, intVar);
            foreach (var floatVar in floatVariables)
                output.Add(floatVar.name, floatVar);
            foreach (var listVar in listVariables)
                output.Add(listVar.name, listVar);
            foreach (var stringVar in stringVariables)
                output.Add(stringVar.name, stringVar);
            return output;
        }

        /// <summary>
        /// Exports the <see cref="DialogueVariablesState"/> to JSON.
        /// </summary>
        /// <returns>A UTF8-encoded <see cref="string"/> containing the JSON export data.</returns>
        public string ToJSON(bool formatted = false)
        {
            return JsonUtility.ToJson(this, formatted);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }

    /// <summary>
    /// A serializable data object representing an <see cref="BoolValue"/> and its name.
    /// </summary>
    [Serializable]
    public class BoolVariable
    {
        /// <summary>
        /// The <see cref="BoolVariable"/>'s <see cref="string"/> name.
        /// </summary>
        public string name;

        /// <summary>
        /// A the <see cref="bool"/> value.
        /// </summary>
        public bool value;

        /// <summary>
        /// Converts from a (<see cref="string"/>, <see cref="BoolValue"/>) tuple 
        /// to a <see cref="BoolVariable"/>.
        /// </summary>
        public static implicit operator BoolVariable((string name, BoolValue value) tuple)
            => new() { name = tuple.name, value = tuple.value.value };

        /// <summary>
        /// Converts from a <see cref="BoolVariable"/> to a <see cref="BoolValue"/>,
        /// losing the <see cref="BoolVariable"/>'s name in the process.
        /// </summary>
        public static implicit operator BoolValue(BoolVariable boolVariable)
            => new(boolVariable.value);
    }

    /// <summary>
    /// A serializable data object representing an <see cref="IntValue"/> and its name.
    /// </summary>
    [Serializable]
    public class IntVariable
    {
        /// <summary>
        /// The <see cref="IntVariable"/>'s <see cref="string"/> name.
        /// </summary>
        public string name;

        /// <summary>
        /// A the <see cref="int"/> value.
        /// </summary>
        public int value;

        /// <summary>
        /// Converts from a (<see cref="string"/>, <see cref="IntValue"/>) tuple 
        /// to a <see cref="IntVariable"/>.
        /// </summary>
        public static implicit operator IntVariable((string name, IntValue value) tuple)
            => new() { name = tuple.name, value = tuple.value.value };

        /// <summary>
        /// Converts from a <see cref="IntVariable"/> to a <see cref="IntValue"/>,
        /// losing the <see cref="IntVariable"/>'s name in the process.
        /// </summary>
        public static implicit operator IntValue(IntVariable intVariable)
            => new(intVariable.value);
    }

    /// <summary>
    /// A serializable data object representing an <see cref="FloatValue"/> and its name.
    /// </summary>
    [Serializable]
    public class FloatVariable
    {
        /// <summary>
        /// The <see cref="FloatVariable"/>'s <see cref="string"/> name.
        /// </summary>
        public string name;

        /// <summary>
        /// A the <see cref="float"/> value.
        /// </summary>
        public float value;

        /// <summary>
        /// Converts from a (<see cref="string"/>, <see cref="FloatValue"/>) tuple 
        /// to a <see cref="FloatVariable"/>.
        /// </summary>
        public static implicit operator FloatVariable((string name, FloatValue value) tuple)
            => new() { name = tuple.name, value = tuple.value.value };

        /// <summary>
        /// Converts from a <see cref="FloatVariable"/> to a <see cref="FloatValue"/>,
        /// losing the <see cref="FloatVariable"/>'s name in the process.
        /// </summary>
        public static implicit operator FloatValue(FloatVariable floatVariable)
            => new(floatVariable.value);
    }

    /// <summary>
    /// A serializable data object representing an <see cref="FloatValue"/> and its name.
    /// </summary>
    [Serializable]
    public class StringVariable
    {
        /// <summary>
        /// The <see cref="StringVariable"/>'s <see cref="string"/> name.
        /// </summary>
        public string name;

        /// <summary>
        /// A the <see cref="string"/> value.
        /// </summary>
        public string value;

        /// <summary>
        /// Converts from a (<see cref="string"/>, <see cref="StringValue"/>) tuple 
        /// to a <see cref="StringVariable"/>.
        /// </summary>
        public static implicit operator StringVariable((string name, StringValue value) tuple)
            => new() { name = tuple.name, value = tuple.value.value };

        /// <summary>
        /// Converts from a <see cref="StringVariable"/> to a <see cref="StringValue"/>,
        /// losing the <see cref="StringVariable"/>'s name in the process.
        /// </summary>
        public static implicit operator StringValue(StringVariable stringVariable)
            => new(stringVariable.value);
    }

    /// <summary>
    /// A serializable data object representing an <see cref="InkList"/> and its name.
    /// </summary>
    [Serializable]
    public class ListVariable
    {
        /// <summary>
        /// The <see cref="ListVariable"/>'s <see cref="string"/> name.
        /// </summary>
        public string name;

        /// <summary>
        /// A the <see cref="InkList"/> value.
        /// </summary>
        public InkList value;

        /// <summary>
        /// Converts from a (<see cref="string"/>, <see cref="ListValue"/>) tuple 
        /// to a <see cref="ListVariable"/>.
        /// </summary>
        public static implicit operator ListVariable((string name, ListValue value) tuple)
            => new() { name = tuple.name, value = tuple.value.value };

        /// <summary>
        /// Converts from a <see cref="ListVariable"/> to a <see cref="ListValue"/>,
        /// losing the <see cref="ListVariable"/>'s name in the process.
        /// </summary>
        public static implicit operator ListValue(ListVariable listVariable)
            => new(listVariable.value);

        /// <summary>
        /// A serialisable version of a <see cref="Ink.Runtime.InkList"/>.
        /// <para>
        /// Contains <see cref="InkListItem"/>s and <see cref="InkListDefinition"/>s for said items' origins.
        /// </para>
        /// </summary>
        [Serializable]
        public class InkList
        {
            /// <summary>
            /// The <see cref="InkList"/>'s <see cref="InkListItem"/>s.
            /// </summary>
            public List<(InkListItem key, int value)> items;

            /// <summary>
            /// The <see cref="InkList"/>'s origin <see cref="InkListDefinition"/>s.
            /// </summary>
            public List<InkListDefinition> origins;

            /// <summary>
            /// Converts an <see cref="Ink.Runtime.InkList"/> into an <see cref="InkList"/>.
            /// </summary>
            public static implicit operator InkList(Ink.Runtime.InkList list)
            {
                var output = new InkList { items = new(), origins = null };
                foreach (var itemAndValue in list)
                    output.items.Add((itemAndValue.Key, itemAndValue.Value));
                if (list.origins != null)
                {
                    output.origins = new();
                    foreach (var origin in list.origins)
                        output.origins.Add(origin);
                }
                return output;
            }

            /// <summary>
            /// Converts an <see cref="InkList"/> into an <see cref="Ink.Runtime.InkList"/>.
            /// </summary>
            /// <param name="list"></param>
            public static implicit operator Ink.Runtime.InkList(InkList list)
            {
                Ink.Runtime.InkList inkList = new();
                if (list.origins != null)
                {
                    inkList.origins = new();
                    foreach (var origin in list.origins)
                        inkList.origins.Add(origin);
                }
                if (list.items != null)
                    foreach (var item in list.items)
                        inkList.AddItem(item.key);
                return inkList;
            }

            /// <summary>
            /// A serialisable version of a <see cref="Ink.Runtime.InkListItem"/>.
            /// </summary>
            [Serializable]
            public struct InkListItem
            {
                /// <summary>
                /// The name of the <see cref="InkListItem"/>'s list origin.
                /// </summary>
                public string originName;

                /// <summary>
                /// The <see cref="InkListItem"/>'s name.
                /// </summary>
                public string itemName;

                /// <summary>
                /// Converts an <see cref="Ink.Runtime.InkListItem"/> into an <see cref="InkListItem"/>.
                /// </summary>
                public static implicit operator InkListItem(Ink.Runtime.InkListItem item)
                    => new() { originName = item.originName, itemName = item.itemName };

                /// <summary>
                /// Converts an <see cref="InkListItem"/> into an <see cref="Ink.Runtime.InkListItem"/>.
                /// </summary>
                public static implicit operator Ink.Runtime.InkListItem(InkListItem item)
                    => new(item.originName, item.itemName);
            }

            /// <summary>
            /// A serialisable version of a <see cref="ListDefinition"/>.
            /// </summary>
            [Serializable]
            public class InkListDefinition
            {
                /// <summary>
                /// The <see cref="InkListDefinition"/>'s name.
                /// </summary>
                public string listName;

                /// <summary>
                /// The <see cref="InkListDefinition"/>'s <see cref="InkListItem"/>s.
                /// </summary>
                public List<(InkListItem listItem, int value)> items;

                /// <summary>
                /// Converts a <see cref="ListDefinition"/> into an <see cref="InkListDefinition"/>.
                /// </summary>
                public static implicit operator InkListDefinition(ListDefinition definition)
                {
                    var items = new List<(InkListItem name, int value)>();
                    foreach (var item in definition.items)
                        items.Add((item.Key, item.Value));
                    return new() { listName = definition.name, items = items };
                }

                /// <summary>
                /// Converts an <see cref="InkListDefinition"/> into a <see cref="ListDefinition"/>.
                /// </summary>
                public static implicit operator ListDefinition(InkListDefinition definition)
                {
                    var items = new Dictionary<string, int>();
                    if (definition.items == null)
                        return new(null, items);
                    var name = definition.items.Count > 0
                        ? definition.items[0].listItem.originName
                        : null;
                    foreach (var item in definition.items)
                        items.Add(item.listItem.itemName, item.value);
                    return new(name, items);
                }
            }
        }
    }
}
