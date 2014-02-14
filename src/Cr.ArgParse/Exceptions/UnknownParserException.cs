using Cr.ArgParse.Actions;

namespace Cr.ArgParse.Exceptions
{
    public class UnknownParserException : ArgumentException
    {
        public string ParserName { get; private set; }

        public UnknownParserException(ArgumentAction action, string parserName)
            : base(action, string.Format("Unknown parser {0}", parserName))
        {
            ParserName = parserName;
        }
    }
}