namespace Cr.ArgParse
{
    public abstract class ArgumentAction : IArgumentAction
    {
        public Argument Argument { get; private set; }

        public ArgumentAction(Argument argument)
        {
            Argument = argument;
        }

        public abstract void Call(IArgumentParser parser, ParseResult parseResult, object values, string optionString);
    }
}