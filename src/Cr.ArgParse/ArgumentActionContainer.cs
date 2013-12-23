using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cr.ArgParse.Extensions;

namespace Cr.ArgParse
{
    public class ArgumentActionContainer : IArgumentActionContainer
    {
        private readonly IDictionary<string, Func<Argument, ArgumentAction>> actionFactories =
            new Dictionary<string, Func<Argument, ArgumentAction>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly IList<ArgumentAction> actions = new List<ArgumentAction>();

        private readonly IDictionary<string, ArgumentAction> optionStringActions =
            new Dictionary<string, ArgumentAction>(StringComparer.InvariantCultureIgnoreCase);

        private IList<string> prefixes;

        protected ArgumentActionContainer()
            : this("", new[] {"-", "--", "/"}, "resolve")
        {
        }

        public ArgumentActionContainer(string description, IList<string> prefixes, string conflictHandlerName)
        {
            Description = description;
            Prefixes = prefixes;
            ConflictHandlerName = conflictHandlerName;
            //check conflict resolving
            var conflictHandler = GetConflictHandler();
        }

        private IDictionary<string, Func<Argument, ArgumentAction>> ActionFactories
        {
            get { return actionFactories; }
        }

        public virtual IList<ArgumentAction> Actions
        {
            get { return actions; }
        }

        public string ConflictHandlerName { get; set; }
        public string DefaultAction { get; set; }
        public string Description { get; set; }

        public IList<string> LongPrefixes { get; private set; }

        public virtual IDictionary<string, ArgumentAction> OptionStringActions
        {
            get { return optionStringActions; }
        }

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

        public IList<string> ShortPrefixes { get; private set; }

        public ArgumentAction AddArgument(Argument argument)
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
            return AddArgumentAction(argumentAction);
        }

        private Action<ArgumentAction, IEnumerable<KeyValuePair<string, ArgumentAction>>> GetConflictHandler()
        {
            if (StringComparer.InvariantCultureIgnoreCase.Equals("error", ConflictHandlerName))
                return HandleConflictWithError;
            if (StringComparer.InvariantCultureIgnoreCase.Equals("resolve", ConflictHandlerName))
                return HandleConflictWithResolve;
            throw new Exception(string.Format("Invalid conflict resolution value: {0}", ConflictHandlerName));
        }

        public void CheckConflict(ArgumentAction action)
        {
            var conflOptionals = new List<KeyValuePair<string, ArgumentAction>>();
            foreach (var optionString in action.OptionStrings)
            {
                ArgumentAction conflOptional;
                if (OptionStringActions.TryGetValue(optionString, out conflOptional))
                    conflOptionals.Add(new KeyValuePair<string, ArgumentAction>(optionString, conflOptional));
            }
            if (!conflOptionals.Any()) return;
            var confictHandler = GetConflictHandler();
            confictHandler(action, conflOptionals);
        }

        private void HandleConflictWithError(ArgumentAction action,
            IEnumerable<KeyValuePair<string, ArgumentAction>> conflictingActions)
        {
            var conflictionOptions = conflictingActions.Select(it => it.Key).ToList();
            var conflictString = string.Join(", ", conflictionOptions);
            if (conflictionOptions.Count == 1)
                throw new Exception(string.Format("Confliction option string: {0}", conflictString));
            throw new Exception(string.Format("Confliction option strings: {0}", conflictString));
        }

        private void HandleConflictWithResolve(ArgumentAction action,
            IEnumerable<KeyValuePair<string, ArgumentAction>> conflictingActions)
        {
            //remove all conflicting options
            foreach (var kv in conflictingActions)
            {
                kv.Value.OptionStrings.Remove(kv.Key);
                OptionStringActions.Remove(kv.Key);

                //if the option now has no option string, remove it from the container holding it
                if (!kv.Value.OptionStrings.Any())
                    kv.Value.Container.RemoveArgumentAction(kv.Value);
            }
        }

        public virtual ArgumentAction AddArgumentAction(ArgumentAction argumentAction)
        {
            if (ReferenceEquals(argumentAction, null))
                throw new ArgumentNullException("argumentAction");
            CheckConflict(argumentAction);

            Actions.Add(argumentAction);
            argumentAction.Container = this;

            foreach (var optionString in argumentAction.OptionStrings)
                OptionStringActions[optionString] = argumentAction;
            return argumentAction;
        }

        public virtual void RemoveArgumentAction(ArgumentAction argumentAction)
        {
            Actions.Remove(argumentAction);
        }

        protected string StripPrefix(string optionString)
        {
            if (string.IsNullOrEmpty(optionString))
                return optionString;
            var localPrefixes = Prefixes;
            return Enumerable.Where<string>(localPrefixes,
                prefix => optionString.StartsWith(prefix, true, CultureInfo.InvariantCulture))
                .Select(prefix => optionString.Substring(prefix.Length))
                .FirstOrDefault() ?? optionString;
        }

        private ArgumentAction CreateArgumentAction(Argument argument)
        {
            var argumentAction = argument.Action;
            if (argumentAction != null)
                return argumentAction;
            var argumentActionFactory = ActionFactories.SafeGetValue(argument.ActionName) ??
                                        ActionFactories.SafeGetValue(DefaultAction);
            return argumentActionFactory != null ? argumentActionFactory(argument) : null;
        }

        protected void RegisterArgumentActions(
            IEnumerable<KeyValuePair<string, Func<Argument, ArgumentAction>>> newActionFactories)
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
    }
}