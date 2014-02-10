using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cr.ArgParse.Extensions;

namespace Cr.ArgParse
{
    public class Parser : ActionContainer, IArgumentParser
    {
        public Parser(string description, IList<string> prefixes, string conflictHandlerName)
            : base(description, prefixes, conflictHandlerName)
        {
        }

        public Parser()
            : this("", new[] {"-", "--", "/"}, "resolve")
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
                        if (!ReferenceEquals(action.DefaultValue, null) || action.Argument.Type == null)
                            parseResult[action.Destination] = action.DefaultValue;
                }
            }

            //parse the arguments and exit if there are any errors
            parseResult = ParseKnowArguments(args, parseResult);
            if (parseResult.UnrecognizedArguments != null && parseResult.UnrecognizedArguments.Any())
            {
                throw new UnrecognizedArgumentsException(parseResult.UnrecognizedArguments);
            }
            return parseResult;
        }

        public ParseResult ParseKnowArguments(IEnumerable<string> args, ParseResult parseResult)
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
            var extras = new List<string>();
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
            Func<int, int> consumeOptional =
                startIndex =>
                {
                    var optionTuple = optionStringIndices.SafeGetValue(startIndex);
                    var action = optionTuple.Action;
                    var optionString = optionTuple.OptionString;
                    var explicitArg = optionTuple.ExplicitArgument;
                    var stopIndex = startIndex;

                    // identify additional optionals in the same arg string
                    // (e.g. -xyz is the same as -x -y -z if no args are required)
                    var actionTuples = new List<ActionTuple>();
                    while (true)
                    {
                        //if we found no optional action, skip it
                        if (action == null)
                        {
                            extras.Add(argStrings[startIndex]);
                            return startIndex + 1;
                        }
                        if (explicitArg != null)
                        {
                            var argCount = MatchArgument(action, "A");
                            //if the action is a single-dash option and takes no
                            // arguments, try to parse more single-dash options out
                            // of the tail of the option string
                            if (argCount == 0 && optionString.StartsWith(ShortPrefixes) &&
                                !optionString.StartsWith(LongPrefixes))
                            {
                                actionTuples.Add(new ActionTuple
                                {
                                    Action = action,
                                    Arguments = new string[] {},
                                    OptionString = optionString
                                });
                                var prefix = optionString.Substring(0, 1);
                                optionString = prefix + explicitArg[0];
                                var newExplicitArg = explicitArg.Length > 1 ? explicitArg.Substring(1) : null;
                                var newAction = OptionStringActions.SafeGetValue(optionString);
                                if (newAction != null)
                                {
                                    action = newAction;
                                    explicitArg = newExplicitArg;
                                }
                                else
                                {
                                    throw new ArgumentError(action,
                                        string.Format("Ignored explicit argument {0}", explicitArg));
                                }
                            }
                                // if the action expect exactly one argument, we've
                                // successfully matched the option; exit the loop
                            else if (argCount == 1)
                            {
                                stopIndex = startIndex + 1;
                                actionTuples.Add(new ActionTuple
                                {
                                    Action = action,
                                    Arguments = new[] {explicitArg},
                                    OptionString = optionString
                                });
                                break;
                            }
                                // error if a double-dash option did not use the explicit argument
                            else
                            {
                                throw new ArgumentError(action,
                                    string.Format("Ignored explicit argument {0}", explicitArg));
                            }
                        }
                            // if there is no explicit argument, try to match the
                            // optional's string arguments with the following strings
                            // if successful, exit the loop
                        else
                        {
                            var start = startIndex + 1;
                            var selectedPatterns = argStringPattern.Substring(start);
                            var argCount = MatchArgument(action, selectedPatterns);
                            stopIndex = start + argCount;
                            actionTuples.Add(new ActionTuple
                            {
                                Action = action,
                                Arguments = argStrings.Skip(start).Take(argCount).ToList(),
                                OptionString = optionString
                            });
                            break;
                        }
                    }
                    // add the Optional to the list and return the index at which
                    // the Optional's string args stopped

                    if (!actionTuples.Any())
                        throw new ParserException("Should be at least one action");
                    foreach (var actionTuple in actionTuples)
                        takeAction(actionTuple.Action, actionTuple.Arguments, actionTuple.OptionString);
                    return stopIndex;
                };

            var positionals = GetPositionalActions();

            Func<int, int> consumePositionals =
                patternStartIndex =>
                {
                    var selectedPattern = argStringPattern.Substring(patternStartIndex);
                    var argCounts = MatchArgumentsPartial(positionals, selectedPattern);

                    //slice off the appropriate arg strings for each Positional
                    // and add the Positional and its args to the list
                    foreach (var it in positionals.Zip(argCounts, (action, argCount) => new {action, argCount}))
                    {
                        var actionArgs = argStrings.Skip(patternStartIndex).Take(it.argCount).ToList();
                        patternStartIndex += it.argCount;
                        takeAction(it.action, actionArgs, null);
                    }
                    positionals.RemoveRange(0, argCounts.Count);
                    return patternStartIndex;
                };
            extras = new List<string>();
            var globalStartIndex = 0;
            var maxOptionStringIndex = optionStringIndices.Any() ? optionStringIndices.Keys.Max() : -1;
            while (globalStartIndex <= maxOptionStringIndex)
            {
                var nextOptionStringIndex = optionStringIndices.Keys.Where(it => it >= globalStartIndex).Min();
                if (globalStartIndex != nextOptionStringIndex)
                {
                    var positionalEndIndex = consumePositionals(globalStartIndex);
                    // only try to parse the next optional if we didn't consume
                    // the option string during the positionals parsing
                    globalStartIndex = positionalEndIndex;
                    if (positionalEndIndex > nextOptionStringIndex)
                        continue;
                }

                // if we consumed all the positionals we could and we're not
                // at the index of an option string, there were extra arguments
                if (!optionStringIndices.ContainsKey(globalStartIndex))
                {
                    extras.AddRange(argStrings.Skip(globalStartIndex).Take(nextOptionStringIndex - globalStartIndex));
                    globalStartIndex = nextOptionStringIndex;
                }

                // consume the next optional and any arguments for it
                globalStartIndex = consumeOptional(globalStartIndex);
            }

            // consume any positionals following the last Optional
            var globalStopIndex = consumePositionals(globalStartIndex);

            // if we didn't consume all the argument strings, there were extras
            extras.AddRange(argStrings.Skip(globalStopIndex));

            if (extras.Any())
                parseResult.UnrecognizedArguments = extras;
            return parseResult;
        }

        private class ActionTuple
        {
            public ArgumentAction Action { get; set; }
            public IList<string> Arguments { get; set; }
            public string OptionString { get; set; }
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
            object value = null;
            var argStrings = (argumentStrings ?? new string[] {}).ToList();
            if (argumentStrings != null)
                if (action.IsParser || action.IsRemainder)
                {
                    argStrings.Remove("--");
                    argumentStrings = argStrings;
                }
            //optional argument produces a default when not present
            if ((argumentStrings == null || !argumentStrings.Any()) && action.IsOptional)
            {
                value = action.OptionStrings.Any() ? action.ConstValue : action.DefaultValue;
                var strValue = value as string;
                if (strValue != null)
                {
                    value = GetValue(action, strValue);
                    CheckValue(action, value);
                }
            }
                //when ValueCount='0,n' on a positional, if there were no command-line args, use the default if it is anything other than null
            else if ((argumentStrings == null || !argumentStrings.Any()) && !action.OptionStrings.Any() &&
                     action.ValueCount != null &&
                     action.ValueCount.Min == 0 && action.ValueCount.Max > 0)
            {
                value = action.DefaultValue ?? argumentStrings;
                CheckValue(action, value);
            }
                // single argument or optional argument produces a single value
            else if (argStrings.Count == 1 && (action.ValueCount == null || action.IsOptional))
            {
                value = GetValue(action, argStrings[0]);
                CheckValue(action, value);
            }
                // REMAINDER arguments convert all values, checking none
            else if (action.IsRemainder)
                value = argStrings.Select(it => GetValue(action, it)).ToList();
            else if (action.IsParser)
            {
                var convertedValues = argStrings.Select(it => GetValue(action, it)).ToList();
                value = convertedValues;
                CheckValue(action, value);
            }
                // all other types of ValueCount produce a list
            else
            {
                var convertedValues = argStrings.Select(it => GetValue(action, it)).ToList();
                value = convertedValues;
                foreach (var convertedValue in convertedValues)
                    CheckValue(action, convertedValue);
            }

            // return the converted value
            return value;
        }

        private Func<string, object> GetTypeFactory(string typeName)
        {
            if (StringComparer.InvariantCultureIgnoreCase.Equals(typeName, "int"))
                return argString => int.Parse(argString);
            return argString => argString;
        }

        private object GetValue(ArgumentAction action, string argString)
        {
            var typeFactory = GetTypeFactory(action.Type);
            try
            {
                return typeFactory(argString);
            }
            catch (Exception err)
            {
                //TODO: Add err as inner exception
                throw new ArgumentError(action,
                    string.Format("Invalid type \"{0}\" for value \"{1}\"", action.Type, argString));
            }
        }

        private void CheckValue(ArgumentAction action, object value)
        {
            //TODO: implement validation parsing result
        }

        private int MatchArgument(ArgumentAction action, string argStringsPattern)
        {
            try
            {
                var valueCountPattern = "^" + GetValueCountPattern(action);
                var match = Regex.Match(argStringsPattern, valueCountPattern);
                if (!match.Success)
                    throw new ArgumentError(action,
                        string.Format("Expected {0} argument(s)", action.ValueCount ?? new ValueCount(1)));
                return match.Groups[1].Value.Length;
            }
            catch (ParserException)
            {
                throw;
            }
            catch (Exception err)
            {
                throw new ParserException("Match argument error", err);
            }
        }

        private IList<int> MatchArgumentsPartial(IList<ArgumentAction> actions, string argStringsPattern)
        {
            var res = new List<int>();
            for (var i = actions.Count; i > 0; --i)
            {
                var actionsSlice = actions.Take(i);
                var pattern = "^" + string.Concat(actionsSlice.Select(GetValueCountPattern));
                var match = Regex.Match(argStringsPattern, pattern);
                if (!match.Success) continue;
                res.AddRange(match.Groups.OfType<Group>().Select(it => it.Length));
                break;
            }
            return res;
        }

        private static string GetValueCountPattern(ArgumentAction action)
        {
            var res = action.IsRemainder
                ? "([-AO]*)"
                : (action.IsParser
                    ? "(-*A[-AO]*)"
                    : string.Format("(-*(?:A-*){0})", action.ValueCount ?? new ValueCount(1)));
            if (action.OptionStrings.Any())
                res = res.Replace("-*", "").Replace("-", "");
            return res;
        }

        private OptionTuple ParseOptional(string argString)
        {
            // if it's an empty string, it was meant to be a positional
            if (string.IsNullOrEmpty(argString)) return null;
            // if it doesn't start with a prefix, it was meant to be positional
            if (!argString.StartsWith(Prefixes))
                return null;
            // if the option string is present in the parser, return the action
            ArgumentAction action;
            if (OptionStringActions.TryGetValue(argString, out action))
                return new OptionTuple(action, argString);
            // if it's just a single character, it was meant to be positional
            if (argString.Length == 1)
                return null;
            // if the option string before the "=" is present, return the action
            if (argString.Contains('='))
            {
                var parts = argString.Split(new[] {'='}, 2);
                var optionString = parts[0];
                var explicitArg = parts.Skip(1).FirstOrDefault();
                if (OptionStringActions.TryGetValue(optionString, out action))
                    return new OptionTuple(action, optionString, explicitArg);
            }

            // search through all possible prefixes of the option string
            // and all actions in the parser for possible interpretations
            var optionTuples = GetOptionTuples(argString);
            // if multiple actions match, the option string was ambiguous
            if (optionTuples.Count > 1)
            {
                var options = string.Join(", ", optionTuples.Select(it => it.OptionString));
                throw new ParserException(string.Format("Ambiguous option: {0} could match {1}", argString, options));
            }
                // if exactly one action matched, this segmentation is good,
                // so return the parsed action
            else if (optionTuples.Count == 1)
                return optionTuples[0];

            // if it was not found as an option, but it looks like a negative
            // number, it was meant to be positional
            // unless there are negative-number-like options
            if (NegativeNumberMatcher.IsMatch(argString))
                if (!HasNegativeNumberOptionals.IsTrue())
                    return null;

            // if it contains a space, it was meant to be a positional
            if (argString.Contains(' ')) return null;

            // it was meant to be an optional but there is no such option
            // in this parser (though it might be a valid option in a subparser)
            return new OptionTuple(null, argString);
        }

        private IList<OptionTuple> GetOptionTuples(string optionString)
        {
            var ret = new List<OptionTuple>();
            if (string.IsNullOrEmpty(optionString) || optionString.Length < 2) return ret;
            string optionPrefix;
            string explicitArg;
            // option strings starting with two prefix characters are only
            // split at the '='
            if (optionString.StartsWith(LongPrefixes))
            {
                if (optionString.Contains('='))
                {
                    var parts = optionString.Split(new[] {'='}, 2);
                    optionPrefix = parts[0];
                    explicitArg = parts.Skip(1).FirstOrDefault();
                }
                else
                {
                    optionPrefix = optionString;
                    explicitArg = null;
                }
                ret.AddRange(
                    OptionStringActions.Keys.Where(key => key.StartsWith(optionPrefix))
                        .Select(optionKey => new OptionTuple(OptionStringActions[optionKey], optionKey, explicitArg)));
            }
                // single character options can be concatenated with their arguments
                // but multiple character options always have to have their argument
                // separate
            else if (optionString.StartsWith(ShortPrefixes) && !optionString.StartsWith(LongPrefixes))
            {
                optionPrefix = optionString;
                var shortOptionPrefix = optionString.Substring(0, 2);
                var shortExplicitArg = optionString.Substring(2);
                foreach (var optionKey in OptionStringActions.Keys)
                {
                    if (optionKey == shortOptionPrefix)
                        ret.Add(new OptionTuple(OptionStringActions[optionKey], optionKey, shortExplicitArg));
                    else if (optionKey.StartsWith(optionPrefix))
                        ret.Add(new OptionTuple(OptionStringActions[optionKey], optionKey));
                }
            }
                // shouldn't ever get here
            else
                throw new ParserException(string.Format("Unexpected option string{0}", optionString));
            return ret;
        }

        private class OptionTuple
        {
            public OptionTuple(ArgumentAction action, string optionString, string explicitArgument = null)
            {
                Action = action;
                OptionString = optionString;
                ExplicitArgument = explicitArgument;
            }

            public ArgumentAction Action { get; private set; }
            public string ExplicitArgument { get; private set; }
            public string OptionString { get; private set; }
        }
    }
}