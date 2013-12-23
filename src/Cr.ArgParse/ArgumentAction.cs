using System.Collections.Generic;

namespace Cr.ArgParse
{
    public abstract class ArgumentAction
    {
        public Argument Argument { get; private set; }
        public ArgumentActionContainer Container { get; set; }

        public IList<string> OptionStrings { get; private set; }

        public ArgumentAction(Argument argument)
        {
            Argument = argument;
            OptionStrings = new List<string>(Argument.OptionStrings ?? new string[] {});
            Destination = Argument.Destination;
        }

        public string Destination { get; private set; }

        public abstract void Call(ParseResult parseResult, object values, string optionString);
    }
}