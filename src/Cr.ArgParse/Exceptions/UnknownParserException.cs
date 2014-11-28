using Cr.ArgParse.Actions;

namespace Cr.ArgParse.Exceptions
{
    public class UnknownParserException : ArgumentException
    {
        public UnknownParserException(Action action, string parserName)
            : base(action, string.Format("Unknown parser {0}", parserName))
        {
            ParserName = parserName;
        }

        public string ParserName { get; private set; }
    }
}