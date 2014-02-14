using System;
using Cr.ArgParse.Actions;

namespace Cr.ArgParse.Exceptions
{
    public class ArgumentException : ParserException
    {
        public ArgumentAction Action { get; private set; }

        public ArgumentException(ArgumentAction action, string message, Exception innerException=null):base(message,innerException)
        {
            Action = action;
        }
    }
}