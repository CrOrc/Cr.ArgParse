namespace Cr.ArgParse.Actions
{
    public class StoreAction : ArgumentAction
    {
        public StoreAction(Argument argument) : base(argument)
        {
        }

        public override void Call(ParseResult parseResult, object values, string optionString)
        {
            parseResult[Destination] = values;
        }
    }
}