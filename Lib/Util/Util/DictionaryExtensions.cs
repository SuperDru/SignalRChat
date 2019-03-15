using System.Collections.Generic;

namespace Qoden.Util
{
    /// <summary>
    /// Common extensions to <see cref="IDictionary&lt;TKey,TValue&gt;"/>
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the value for a key. If the key does not exist, return default(TValue);
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dict">The dictionary to call this method on.</param>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">Default value to return in case there is no key in the dictionary</param>
        /// <returns>The key value. default(TValue) if this key is not in the dictionary.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
                                                             TValue defaultValue = default(TValue))
        {
            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
