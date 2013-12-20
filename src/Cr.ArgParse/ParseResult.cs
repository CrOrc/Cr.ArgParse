using System.Collections.Generic;

namespace Cr.ArgParse
{
    public class ParseResult : IParseResult
    {
        private IDictionary<string,object> results = new Dictionary<string, object>();
        public T GetArgument<T>(string argName)
        {
            return default(T);
        }

        public void SaveArgument(string optionString, object values)
        {
            results[optionString] = values;
        }
    }
}