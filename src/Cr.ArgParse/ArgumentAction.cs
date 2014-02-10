using System.Collections.Generic;

namespace Cr.ArgParse
{
    public abstract class ArgumentAction
    {
        public Argument Argument { get; private set; }
        public ActionContainer Container { get; set; }

        public IList<string> OptionStrings { get; private set; }
        public bool IsRequired { get; set; }

        protected ArgumentAction(Argument argument)
        {
            Argument = argument;
            OptionStrings = new List<string>(Argument.OptionStrings ?? new string[] {});
            Destination = Argument.Destination;
            IsRequired = Argument.IsRequired;
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

        public string Type
        {
            get { return Argument.Type; }
        }

        public bool IsOptional
        {
            get { return ValueCount != null && ValueCount.Min == 0 && ValueCount.Max == 1; }
        }

        public bool IsParser { get; set; }
        public bool IsRemainder { get; set; }

        public abstract void Call(ParseResult parseResult, object values, string optionString);
    }
}