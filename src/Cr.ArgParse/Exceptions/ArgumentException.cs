using System;
using Action = Cr.ArgParse.Actions.Action;

namespace Cr.ArgParse.Exceptions
{
    public class ArgumentException : ParserException
    {
        public ArgumentException(Action action, string message, Exception innerException = null)
            : base(message, innerException)
        {
            Action = action;
        }

        public Action Action { get; private set; }
    }
}