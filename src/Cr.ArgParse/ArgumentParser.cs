using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cr.ArgParse.Extensions;

namespace Cr.ArgParse
{
    public class ArgumentParser : IArgumentParser
    {
        public IParseResult ParseArguments(IEnumerable<string> args)
        {
            return new ParseResult();
        }

        public void AddArgument(Argument argument)
        {
            var localPrefixes = Prefixes;
            Argument preparedArgument;
            if (argument.OptionStrings.IsNullOrEmpty() ||
                argument.OptionStrings.Count == 1 && !argument.OptionStrings[0].StartsWith(localPrefixes))
                preparedArgument = PreparePositionalArgument(argument);
            else
                preparedArgument = PrepareOptionalArgument(argument);
            var argumentAction = CreateArgumentAction(preparedArgument);
            if (ReferenceEquals(argumentAction, null))
                throw new Exception("Unregistered action exception");
            AddArgumentAction(argumentAction);
        }

        private void AddArgumentAction(IArgumentAction argumentAction)
        {
            if(ReferenceEquals(argumentAction,null))
                throw new ArgumentNullException("argumentAction");
            Actions.Add(argumentAction);
            var optionStrings = argumentAction.Argument.OptionStrings ?? new string[] {};
            foreach (var optionString in optionStrings)
                OptionStringActions[optionString] = argumentAction;
        }

        private readonly IList<IArgumentAction> actions = new List<IArgumentAction>();

        private IList<IArgumentAction> Actions{get { return actions; }}


        private readonly IDictionary<string, Func<Argument, IArgumentAction>> actionFactories =
            new Dictionary<string, Func<Argument, IArgumentAction>>(StringComparer.InvariantCultureIgnoreCase);

        private IList<string> prefixes;

        private IDictionary<string, Func<Argument, IArgumentAction>> ActionFactories
        {
            get { return actionFactories; }
        }

        private readonly IDictionary<string, IArgumentAction> optionStringActions =
            new Dictionary<string, IArgumentAction>(StringComparer.InvariantCultureIgnoreCase);

        private IDictionary<string, IArgumentAction> OptionStringActions{get { return optionStringActions; }}

        private IArgumentAction CreateArgumentAction(Argument argument)
        {
            var argumentAction = argument.Action;
            if (argumentAction != null)
                return argumentAction;
            var argumentActionFactory = ActionFactories.SafeGetValue(argument.ActionName) ??
                                        ActionFactories.SafeGetValue(DefaultAction);
            return argumentActionFactory != null ? argumentActionFactory(argument) : null;
        }

        private void RegisterArgumentActions(
            IEnumerable<KeyValuePair<string, Func<Argument, IArgumentAction>>> newActionFactories)
        {
            foreach (var kv in newActionFactories)
                ActionFactories[kv.Key] = kv.Value;
        }

        private Argument PreparePositionalArgument(Argument argument)
        {
            if (ReferenceEquals(argument.ValueCount, null))
                argument.ValueCount = new ValueCount();
            // mark positional arguments as required if at least one is always required
            var valueCount = argument.ValueCount;
            if (valueCount.Min.HasValue && valueCount.Min > 0)
                argument.IsRequired = true;
            else
            {
                if (valueCount.Max == 1)
                    argument.IsRequired = false;
                else if (ReferenceEquals(argument.DefaultValue, null))
                    argument.IsRequired = true;
            }

            argument.OptionStrings = new string[] {};
            return argument;
        }

        private string StripPrefix(string optionString)
        {
            if (string.IsNullOrEmpty(optionString))
                return optionString;
            var localPrefixes = Prefixes;
            return localPrefixes.Where(prefix => optionString.StartsWith(prefix, true, CultureInfo.InvariantCulture))
                .Select(prefix => optionString.Substring(prefix.Length))
                .FirstOrDefault() ?? optionString;
        }

        private Argument PrepareOptionalArgument(Argument argument)
        {
            argument.OptionStrings =
                (argument.OptionStrings ?? new string[] {}).Where(it => it.StartsWith(Prefixes)).ToList();
            if (!argument.OptionStrings.Any())
                throw new Exception("Optional argument should have name starting with prefix");
            // infer destination
            if (string.IsNullOrWhiteSpace(argument.Destination) && argument.OptionStrings.Any())
            {
                var longOptionStrings = argument.OptionStrings.Where(it => it.StartsWith(LongPrefixes)).Take(1).ToList();
                argument.Destination =
                    StripPrefix(longOptionStrings.FirstOrDefault() ?? argument.OptionStrings.FirstOrDefault());
            }
            if (string.IsNullOrWhiteSpace(argument.Destination))
                throw new Exception("Destination should be specified for options like " + argument.OptionStrings[0]);
            return argument;
        }

        public ArgumentParser()
        {
            Prefixes = new[] {"-", "--", "/"};
            RegisterArgumentActions(new Dictionary<string, Func<Argument, IArgumentAction>>
            {
                {"store", arg => new StoreAction(arg)},
                {"count", arg => new CountAction(arg)}
            });
            DefaultAction = "store";
        }

        public string DefaultAction { get; set; }

        private IList<string> LongPrefixes { get; set; }
        private IList<string> ShortPrefixes { get; set; }

        public IList<string> Prefixes
        {
            get { return prefixes; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                prefixes =
                    value.Where(it => !string.IsNullOrWhiteSpace(it))
                        .Distinct()
                        .OrderByDescending(it => it.Length)
                        .ThenBy(it => it)
                        .ToList();
                LongPrefixes = prefixes.Where(it => it.Length > 1).ToList();
                ShortPrefixes = prefixes.Where(it => it.Length == 1).ToList();
            }
        }
    }
}