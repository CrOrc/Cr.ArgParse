using System;
using System.Collections.Generic;

namespace Cr.ArgParse
{
    /// <summary>
    /// Single argument description
    /// </summary>
    public class Argument
    {
        public Argument(Argument argument=null)
        {
            if (ReferenceEquals(argument,null)) return;
            OptionStrings = argument.OptionStrings;
            Destination = argument.Destination;
            HelpText = argument.HelpText;
            ActionName = argument.ActionName;
            ActionFactory = argument.ActionFactory;
            ValueCount = argument.ValueCount;
            IsRequired = argument.IsRequired;
            DefaultValue = argument.DefaultValue;
            ConstValue = argument.ConstValue;
        }

        /// <summary>
        /// strings to identify argument. Empty for positional arguments.
        /// </summary>
        public IList<string> OptionStrings { get; set;}

        /// <summary>
        /// Name for storing parsing results.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Description of argument
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Name of predefined action.
        /// One of "store", "store_const", "store_true", "store_false", "append", "append_const", "count".
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Custom action to be performed on argument. If present <see cref="ActionName"/> will be ignored.
        /// </summary>
        public Func<Argument, ArgumentAction> ActionFactory { get; set; }

        /// <summary>
        /// Count of values for this argument.
        /// </summary>
        public ValueCount ValueCount { get; set; }

        /// <summary>
        /// Is argument required.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Default argument value
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Default argument value
        /// </summary>
        public object ConstValue { get; set; }

    }
}