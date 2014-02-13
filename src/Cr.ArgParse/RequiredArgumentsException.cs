using System.Collections.Generic;
using System.Linq;
using Cr.ArgParse.Actions;
using Cr.ArgParse.Extensions;

namespace Cr.ArgParse
{
    public class RequiredArgumentsException : ParserException
    {
        private readonly string message;

        public RequiredArgumentsException(IEnumerable<ArgumentAction> requiredActions)
        {
            RequiredActions = (requiredActions ?? new ArgumentAction[] {}).ToList();
            RequiredArguments = RequiredActions.Select(GetArgumentName).ToList();
            message = string.Format("The following arguments are required: {0}", string.Join(", ", RequiredArguments));
        }

        public override string Message { get { return message; } }

        public IList<ArgumentAction> RequiredActions { get; private set; }
        public IList<string> RequiredArguments { get; private set; }

        private static string GetArgumentName(ArgumentAction argumentAction)
        {
            if (argumentAction == null) return null;
            if (argumentAction.OptionStrings.IsTrue())
                return string.Format("({0})", string.Join("/", argumentAction.OptionStrings));
            if (!string.IsNullOrWhiteSpace(argumentAction.MetaVariable))
                return argumentAction.MetaVariable;
            if (argumentAction.HasDestination && argumentAction.HasValidDestination)
                return argumentAction.Destination;
            return null;
        }
    }
}