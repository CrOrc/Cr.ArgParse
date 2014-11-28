using System.Collections.Generic;
using System.Linq;
using Cr.ArgParse.Actions;
using Cr.ArgParse.Extensions;

namespace Cr.ArgParse.Exceptions
{
    public class RequiredArgumentsException : ParserException
    {
        private readonly string message;

        public RequiredArgumentsException(IEnumerable<Action> requiredActions)
        {
            RequiredActions = (requiredActions ?? new Action[] {}).ToList();
            RequiredArguments = RequiredActions.Select(GetArgumentName).ToList();
            message = string.Format("The following arguments are required: {0}", string.Join(", ", RequiredArguments));
        }

        public override string Message
        {
            get { return message; }
        }

        public IList<Action> RequiredActions { get; private set; }
        public IList<string> RequiredArguments { get; private set; }

        private static string GetArgumentName(Action action)
        {
            if (action == null) return null;
            if (action.OptionStrings.IsTrue())
                return string.Format("({0})", string.Join("/", action.OptionStrings));
            if (!string.IsNullOrWhiteSpace(action.MetaVariable))
                return action.MetaVariable;
            if (action.HasDestination && action.HasValidDestination)
                return action.Destination;
            return null;
        }
    }
}