using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Cr.ArgParse.Actions;
using Cr.ArgParse.Exceptions;
using Cr.ArgParse.Extensions;
using Action = Cr.ArgParse.Actions.Action;

namespace Cr.ArgParse
{
    public class ActionContainer : IActionContainer
    {
        private readonly IDictionary<string, System.Func<Argument, Action>> actionFactories;

        private readonly IList<Action> actions;

        private readonly Regex negativeNumberMatcher;
        private readonly IDictionary<string, Action> optionStringActions;

        private readonly IDictionary<string, Func<string, object>> typeFactoriesByName =
            new Dictionary<string, Func<string, object>>(StringComparer.InvariantCultureIgnoreCase);

        private readonly IDictionary<Type, Func<string, object>> typeFactoriesByType =
            new Dictionary<Type, Func<string, object>>();

        private IList<string> prefixes;

        public ActionContainer(ParserSettings parserSettings = null, IActionContainer container = null)
        {
            parserSettings = parserSettings ?? new ParserSettings();
            typeFactoriesByName =
                new Dictionary<string, Func<string, object>>(StringComparer.InvariantCultureIgnoreCase);
            DefaultTypeFactory = argString => argString;
            AddSimpleTypeFactories(new Dictionary<string, Type>
            {
                {"int", typeof (int)},
                {"uint", typeof (uint)},
                {"float", typeof (float)},
                {"double", typeof (double)},
                {"datetime", typeof (DateTime)},
                {"timespan", typeof (TimeSpan)}
            });

            Prefixes = parserSettings.Prefixes;
            ConflictHandlerType = parserSettings.ConflictHandlerType;

            DefaultAction = "store";
            actionFactories =
                new Dictionary<string, Func<Argument, Action>>(StringComparer.InvariantCultureIgnoreCase);
            RegisterActions(new Dictionary<string, Func<Argument, Action>>
            {
                {"store", arg => new StoreAction(arg, this)},
                {"store_const", arg => new StoreConstAction(arg, this)},
                {"store_true", arg => new StoreTrueAction(arg, this)},
                {"store_false", arg => new StoreFalseAction(arg, this)},
                {"append", arg => new AppendAction(arg, this)},
                {"append_const", arg => new AppendConstAction(arg, this)},
                {"count", arg => new CountAction(arg, this)},
                {"parsers", arg => new SubParsersAction(arg, this)},
            });

            //check conflict resolving
            GetConflictHandler();

            actions = new List<Action>();

            optionStringActions =
                new Dictionary<string, Action>(StringComparer.InvariantCulture);

            //groups
            ActionGroups = new List<ArgumentGroup>();
            MutuallyExclusiveGroups = new List<MutuallyExclusiveGroup>();

            // determines whether an "option" looks like a negative number
            negativeNumberMatcher = new Regex(@"^-\d+(\.\d*)?$|^-\.\d+$");

            // whether or not there are any optionals that look like negative
            // numbers -- uses a list so it can be shared and edited
            HasNegativeNumberOptionals = new List<bool>();

            Container = container ?? this;
        }

        private IDictionary<string, Func<Argument, Action>> ActionFactories
        {
            get { return actionFactories; }
        }

        public IList<ArgumentGroup> ActionGroups { get; private set; }

        public virtual IList<Action> Actions
        {
            get { return actions; }
        }

        IList<Action> IActionContainer.Actions{get { return Actions; }}

        private ConflictHandlerType ConflictHandlerType { get; set; }
        protected IActionContainer Container { get; private set; }
        public string DefaultAction { get; set; }
        protected Func<string, object> DefaultTypeFactory { get; set; }
        public virtual IList<bool> HasNegativeNumberOptionals { get; private set; }

        private IEnumerable<string> LongPrefixes
        {
            get { return Prefixes.Where(it => it.Length > 1); }
        }

        protected virtual IList<MutuallyExclusiveGroup> MutuallyExclusiveGroups { get; set; }

        IList<MutuallyExclusiveGroup> IActionContainer.MutuallyExclusiveGroups
        {
            get { return MutuallyExclusiveGroups; }
        }

        protected Regex NegativeNumberMatcher
        {
            get { return negativeNumberMatcher; }
        }

        protected virtual IDictionary<string, Action> OptionStringActions
        {
            get { return optionStringActions; }
        }

