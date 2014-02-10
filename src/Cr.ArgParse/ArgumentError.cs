using System;

namespace Cr.ArgParse
{
    public class ArgumentError : ParserException
    {
        public ArgumentAction Action { get; private set; }

        public ArgumentError(ArgumentAction action, string message, Exception innerException=null):base(message,innerException)
        {
            Action = action;
        }
    }
}