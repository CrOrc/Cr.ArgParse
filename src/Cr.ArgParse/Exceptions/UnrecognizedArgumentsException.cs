using System.Collections.Generic;

namespace Cr.ArgParse.Exceptions
{
    public class UnrecognizedArgumentsException : ParserException
    {
        public UnrecognizedArgumentsException(IList<string> unrecognizedArguments)
        {
            UnrecognizedArguments = unrecognizedArguments;
        }

        public IList<string> UnrecognizedArguments { get; private set; }
    }
}