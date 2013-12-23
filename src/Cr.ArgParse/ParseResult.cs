using System;
using System.Collections.Generic;
using Cr.ArgParse.Extensions;

namespace Cr.ArgParse
{
    public class ParseResult
    {
        private readonly IDictionary<string,object> results = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
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

        public void SaveArgument(string optionString, object values)
        {
            results[optionString] = values;
        }
    }
}