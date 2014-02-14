using System;
using Cr.ArgParse.Actions;
using Action = Cr.ArgParse.Actions.Action;

namespace Cr.ArgParse.Exceptions
{
    public class ArgumentException : ParserException
    {
        public Action Action { get; private set; }

        public ArgumentException(Action action, string message, Exception innerException=null):base(message,innerException)
        {
            Action = action;
        }
    }
}