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
        public object DefaultValue { get { return Argument.DefaultValue; } }
        public string MetaVariable { get { return Argument.MetaVariable; }}

        public abstract void Call(ParseResult parseResult, object values, string optionString);
    }
}