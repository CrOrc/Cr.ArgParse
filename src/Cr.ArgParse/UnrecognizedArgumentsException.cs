using System.Collections.Generic;

namespace Cr.ArgParse
{
    public class UnrecognizedArgumentsException : ParserException
    {
        public IList<string> UnrecognizedArguments { get; private set; }

        public UnrecognizedArgumentsException(IList<string> unrecognizedArguments)
        {
            UnrecognizedArguments = unrecognizedArguments;
        }
    }
}