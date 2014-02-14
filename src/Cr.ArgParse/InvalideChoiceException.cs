using System;
using Cr.ArgParse.Actions;
using Action = Cr.ArgParse.Actions.Action;
using ArgumentException = Cr.ArgParse.Exceptions.ArgumentException;

namespace Cr.ArgParse
{
    public class InvalideChoiceException : ArgumentException
    {
        public object Value { get; private set; }

        public InvalideChoiceException(Action action, object value)
            : base(action, string.Format("Invalid choice: {0}", value))
        {
            Value = value;
        }
    }
}