        IDictionary<string, Action> IActionContainer.OptionStringActions{get { return OptionStringActions; }}

        public virtual IList<string> Prefixes
        {
            get { return prefixes; }
            private set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                prefixes =
                    value.Where(it => !string.IsNullOrWhiteSpace(it))
                        .Distinct()
                        .OrderByDescending(it => it.Length)
                        .ThenBy(it => it)
                        .ToArray();
            }
        }

        private IEnumerable<string> ShortPrefixes
        {
            get { return Prefixes.Where(it => it.Length == 1); }
        }

        public virtual Action AddAction(Action action)
        {
            if (ReferenceEquals(action, null))
                throw new ArgumentNullException("action");
            CheckConflict(action);

            Actions.Add(action);
            action.Container = this;

            foreach (string optionString in action.OptionStrings)
                OptionStringActions[optionString] = action;

            if (!HasNegativeNumberOptionals.IsTrue() &&
                action.OptionStrings.Any(optionString => NegativeNumberMatcher.IsMatch(optionString)))
                HasNegativeNumberOptionals.Add(true);
            return action;
        }

        public virtual void RemoveAction(Action action)
        {
            Actions.Remove(action);
        }

        public ArgumentGroup AddArgumentGroup(string title, string description = null)
        {
            var group = new ArgumentGroup(this);
            ActionGroups.Add(group);
            return group;
        }

        public MutuallyExclusiveGroup AddMutuallyExclusiveGroup(bool isRequired = false)
        {
            var group = new MutuallyExclusiveGroup(this, isRequired);
            MutuallyExclusiveGroups.Add(group);
            return group;
        }

        public Action AddArgument(params string[] optionStrings)
        {
            return AddArgument(new Argument(optionStrings));
        }

        public Action AddArgument(Argument argument)
        {
            Argument preparedArgument =
                !IsOptionalArgument(argument)
                    ? PreparePositionalArgument(argument)
                    : PrepareOptionalArgument(argument);
            Action argumentAction = CreateAction(preparedArgument);
            if (ReferenceEquals(argumentAction, null))
                throw new ParserException("Unregistered action exception");
            return AddAction(argumentAction);
        }

        protected bool StartsWithPrefix(string optionString)
        {
            return optionString.StartsWith(Prefixes);
        }

        protected bool StartsWithShortPrefix(string optionString)
        {
            return optionString.StartsWith(ShortPrefixes) && !optionString.StartsWith(LongPrefixes);
        }

        protected bool StartsWithLongPrefix(string optionString)
        {
            return optionString.StartsWith(LongPrefixes);
        }

        private bool IsOptionalArgument(Argument argument)
        {
            bool ret = !argument.OptionStrings.IsNullOrEmpty() &&
                       (argument.OptionStrings.Count != 1 ||
                        StartsWithPrefix(argument.OptionStrings[0]));

            return ret;
        }

        private Action<Action, IEnumerable<KeyValuePair<string, Action>>> GetConflictHandler()
        {
            switch (ConflictHandlerType)
            {
                case ConflictHandlerType.Error:
                    return HandleConflictWithError;
                case ConflictHandlerType.Resolve:
                    return HandleConflictWithResolve;
            }
            throw new ParserException(string.Format("Invalid conflict resolution value: {0}", ConflictHandlerType));
        }

        public void CheckConflict(Action action)
        {
            var conflOptionals = new List<KeyValuePair<string, Action>>();
            foreach (string optionString in action.OptionStrings)
            {
                Action conflOptional;
                if (OptionStringActions.TryGetValue(optionString, out conflOptional))
                    conflOptionals.Add(new KeyValuePair<string, Action>(optionString, conflOptional));
            }
            if (!conflOptionals.Any()) return;
            Action<Action, IEnumerable<KeyValuePair<string, Action>>> confictHandler = GetConflictHandler();
            confictHandler(action, conflOptionals);
        }

        private void HandleConflictWithError(Action action,
            IEnumerable<KeyValuePair<string, Action>> conflictingActions)
        {
            List<string> conflictionOptions = conflictingActions.Select(it => it.Key).ToList();
            string conflictString = string.Join(", ", conflictionOptions);
            if (conflictionOptions.Count == 1)
                throw new ParserException(string.Format("Confliction option string: {0}", conflictString));
            throw new ParserException(string.Format("Confliction option strings: {0}", conflictString));
        }

