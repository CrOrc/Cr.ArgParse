using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cr.ArgParse.Extensions;

namespace Cr.ArgParse
{
    public class ParseResult : IEnumerable
    {
        private IEqualityComparer<string> EqualityComparer { get; set; }
        private readonly IDictionary<string, object> results;

        public ParseResult(IEqualityComparer<string> equalityComparer = null)
        {
            EqualityComparer = equalityComparer ?? StringComparer.InvariantCultureIgnoreCase;
            results = new Dictionary<string, object>(EqualityComparer);
        }

        public T GetArgument<T>(string argName, T defaultValue = default (T))
        {
            try
            {
                var res = results.SafeGetValue(argName);
                if (res is T)
                    return (T) res;
            }
            catch
            {
            }
            return defaultValue;
        }

        public object this[string s]
        {
            get { return results.SafeGetValue(s); }
            set
            {
                if (s == null)
                    throw new ArgumentNullException("s");
                results[s] = value;
            }
        }

        public void Add(string key, object value)
        {
            this[key] = value;
        }

        public void Clear()
        {
            results.Clear();
        }

        public IDictionary<string, object> ToDictionary()
        {
            return results.ToDictionary(kv => kv.Key,
                kv => kv.Value is ParseResult ? (kv.Value as ParseResult).ToDictionary() : kv.Value, EqualityComparer);
        }

        public IEnumerator GetEnumerator()
        {
            return results.GetEnumerator();
        }
    }
}