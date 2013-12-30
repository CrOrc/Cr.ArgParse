using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cr.ArgParse.Extensions;

namespace Cr.ArgParse
{
    public class Parser : ActionContainer, IArgumentParser
    {
        public Parser(string description, IList<string> prefixes, string conflictHandlerName)
            : base(description, prefixes, conflictHandlerName)
        {
        }

        public ParseResult ParseArguments(IEnumerable<string> args, ParseResult parseResult = null)
        {
            if (parseResult == null)
                parseResult = new ParseResult();
            //add any action defaults that aren't present
            foreach (var action in Actions)
            {
                //if action.dest is not SUPPRESS:
                if (action.Destination != null)
                {
                    var res = parseResult[action.Destination];
                    if (ReferenceEquals(res, null))
                        //if action.default is not SUPPRESS:
                        if (!ReferenceEquals(action.Argument.DefaultValue, null))
                            parseResult[action.Destination] = action.Argument.DefaultValue;
                }
            }

            //parse the arguments and exit if there are any errors
            parseResult = ParseKnownArgs(args, parseResult);
            return parseResult;
        }

        private ParseResult ParseKnownArgs(IEnumerable<string> args, ParseResult parseResult)
        {
            var argStrings = args.ToList();
            //map all mutually exclusive arguments to the other arguments they can't occur with
            var actionConflicts = new Dictionary<ArgumentAction, List<ArgumentAction>>();
            foreach (var mutexGroup in MutuallyExclusiveGroups)
            {
                var groupActions = mutexGroup.GroupActions;
                foreach (var @tmp in groupActions.Select((it, i) => new {mutexAction = it, i}))
                {
                    var conflicts = actionConflicts.SafeGetValue(@tmp.mutexAction) ??
                                    (actionConflicts[@tmp.mutexAction] = new List<ArgumentAction>());
                    conflicts.AddRange(groupActions.Take(@tmp.i));
                    conflicts.AddRange(groupActions.Skip(@tmp.i + 1));
                }
            }

            // find all option indices, and determine the arg_string_pattern
            // which has an 'O' if there is an option at an index,
            // an 'A' if there is an argument, or a '-' if there is a '--'
            var optionStringIndices = new Dictionary<int, OptionTuple>();
            var argStringPatternBuilder = new StringBuilder();
            var argEnumerator = argStrings.GetEnumerator();
            var argPos = 0;
            for (; argEnumerator.MoveNext(); ++argPos)
            {
                var argString = argEnumerator.Current;
                //all args after -- are non-options
                if (argString == "--")
                {
                    argStringPatternBuilder.Append('-');
                    while (argEnumerator.MoveNext())
                        argStringPatternBuilder.Append('A');
                }
                    // otherwise, add the arg to the arg strings
                    // and note the index if it was an option
                else
                {
                    var optionTuple = ParseOptional(argString);
                    char pattern;
                    if (optionTuple == null)
                        pattern = 'A';
                    else
                    {
                        optionStringIndices[argPos] = optionTuple;
                        pattern = 'O';
                    }
                    argStringPatternBuilder.Append(pattern);
                }
            }

            var argStringPattern = argStringPatternBuilder.ToString();

            var seenActions = new HashSet<ArgumentAction>();
            var seenNonDefaultActions = new HashSet<ArgumentAction>();
            Action<ArgumentAction, IEnumerable<string>, string> takeAction =
                (action, argumentStrings, optionString) =>
                {
                    seenActions.Add(action);
                    var argumentValues = GetValues(action, argumentStrings);
                    // error if this argument is not allowed with other previously
                    // seen arguments, assuming that actions that use the default
                    // value don't really count as "present"
                    if (action.DefaultValue != argumentValues)
                    {
                        seenNonDefaultActions.Add(action);
                        foreach (var actionName in
                            actionConflicts.SafeGetValue(action, new List<ArgumentAction>())
                                .Where(seenNonDefaultActions.Contains)
                                .Select(GetActionName))
                            throw new ArgumentError(action,
                                string.Format("not allowed with argument {0}", actionName));
                    }
                    //if argument_values is not SUPPRESS
                    if (argumentValues != null)
                        action.Call(parseResult, argumentValues, optionString);
                };
            return parseResult;
        }

        private static string GetActionName(ArgumentAction action)
        {
            return action == null
                ? null
                : (action.OptionStrings != null && action.OptionStrings.Any()
                    ? string.Join("/", action.OptionStrings)
                    : (action.MetaVariable ?? action.Destination));
        }

        private object GetValues(ArgumentAction action, IEnumerable<string> argumentStrings)
        {
            return null;
        }

        private OptionTuple ParseOptional(string argString)
        {
            return null;
        }

        private class OptionTuple
        {
            public ArgumentAction Action { get; set; }
            public string OptionString { get; set; }
            public object ExplicitArgument { get; set; }
        }

        public Parser()
            : this("", new[] {"-", "--", "/"}, "resolve")
        {
        }
    }
}