using System;
using System.Collections.Generic;
using System.Linq;

namespace Cr.ArgParse.Extensions
{
    internal static class DictionaryExtensions
    {
        public static bool SafeContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null || Equals(key, null)) return false;
            return dictionary.ContainsKey(key);
        }

        public static TValue SafeGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            TValue defaultValue = default (TValue),
            Func<TValue, bool> fallbackToDefaultPredicate = null)
        {
            if (dictionary == null || Equals(key, null)) return defaultValue;
            TValue ret;
            return dictionary.TryGetValue(key, out ret) &&
                   !(fallbackToDefaultPredicate != null && fallbackToDefaultPredicate(ret))
                ? ret
                : defaultValue;
        }

        public static TValue SafeGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> defaultValueFactory,
            Func<TValue, bool> fallbackToDefaultPredicate = null)
        {
            if (dictionary == null || Equals(key, null)) return defaultValueFactory();
            TValue ret;
            return dictionary.TryGetValue(key, out ret) &&
                   !(fallbackToDefaultPredicate != null && fallbackToDefaultPredicate(ret))
                ? ret
                : defaultValueFactory();
        }

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> dictionary,
            Func<KeyValuePair<TKey, TValue>, bool> filterPredicate = null,
            IEqualityComparer<TKey> equalityComparer = null)
        {
            if (dictionary == null) return null;

            var res = new Dictionary<TKey, TValue>(equalityComparer);
            var items = filterPredicate == null ? dictionary : dictionary.Where(filterPredicate);
            foreach (var kv in items)
                res[kv.Key] = kv.Value;
            return res;
        }

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> dictionary,
            IEqualityComparer<TKey> equalityComparer = null)
        {
            return ToDictionary(dictionary, null, equalityComparer);
        }

    }
}