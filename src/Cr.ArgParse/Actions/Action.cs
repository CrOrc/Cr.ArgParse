using System;
using System.Collections.Generic;

namespace Cr.ArgParse.Actions
{
    public abstract class Action : IAction
    {
        public Argument Argument { get; private set; }
        public ActionContainer Container { get; set; }

        public IList<string> OptionStrings { get; private set; }
        public bool IsRequired { get; set; }

        protected Action(Argument argument)
        {
            Argument = argument;
            OptionStrings = new List<string>(Argument.OptionStrings ?? new string[] {});
            Destination = Argument.Destination;
            IsRequired = Argument.IsRequired;
        }

        public Func<string, object> TypeFactory
        {
            get { return Argument.TypeFactory; }
        }

        public string Destination { get; private set; }

        public virtual object DefaultValue
        {
            get { return Argument.DefaultValue; }
        }

        public virtual object ConstValue
        {
            get { return Argument.ConstValue; }
        }

        public string MetaVariable
        {
            get { return Argument.MetaVariable; }
        }

        public virtual ValueCount ValueCount
        {
            get { return Argument.ValueCount; }
        }

        public IList<object> Choices
        {
            get { return Argument.Choices; }
        }

        public string TypeName
        {
            get { return Argument.TypeName; }
        }

        public Type Type
        {get { return Argument.Type; }}

        public bool IsOptional
        {
            get { return !IsSpecial && ValueCount == ValueCount.Optional; }
        }

        public bool IsZeroOrMore
        {
            get
            {
                return
                    !IsSpecial && (ValueCount != null && ValueCount.IsZeroOrMore);
            }
        }

        public bool IsSingleOrOptional
        {
            get{return !IsSpecial &&
                ( ValueCount == null || ValueCount == ValueCount.Optional)
                ;
            }
        }

        public bool IsSpecial{get { return IsParser || IsRemainder; }}

        public virtual bool HasDestination
        {
            get { return true; }
        }

        public virtual bool HasDefaultValue
        {
            get { return !Argument.SuppressDefaultValue; }
        }

        public bool HasValidDestination
        {
            get { return !string.IsNullOrWhiteSpace(Destination); }
        }

        public virtual bool IsParser
        {
            get { return false; }
        }

        public bool IsRemainder
        {
            get { return Argument.IsRemainder; }
        }

        public abstract void Call(ParseResult parseResult, object values, string optionString);
    }
}