using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cr.ArgParse.Extensions;

namespace Cr.ArgParse
{
    public class ActionContainer
    {
        private readonly IDictionary<string, Func<Argument, ArgumentAction>> actionFactories;

        private readonly IList<ArgumentAction> actions;

        private readonly IDictionary<string, ArgumentAction> optionStringActions;

        private IList<string> prefixes;

        public ActionContainer()
            : this("", new[] {"-", "--", "/"}, "resolve")
        {
        }

        public ActionContainer(string description, IList<string> prefixes, string conflictHandlerName)
        {
            Description = description;
            Prefixes = prefixes;
            ConflictHandlerName = conflictHandlerName;

            DefaultAction = "store";
            actionFactories =
                new Dictionary<string, Func<Argument, ArgumentAction>>(StringComparer.InvariantCultureIgnoreCase);
            RegisterActions(new Dictionary<string, Func<Argument, ArgumentAction>>
            {
                {"store", arg => new StoreAction(arg)},
                {"count", arg => new CountAction(arg)}
            });

            //check conflict resolving
            var conflictHandler = GetConflictHandler();

            actions = new List<ArgumentAction>();

            optionStringActions =
                new Dictionary<string, ArgumentAction>(StringComparer.InvariantCultureIgnoreCase);

            //groups
            ActionGroups = new List<ArgumentGroup>();
            MutuallyExclusiveGroups = new List<MutuallyExclusiveGroup>();
        }

        public IList<ArgumentGroup> ActionGroups { get; private set; }
        public virtual IList<MutuallyExclusiveGroup> MutuallyExclusiveGroups { get; private set; }

        public ArgumentGroup AddArgumentGroup(string title, string description)
        {
            var group = new ArgumentGroup(this, title, description);
            ActionGroups.Add(group);
            return group;
        }

        public MutuallyExclusiveGroup AddMutuallyExclusiveGroup(bool isRequired = false)
        {
            var group = new MutuallyExclusiveGroup(this, isRequired);
            MutuallyExclusiveGroups.Add(group);
            return group;
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
                        .ToArray();
                LongPrefixes = prefixes.Where(it => it.Length > 1).ToArray();
                ShortPrefixes = prefixes.Where(it => it.Length == 1).ToArray();
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
            var argumentAction = CreateAction(preparedArgument);
            if (ReferenceEquals(argumentAction, null))
                throw new Exception("Unregistered action exception");
            return AddAction(argumentAction);
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
                    kv.Value.Container.RemoveAction(kv.Value);
            }
        }

        public virtual ArgumentAction AddAction(ArgumentAction argumentAction)
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

        public virtual void RemoveAction(ArgumentAction argumentAction)
        {
            Actions.Remove(argumentAction);
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

        private ArgumentAction CreateAction(Argument argument)
        {
            var argumentAction = argument.Action;
            if (argumentAction != null)
                return argumentAction;
            var argumentActionFactory = ActionFactories.SafeGetValue(argument.ActionName) ??
                                        ActionFactories.SafeGetValue(DefaultAction);
            return argumentActionFactory != null ? argumentActionFactory(argument) : null;
        }

        protected void RegisterActions(
            IEnumerable<KeyValuePair<string, Func<Argument, ArgumentAction>>> newActionFactories)
        {
            foreach (var kv in newActionFactories)
                ActionFactories[kv.Key] = kv.Value;
        }

        private Argument PreparePositionalArgument(Argument argument)
        {
            var res = new Argument(argument);
            if (ReferenceEquals(res.ValueCount, null))
                res.ValueCount = new ValueCount(1);
            // mark positional arguments as required if at least one is always required
            var valueCount = res.ValueCount;
            if (valueCount.Min.HasValue && valueCount.Min > 0)
                res.IsRequired = true;
            else
            {
                if (valueCount.Max == 1)
                    res.IsRequired = false;
                else if (ReferenceEquals(res.DefaultValue, null))
                    res.IsRequired = true;
            }

            res.OptionStrings = new string[] {};
            return res;
        }

        private Argument PrepareOptionalArgument(Argument argument)
        {
            var res = new Argument(argument);
            res.OptionStrings =
                (argument.OptionStrings ?? new string[] {}).Where(it => it.StartsWith(Prefixes)).ToList();
            if (!res.OptionStrings.Any())
                throw new Exception("Optional argument should have name starting with prefix");
            // infer destination
            if (string.IsNullOrWhiteSpace(res.Destination) && res.OptionStrings.Any())
            {
                var longOptionStrings = res.OptionStrings.Where(it => it.StartsWith(LongPrefixes)).Take(1).ToList();
                res.Destination =
                    StripPrefix(longOptionStrings.FirstOrDefault() ?? res.OptionStrings.FirstOrDefault());
            }
            if (string.IsNullOrWhiteSpace(res.Destination))
                throw new Exception("Destination should be specified for options like " + argument.OptionStrings[0]);
            return res;
        }
    }
}