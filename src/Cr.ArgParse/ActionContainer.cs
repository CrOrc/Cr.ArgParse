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
    public class ActionContainer
    {
        private readonly IDictionary<string, Func<Argument, Action>> actionFactories;

        private readonly IList<Action> actions;

        private readonly IDictionary<string, Action> optionStringActions;

        private IList<string> prefixes;
        private readonly Regex negativeNumberMatcher;

        private readonly IDictionary<string, Func<string, object>> typeFactories =
            new Dictionary<string, Func<string, object>>(StringComparer.InvariantCultureIgnoreCase);

        protected Regex NegativeNumberMatcher
        {
            get { return negativeNumberMatcher; }
        }

        public ActionContainer()
            : this("", new[] {"-", "--", "/"}, "resolve")
        {
        }

        public ActionContainer(string description, IList<string> prefixes, string conflictHandlerName)
        {

            typeFactories =
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

            Description = description;
            Prefixes = prefixes;
            ConflictHandlerName = conflictHandlerName;

            DefaultAction = "store";
            actionFactories =
                new Dictionary<string, Func<Argument, Action>>(StringComparer.InvariantCultureIgnoreCase);
            RegisterActions(new Dictionary<string, Func<Argument, Action>>
            {
                {"store", arg => new StoreAction(arg)},
                {"store_const", arg => new StoreConstAction(arg)},
                {"store_true", arg => new StoreTrueAction(arg)},
                {"store_false", arg => new StoreFalseAction(arg)},
                {"append", arg => new AppendAction(arg)},
                {"append_const", arg => new AppendConstAction(arg)},
                {"count", arg => new CountAction(arg)},
                {"parsers", arg => new SubParsersAction(arg)},
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
        }

        public virtual IList<bool> HasNegativeNumberOptionals { get; private set; }

        public IList<ArgumentGroup> ActionGroups { get; private set; }
        public virtual IList<MutuallyExclusiveGroup> MutuallyExclusiveGroups { get; private set; }

        public ArgumentGroup AddArgumentGroup(string title, string description = null)
        {
            var group = new ArgumentGroup(this, title, description ?? title);
            ActionGroups.Add(group);
            return group;
        }

        public MutuallyExclusiveGroup AddMutuallyExclusiveGroup(bool isRequired = false)
        {
            var group = new MutuallyExclusiveGroup(this, isRequired);
            MutuallyExclusiveGroups.Add(group);
            return group;
        }

        private IDictionary<string, Func<Argument, Action>> ActionFactories
        {
            get { return actionFactories; }
        }

        public virtual IList<Action> Actions
        {
            get { return actions; }
        }

        public string ConflictHandlerName { get; set; }
        public string DefaultAction { get; set; }
        public string Description { get; set; }

        public virtual IList<string> LongPrefixes { get; private set; }

        public virtual IDictionary<string, Action> OptionStringActions
        {
            get { return optionStringActions; }
        }

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
                LongPrefixes = prefixes.Where(it => it.Length > 1).ToArray();
                ShortPrefixes = prefixes.Where(it => it.Length == 1).ToArray();
            }
        }

        public virtual IList<string> ShortPrefixes { get; private set; }
        protected Func<string, object> DefaultTypeFactory { get; set; }

        public Action AddArgument(Argument argument)
        {
            var preparedArgument =
                !IsOptionalArgument(argument)
                    ? PreparePositionalArgument(argument)
                    : PrepareOptionalArgument(argument);
            var argumentAction = CreateAction(preparedArgument);
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
            var ret = argument.OptionStrings.IsTrue() &&
                      (argument.OptionStrings.Count != 1 ||
                       StartsWithPrefix(argument.OptionStrings[0]));
            
            return ret;
        }

        private Action<Action, IEnumerable<KeyValuePair<string, Action>>> GetConflictHandler()
        {
            if (StringComparer.InvariantCultureIgnoreCase.Equals("error", ConflictHandlerName))
                return HandleConflictWithError;
            if (StringComparer.InvariantCultureIgnoreCase.Equals("resolve", ConflictHandlerName))
                return HandleConflictWithResolve;
            throw new ParserException(string.Format("Invalid conflict resolution value: {0}", ConflictHandlerName));
        }

        public void CheckConflict(Action action)
        {
            var conflOptionals = new List<KeyValuePair<string, Action>>();
            foreach (var optionString in action.OptionStrings)
            {
                Action conflOptional;
                if (OptionStringActions.TryGetValue(optionString, out conflOptional))
                    conflOptionals.Add(new KeyValuePair<string, Action>(optionString, conflOptional));
            }
            if (!conflOptionals.Any()) return;
            var confictHandler = GetConflictHandler();
            confictHandler(action, conflOptionals);
        }

        private void HandleConflictWithError(Action action,
            IEnumerable<KeyValuePair<string, Action>> conflictingActions)
        {
            var conflictionOptions = conflictingActions.Select(it => it.Key).ToList();
            var conflictString = string.Join(", ", conflictionOptions);
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

        public virtual Action AddAction(Action action)
        {
            if (ReferenceEquals(action, null))
                throw new ArgumentNullException("action");
            CheckConflict(action);

            Actions.Add(action);
            action.Container = this;

            foreach (var optionString in action.OptionStrings)
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
            var argumentActionFactory = argument.ActionFactory ??
                                        ActionFactories.SafeGetValue(!string.IsNullOrWhiteSpace(argument.ActionName)
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
            var valueCount = res.ValueCount;
            if (valueCount == null || valueCount.Min.HasValue && valueCount.Min > 0)
                res.IsRequired = true;
            else
            {
                if (valueCount.Max == 1)
                    res.IsRequired = false;
                else if (ReferenceEquals(res.DefaultValue, null))
                    res.IsRequired = true;
            }

            if (string.IsNullOrEmpty(res.Destination) && argument.OptionStrings.IsTrue())
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
                AddTypeFactory(kv.Key, CreateSimpleTypeFactory(kv.Value));
        }

        private Func<string, object> CreateSimpleTypeFactory(Type targetType)
        {
            return argString => Convert.ChangeType(argString, targetType);
        }

        public void AddTypeFactory(string typeName, Func<string, object> typeFactory)
        {
            if (!string.IsNullOrEmpty(typeName))
                typeFactories[typeName] = typeFactory;
            else
            {
                if (typeFactory != null)
                    DefaultTypeFactory = typeFactory;
            }
        }

        protected Func<string, object> GetTypeFactory(string typeName)
        {
            return typeFactories.SafeGetValue(typeName, DefaultTypeFactory) ?? DefaultTypeFactory;
        }
    }
}