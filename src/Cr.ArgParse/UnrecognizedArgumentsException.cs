using System;
using System.Collections.Generic;
using Cr.ArgParse.Actions;

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

    public class UnknownParserError:ArgumentError
    {
        public string ParserName { get; private set; }

        public UnknownParserError(ArgumentAction action, string parserName) : base(action, string.Format("Unknown parser {0}",parserName))
        {
            ParserName = parserName;
        }
    }
}