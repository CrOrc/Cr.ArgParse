using System.Collections.Generic;

namespace Cr.ArgParse.Actions
{
    public abstract class ArgumentAction : IArgumentAction
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

        public string TypeName
        {
            get { return Argument.TypeName; }
        }

        public bool IsOptional
        {
            get { return ValueCount != null && ValueCount.Min == 0 && ValueCount.Max == 1; }
        }

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

        public bool IsParser { get; set; }
        public bool IsRemainder { get; set; }

        public abstract void Call(ParseResult parseResult, object values, string optionString);
    }
}