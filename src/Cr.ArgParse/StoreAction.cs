namespace Cr.ArgParse
{
    public class StoreAction : ArgumentAction
    {
        public StoreAction(Argument argument) : base(argument)
        {
        }

        public override void Call(IArgumentParser parser, ParseResult parseResult, object values, string optionString)
        {
            parseResult.SaveArgument(Argument.Destination, values);
        }
    }
}