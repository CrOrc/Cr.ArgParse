using System;
using System.Collections.Generic;
using System.Linq;
using Cr.ArgParse.Actions;

namespace Cr.ArgParse
{
    /// <summary>
    /// Single argument description
    /// </summary>
    public class Argument
    {
        public Argument()
        {
        }

        public Argument(Argument argument, IEnumerable<string> optionStrings = null)
            : this(optionStrings ?? (argument != null ? argument.OptionStrings : null))
        {
            if (ReferenceEquals(argument, null)) return;
            ActionFactory = argument.ActionFactory;
            ActionName = argument.ActionName;
            ConstValue = argument.ConstValue;
            DefaultValue = argument.DefaultValue;
            Destination = argument.Destination;
            HelpText = argument.HelpText;
            IsRemainder = argument.IsRemainder;
            IsRequired = argument.IsRequired;
            MetaVariable = argument.MetaVariable;
            TypeName = argument.TypeName;
            ValueCount = argument.ValueCount;
        }

        public Argument(params string[] optionStrings) : this(optionStrings as IEnumerable<string>)
        {
        }

        public Argument(IEnumerable<string> optionStrings)
        {
            OptionStrings = (optionStrings ?? new string[] {}).ToList();
        }

        /// <summary>
        /// Custom action to be performed on argument. If present <see cref="ActionName"/> will be ignored.
        /// </summary>
        public Func<Argument, ArgumentAction> ActionFactory { get; set; }

        /// <summary>
        /// Name of predefined action.
        /// One of "store", "store_const", "store_true", "store_false", "append", "append_const", "count".
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Default argument value
        /// </summary>
        public object ConstValue { get; set; }

        /// <summary>
        /// Default argument value
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Name for storing parsing results.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Description of argument
        /// </summary>
        public string HelpText { get; set; }

        public bool SuppressDefaultValue { get; set; }

        /// <summary>
        /// This argument should contain all remaining args
        /// </summary>
        public bool IsRemainder { get; set; }

        /// <summary>
        /// Is argument required.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Argument to show in help / errors
        /// </summary>
        public string MetaVariable { get; set; }

        /// <summary>
        /// strings to identify argument. Empty for positional arguments.
        /// </summary>
        public IList<string> OptionStrings { get; private set; }

        /// <summary>
        /// Name argument type
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Count of values for this argument.
        /// </summary>
        public ValueCount ValueCount { get; set; }
    }
}