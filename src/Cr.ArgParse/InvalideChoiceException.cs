using System;
using Cr.ArgParse.Actions;
using ArgumentException = Cr.ArgParse.Exceptions.ArgumentException;

namespace Cr.ArgParse
{
    public class InvalideChoiceException : ArgumentException
    {
        public object Value { get; private set; }

        public InvalideChoiceException(ArgumentAction action, object value)
            : base(action, string.Format("Invalid choice: {0}", value))
        {
            Value = value;
        }
    }
}