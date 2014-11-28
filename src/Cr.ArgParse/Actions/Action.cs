using System;
using System.Collections.Generic;

namespace Cr.ArgParse.Actions
{
    public abstract class Action : IAction
    {
        protected Action(Argument argument)
        {
            Argument = argument;
            OptionStrings = new List<string>(Argument.OptionStrings ?? new string[] {});
            Destination = Argument.Destination;
            IsRequired = Argument.IsRequired;
        }

        public Argument Argument { get; private set; }

        public IList<object> Choices
        {
            get { return Argument.Choices; }
        }

        public virtual object ConstValue
        {
            get { return Argument.ConstValue; }
        }

        public ActionContainer Container { get; set; }

        public virtual object DefaultValue
        {
            get { return Argument.DefaultValue; }
        }

        public string Destination { get; private set; }

        public virtual bool HasDefaultValue
        {
            get { return !Argument.SuppressDefaultValue; }
        }

        public virtual bool HasDestination
        {
            get { return true; }
        }

        public bool HasValidDestination
        {
            get { return !string.IsNullOrWhiteSpace(Destination); }
        }

        public bool IsOptional
        {
            get { return !IsSpecial && ValueCount == ValueCount.Optional; }
        }

        public virtual bool IsParser
        {
            get { return false; }
        }

        public bool IsRemainder
        {
            get { return Argument.IsRemainder; }
        }

        public bool IsRequired { get; set; }

        public bool IsSingleOrOptional
        {
            get
            {
                return !IsSpecial &&
                       (ValueCount == null || ValueCount == ValueCount.Optional)
                    ;
            }
        }

        public bool IsSpecial
        {
            get { return IsParser || IsRemainder; }
        }

        public bool IsZeroOrMore
        {
            get
            {
                return
                    !IsSpecial && (ValueCount != null && ValueCount.IsZeroOrMore);
            }
        }

        public string MetaVariable
        {
            get { return Argument.MetaVariable; }
        }

        public IList<string> OptionStrings { get; private set; }

        public Type Type
        {
            get { return Argument.Type; }
        }

        public Func<string, object> TypeFactory
        {
            get { return Argument.TypeFactory; }
        }

        public string TypeName
        {
            get { return Argument.TypeName; }
        }

        public virtual ValueCount ValueCount
        {
            get { return Argument.ValueCount; }
        }

        public abstract void Call(ParseResult parseResult, object values, string optionString);
    }
}