namespace Cr.ArgParse
{
    public class StoreAction : ArgumentAction
    {
        public StoreAction(Argument argument) : base(argument)
        {
        }

        public override void Call(ParseResult parseResult, object values, string optionString)
        {
            parseResult.SaveArgument(Destination, values);
        }
    }
}