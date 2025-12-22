using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StephanHooft.Dialogue.Data
{
    /// <summary>
    /// A serializable dictionary (with <see cref="string"/>s as keys) to store <typeparamref name="T"/>s matching
    /// a <see cref="string"/> tag's values.
    /// </summary>
    [System.Serializable]
    public sealed class TagMapperDictionary<T> : ISerializationCallbackReceiver
    {
        #region Properties

        /// <summary>
        /// Returns true if the <see cref="TagMapperDictionary{T}"/> contains a particular <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The <see cref="string"/> key to check for.
        /// </param>
        /// <returns>
        /// True if the <paramref name="key"/> is contained by the <see cref="TagMapperDictionary{T}"/>.
        /// </returns>
        public bool ContainsKey(string key)
            => dictionary.ContainsKey(key);

        /// <summary>
        /// The number of items in the <see cref="TagMapperDictionary{T}"/>.
        /// </summary>
        public int Count
            => dictionary.Count;

        /// <summary>
        /// Retrieves a <typeparamref name="T"/> from the <see cref="TagMapperDictionary{T}"/> by
        /// <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The <see cref="string"/> key of the <typeparamref name="T"/> to retrieve.
        /// </param>
        /// <returns>
        /// A <typeparamref name="T"/>.
        /// </returns>
        public T this[string key]
        { get 
            {
                TagEntry tagEntry;
                try { tagEntry = dictionary[key]; }
                catch (KeyNotFoundException e)
                {
                    throw Exceptions.KeyNotFound(key, e);
                }
                if (tagEntry.HasInvalidKey())
                    throw Exceptions.InvalidKey(key);
                return tagEntry.Value;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Fields

        [SerializeField]
        private List<SerializableKeyValuePair> entries = new();

        private readonly Dictionary<string, TagEntry> dictionary = new();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region ISerializationCallbackReceiver Implementation

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            dictionary.Clear();
            foreach (var entry in entries)
                SafelyAddToDictionary(entry);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            entries.Clear();
            foreach (var kvp in dictionary)
                SafelyAddToList(kvp);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Methods

        private void SafelyAddToDictionary(SerializableKeyValuePair kvp)
        {
            var key = kvp.Key;
            bool keyIsInvalid = string.IsNullOrEmpty(key) || dictionary.ContainsKey(key);
            (string key, TagEntry tagEntry) safeSet = keyIsInvalid
                ? (GeneratePlaceholderKey(), new(kvp.Value, key))
                : (key, new(kvp.Value));
            dictionary.Add(safeSet.key, safeSet.tagEntry);
        }

        private void SafelyAddToList(KeyValuePair<string, TagEntry> kvp)
        {
            var tagEntry = kvp.Value;
            var serializableKeyValuePair = tagEntry.HasInvalidKey(out var invalidKey)
                    ? new SerializableKeyValuePair(invalidKey, tagEntry)
                    : new SerializableKeyValuePair(kvp.Key, tagEntry);
            entries.Add(serializableKeyValuePair);
        }

        private string GeneratePlaceholderKey()
        {
            string placeholderKey;
            do
                placeholderKey = KeyGenerator.GenerateKey(8, 12);
            while (dictionary.ContainsKey(placeholderKey));
            return placeholderKey;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region TagEntry Class

        private class TagEntry
        {
            public T Value
                => value;

            private readonly T value;

            private readonly string invalidKey;

            public TagEntry(T value, string invalidKey = null)
            {
                this.value = value;
                this.invalidKey = invalidKey;
            }

            public bool HasInvalidKey()
                => invalidKey != null;

            public bool HasInvalidKey(out string key)
            {
                key = invalidKey;
                return invalidKey != null;
            }

            public static implicit operator T(TagEntry d)
                => d.value;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region SerializableKeyValuePair Class

        [System.Serializable]
        private class SerializableKeyValuePair
        {
            [field: SerializeField]
            public string Key { get; private set; }

            [field: SerializeField]
            public T Value { get; private set; }

            public SerializableKeyValuePair(string key, T value)
            {
                Key = key;
                Value = value;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region KeyGenerator Class

        private static class KeyGenerator
        {
            private static readonly System.Lazy<StringBuilder> stringBuilder = new(() => new());
            private static readonly System.Lazy<System.Random> randomNumberGenerator = new(() => new());

            private const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";

            public static string GenerateKey(int minLength, int maxLength)
            {
                var builder = stringBuilder.Value;
                var rng = randomNumberGenerator.Value;
                int stringLength = rng.Next(minLength, maxLength);
                for (int i = 0; i < stringLength; i++)
                    builder.Append(glyphs[rng.Next(0, glyphs.Length)]);
                var output = builder.ToString();
                builder.Clear();
                return output;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        #region Exceptions

        private static class Exceptions
        {
            public static System.ArgumentException InvalidKey(string key)
                => new($"The key '{key}' exists in the {TypeName}, but is invalid. " +
                    $"Ensure that keys aren't empty, null, or duplicated.");
            public static KeyNotFoundException KeyNotFound(string key, System.Exception e)
                => new($"No entry with key '{key}' was found in the {TypeName}.", e);

            private static string TypeName
                => typeof(TagMapperDictionary<T>).Name;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