        private void HandleConflictWithResolve(Action action,
            IEnumerable<KeyValuePair<string, Action>> conflictingActions)
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

        private string StripPrefix(string optionString)
        {
            if (string.IsNullOrEmpty(optionString))
                return optionString;
            return Prefixes.Where(prefix => optionString.StartsWith(prefix, true, CultureInfo.InvariantCulture))
                .Select(prefix => optionString.Substring(prefix.Length))
                .FirstOrDefault() ?? optionString;
        }

        private Action CreateAction(Argument argument)
        {
            Func<Argument, Action> argumentActionFactory = argument.ActionFactory ??
                                                           ActionFactories.SafeGetValue(
                                                               !string.IsNullOrWhiteSpace(argument.ActionName)
                                                                   ? argument.ActionName
                                                                   : DefaultAction);
            return argumentActionFactory != null ? argumentActionFactory(argument) : null;
        }

        protected void RegisterActions(
            IEnumerable<KeyValuePair<string, Func<Argument, Action>>> newActionFactories)
        {
            foreach (var kv in newActionFactories)
                ActionFactories[kv.Key] = kv.Value;
        }

        private Argument PreparePositionalArgument(Argument argument)
        {
            var res = new Argument(argument, new string[] {});
            // mark positional arguments as required if at least one is always required
            ValueCount valueCount = res.ValueCount;
            if (valueCount == null || valueCount.Min.HasValue && valueCount.Min > 0)
                res.IsRequired = true;
            else
            {
                if (valueCount.Max == 1)
                    res.IsRequired = false;
                else if (ReferenceEquals(res.DefaultValue, null))
                    res.IsRequired = true;
            }

            if (string.IsNullOrEmpty(res.Destination) && !argument.OptionStrings.IsNullOrEmpty())
                res.Destination = argument.OptionStrings.FirstOrDefault();

            return res;
        }

        private Argument PrepareOptionalArgument(Argument argument)
        {
            var res = new Argument(argument, (argument.OptionStrings ?? new string[] {}).Where(StartsWithPrefix));
            if (!res.OptionStrings.Any())
                throw new Exception("Optional argument should have name starting with prefix");
            // infer destination
            if (string.IsNullOrWhiteSpace(res.Destination) && res.OptionStrings.Any())
                res.Destination =
                    StripPrefix(res.OptionStrings.FirstOrDefault(StartsWithLongPrefix) ??
                                res.OptionStrings.FirstOrDefault());

            if (string.IsNullOrWhiteSpace(res.Destination))
                throw new ParserException("Destination should be specified for options like " + res.OptionStrings[0]);
            res.Destination = res.Destination.Replace('-', '_');
            return res;
        }

        protected List<Action> GetOptionalActions()
        {
            return Actions.Where(it => it.OptionStrings.Any()).ToList();
        }

        protected List<Action> GetPositionalActions()
        {
            return Actions.Where(it => !it.OptionStrings.Any()).ToList();
        }

        protected void AddSimpleTypeFactories(IEnumerable<KeyValuePair<string, Type>> typeFactoryPairs)
        {
            foreach (var kv in typeFactoryPairs)
                AddTypeFactory(kv.Key, kv.Value, CreateSimpleTypeFactory(kv.Value));
        }

        private Func<string, object> CreateSimpleTypeFactory(Type targetType)
        {
            return argString => Convert.ChangeType(argString, targetType);
        }

        public void AddTypeFactory(string typeName, Type type, Func<string, object> typeFactory)
        {
            if (!ReferenceEquals(type, null))
                typeFactoriesByType[type] = typeFactory;
            if (!string.IsNullOrEmpty(typeName))
                typeFactoriesByName[typeName] = typeFactory;

            if (ReferenceEquals(type, null) && string.IsNullOrEmpty(typeName) && typeFactory != null)
                DefaultTypeFactory = typeFactory;
        }

        protected Func<string, object> GetTypeFactory(string typeName)
        {
            return typeFactoriesByName.SafeGetValue(typeName);
        }

        protected Func<string, object> GetTypeFactory(Type type)
        {
            return typeFactoriesByType.SafeGetValue(type);
        }
    }
}