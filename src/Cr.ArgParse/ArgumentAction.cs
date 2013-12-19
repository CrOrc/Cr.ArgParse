namespace Cr.ArgParse
{
    public class ArgumentAction : IArgumentAction
    {
        public Argument Argument { get; private set; }

        public ArgumentAction(Argument argument)
        {
            Argument = argument;
        }

        public virtual void Call(IArgumentParser parser, ParseResult parseResult, object values, string optionString)
        {
            throw new System.NotImplementedException();
        }
    }
}