using Cr.ArgParse.Actions;

namespace Cr.ArgParse.Exceptions
{
    public class InvalideChoiceException : ArgumentException
    {
        public InvalideChoiceException(Action action, object value)
            : base(action, string.Format("Invalid choice: {0}", value))
        {
            Value = value;
        }

        public object Value { get; private set; }
    }
}